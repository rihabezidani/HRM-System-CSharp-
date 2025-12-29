using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rh.Data;
using Rh.Models;
using System.Diagnostics;

namespace Rh.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. VÉRIFICATION DE LA SESSION
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var today = DateTime.Today;

            // 2. LOGIQUE SELON LE RÔLE
            if (userRole == "Admin")
            {
                // --- STATISTIQUES POUR L'ADMINISTRATEUR ---
                ViewBag.TotalEmployees = await _context.Employees.CountAsync();

                // Nombre de personnes absentes AUJOURD'HUI (Congés validés)
                ViewBag.AbsentsAujourdhui = await _context.Conges
                    .CountAsync(c => today >= c.DateDebut && today <= c.DateFin && c.Statut == "Approuvé");

                // Nombre de demandes en attente de validation
                ViewBag.EnAttente = await _context.Conges
                    .CountAsync(c => c.Statut == "En attente");

                // Dernières demandes reçues (pour affichage rapide)
                ViewBag.RecentRequests = await _context.Conges
                    .Include(c => c.Employe)
                    .OrderByDescending(c => c.DateDemande)
                    .Take(5)
                    .ToListAsync();
            }
            else
            {
                // --- STATISTIQUES POUR L'EMPLOYÉ ---
                var employe = await _context.Employees.FindAsync(userId);

                // Son solde personnel
                ViewBag.MonSolde = employe?.JoursCongesRestants ?? 0;

                // Ses propres demandes en attente
                ViewBag.MesDemandesEnAttente = await _context.Conges
                    .CountAsync(c => c.EmployeId == userId && c.Statut == "En attente");

                // Sa prochaine absence prévue
                ViewBag.ProchainConge = await _context.Conges
                    .Where(c => c.EmployeId == userId && c.DateDebut >= today && c.Statut == "Approuvé")
                    .OrderBy(c => c.DateDebut)
                    .FirstOrDefaultAsync();
            }

            return View();
        }

        public IActionResult Privacy()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}