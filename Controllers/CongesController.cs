using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rh.Data;
using Rh.Models;
using Microsoft.AspNetCore.Http;

namespace Rh.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class CongesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CongesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task SetNotificationCount()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                // On stocke le compte des notifications NON LUES
                var count = await _context.Notifications
                    .CountAsync(n => n.EmployeId == userId && !n.EstLu);
                ViewBag.NotifCount = count;
            }
        }

        // ==========================================
        // NOUVEAU : GÉNÉRATION DE RAPPORT (ADMIN)
        // ==========================================
        public async Task<IActionResult> Rapport()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return Forbid();

            await SetNotificationCount(); // <--- Pour le badge

            var congesApprouves = await _context.Conges
                .Include(c => c.Employe)
                .Where(c => c.Statut == "Approuvé")
                .OrderByDescending(c => c.DateDebut)
                .ToListAsync();

            var model = new RapportViewModel
            {
                CongesApprouves = congesApprouves,
                TotalEmployes = await _context.Employees.CountAsync(),
                TotalJoursConges = congesApprouves.Sum(c => (c.DateFin - c.DateDebut).Days + 1),
                DateGeneration = DateTime.Now
            };

            return View(model);
        }

        // ==========================================
        // PARTIE ADMIN : GESTION GLOBALE
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin") return RedirectToAction(nameof(MesConges));

            await SetNotificationCount();

            var conges = await _context.Conges
                .Include(c => c.Employe)
                .OrderByDescending(c => c.DateDemande)
                .ToListAsync();

            return View(conges);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Valider(int id, string statut)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();

            var conge = await _context.Conges.Include(c => c.Employe).FirstOrDefaultAsync(c => c.Id == id);

            if (conge != null && conge.Statut == "En attente")
            {
                if (statut == "Approuvé")
                {
                    int jours = (conge.DateFin - conge.DateDebut).Days + 1;

                    if (conge.TypeConge == "Annuel" && conge.Employe != null)
                    {
                        if (conge.Employe.JoursCongesRestants < jours)
                        {
                            TempData["AlertMessage"] = "Erreur : Solde insuffisant pour cet employé.";
                            return RedirectToAction(nameof(Index));
                        }
                        conge.Employe.JoursCongesRestants -= jours;
                        _context.Update(conge.Employe);
                    }
                    conge.Statut = "Approuvé";
                }
                else
                {
                    conge.Statut = "Refusé";
                }

                _context.Update(conge);

                // --- CRÉATION DE LA NOTIFICATION ---
                var notification = new Notification
                {
                    EmployeId = conge.EmployeId,
                    Message = $"Votre demande de congé du {conge.DateDebut.ToShortDateString()} a été {conge.Statut.ToLower()}.",
                    DateCreation = DateTime.Now,
                    EstLu = false
                };
                _context.Notifications.Add(notification);

                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = $"La demande a été {conge.Statut.ToLower()} et l'employé notifié.";
            }

           
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // PARTIE EMPLOYÉ : SES PROPRES CONGÉS
        // ==========================================
        public async Task<IActionResult> MesConges()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            await SetNotificationCount(); // <--- Affiche le badge rouge

            var mesConges = await _context.Conges
                .Where(c => c.EmployeId == userId)
                .OrderByDescending(c => c.DateDemande)
                .ToListAsync();

            return View(mesConges);
        }

        public async Task<IActionResult> Demander()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            await SetNotificationCount(); // <--- Affiche le badge rouge
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Demander(Conge conge)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            conge.EmployeId = userId.Value;
            conge.Statut = "En attente";
            conge.DateDemande = DateTime.Now;

            ModelState.Remove("Employe");

            if (ModelState.IsValid)
            {
                int joursDemandes = (conge.DateFin - conge.DateDebut).Days + 1;

                if (conge.DateDebut < DateTime.Today)
                {
                    ModelState.AddModelError("", "La date de début ne peut pas être dans le passé.");
                }
                else if (joursDemandes <= 0)
                {
                    ModelState.AddModelError("", "La date de fin doit être après la date de début.");
                }
                else
                {
                    var employe = await _context.Employees.FindAsync(conge.EmployeId);
                    if (employe != null && conge.TypeConge == "Annuel")
                    {
                        if (employe.JoursCongesRestants < joursDemandes)
                        {
                            TempData["AlertMessage"] = $"Solde insuffisant (Reste: {employe.JoursCongesRestants} j).";
                            await SetNotificationCount();
                            return View(conge);
                        }
                    }

                    _context.Add(conge);
                    await _context.SaveChangesAsync();

                    TempData["AlertMessage"] = "Votre demande de congé a été envoyée avec succès.";
                    return RedirectToAction(nameof(MesConges));
                }
            }

            await SetNotificationCount();
            return View(conge);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            var conge = await _context.Conges.Include(c => c.Employe).FirstOrDefaultAsync(c => c.Id == id);

            if (conge != null)
            {
                if (role == "Admin" || (conge.EmployeId == userId && conge.Statut == "En attente"))
                {
                    if (conge.Statut == "Approuvé" && conge.TypeConge == "Annuel" && conge.Employe != null)
                    {
                        int jours = (conge.DateFin - conge.DateDebut).Days + 1;
                        conge.Employe.JoursCongesRestants += jours;
                        _context.Update(conge.Employe);
                    }

                    _context.Conges.Remove(conge);
                    await _context.SaveChangesAsync();

                    TempData["AlertMessage"] = "La demande a été supprimée.";
                }
            }

            return RedirectToAction(role == "Admin" ? nameof(Index) : nameof(MesConges));
        }
    }
}