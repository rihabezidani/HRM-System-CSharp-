using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rh.Data;
using Rh.Models;

namespace Rh.Controllers
{
    public class PaieController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaieController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // VUE ADMIN : Liste tous les bulletins
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Forbid();

            // On récupère tous les bulletins avec les infos de l'employé lié
            var bulletins = await _context.BulletinsPaie
                .Include(b => b.Employe)
                .OrderByDescending(b => b.DateCreation)
                .ToListAsync();

            // Préparation de la liste des employés pour la modale de génération
            ViewBag.Employees = new SelectList(await _context.Employees.ToListAsync(), "Id", "Nom");

            return View(bulletins);
        }

        // =============================================================
        // ACTION : Générer un bulletin avec calcul dynamique des congés
        // =============================================================
        [HttpPost]
        public async Task<IActionResult> Generer(int employeId, DateTime datePeriode)
        {
            // 1. Récupération de l'employé
            var employe = await _context.Employees.FindAsync(employeId);
            if (employe == null) return NotFound();

            // 2. Définition de la période (Début et Fin du mois sélectionné)
            var debutMois = new DateTime(datePeriode.Year, datePeriode.Month, 1);
            var finMois = debutMois.AddMonths(1).AddDays(-1);

            // 3. Récupération des congés approuvés pour cet employé durant ce mois
            var congesDuMois = await _context.Conges
                .Where(c => c.EmployeId == employeId &&
                            c.Statut == "Approuvé" &&
                            ((c.DateDebut >= debutMois && c.DateDebut <= finMois) ||
                             (c.DateFin >= debutMois && c.DateFin <= finMois)))
                .ToListAsync();

            // 4. Calcul des jours de Maladie (seul type impactant le salaire ici)
            double joursMaladie = 0;
            foreach (var conge in congesDuMois)
            {
                if (conge.TypeConge == "Maladie")
                {
                    // On s'assure de ne compter que les jours à l'intérieur du mois choisi
                    var start = conge.DateDebut < debutMois ? debutMois : conge.DateDebut;
                    var end = conge.DateFin > finMois ? finMois : conge.DateFin;
                    joursMaladie += (end - start).TotalDays + 1;
                }
            }

            // 5. Calcul des montants financiers
            decimal brutBase = employe.SalaireBase;
            decimal tauxJournalier = brutBase / 22; // Moyenne de 22 jours ouvrés par mois

            // Calcul de la retenue pour absence maladie
            decimal deductionAbsence = tauxJournalier * (decimal)joursMaladie;

            // Nouveau Brut après déduction
            decimal nouveauBrut = brutBase - deductionAbsence;

            // Calcul des charges sociales (ex: 20%) sur le nouveau brut
            decimal retenuesSociales = nouveauBrut * 0.20m;

            // Salaire Net Final
            decimal salaireNet = nouveauBrut - retenuesSociales;

            // 6. Création et enregistrement du bulletin
            var bulletin = new BulletinPaie
            {
                EmployeId = employeId,
                // On transforme la date en texte lisible pour la colonne "Periode"
                Periode = datePeriode.ToString("MMMM yyyy"),
                SalaireBrut = Math.Round(nouveauBrut, 2),
                Retenues = Math.Round(retenuesSociales, 2),
                SalaireNet = Math.Round(Math.Max(salaireNet, 0), 2), // Évite un net négatif
                DateCreation = DateTime.Now
            };

            _context.BulletinsPaie.Add(bulletin);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // VUE EMPLOYÉ : Liste ses propres bulletins
        // ==========================================
        public async Task<IActionResult> MesBulletins()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var mesBulletins = await _context.BulletinsPaie
                .Where(b => b.EmployeId == userId)
                .OrderByDescending(b => b.DateCreation)
                .ToListAsync();

            return View(mesBulletins);
        }
    }
}