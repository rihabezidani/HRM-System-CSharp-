using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rh.Data;
using Rh.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Rh.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sécurité et Compteurs de notifications pour le Layout
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");
            var action = filterContext.RouteData.Values["action"]?.ToString();

            if (userId == null)
            {
                filterContext.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // Calcul des notifications 
            if (role == "Admin")
            {
                // Le RH voit les demandes de congés "En attente"
                ViewBag.NotifCount = _context.Conges.Count(c => c.Statut == "En attente");
            }
            else
            {
                // L'employé voit ses congés qui ne sont plus "En attente" (Validés/Refusés)
                ViewBag.NotifCount = _context.Conges.Count(c => c.EmployeId == userId && c.Statut != "En attente");
            }

            if (role != "Admin" && action != "MonProfil")
            {
                filterContext.Result = new RedirectToActionResult("MonProfil", "Employees", null);
            }
            base.OnActionExecuting(filterContext);
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.ToListAsync());
        }

        public async Task<IActionResult> MonProfil()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var employe = await _context.Employees
                .Include(e => e.Responsable)
                .FirstOrDefaultAsync(m => m.Id == userId);

            if (employe == null) return NotFound();
            return View(employe);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employe employe)
        {
            employe.Password = "Collaborateur2025";
            employe.Role = "Employe";
            employe.ResponsableRHId = HttpContext.Session.GetInt32("UserId");

            if (employe.JoursCongesRestants <= 0) employe.JoursCongesRestants = 25;

            // Nettoyage et validation manuelle
            ModelState.Clear();
            if (string.IsNullOrEmpty(employe.Matricule)) ModelState.AddModelError("Matricule", "Obligatoire");
            if (string.IsNullOrEmpty(employe.Nom)) ModelState.AddModelError("Nom", "Obligatoire");
            if (string.IsNullOrEmpty(employe.Email)) ModelState.AddModelError("Email", "Obligatoire");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Employees.Add(employe);
                    await _context.SaveChangesAsync();

                    // Notification Flash pour le RH
                    TempData["AlertMessage"] = "Nouveau collaborateur créé avec succès !";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur : " + ex.Message);
                }
            }
            return View(employe);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var employe = await _context.Employees.FindAsync(id);
            if (employe == null) return NotFound();
            return View(employe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employe employe)
        {
            if (id != employe.Id) return NotFound();

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                try
                {
                    var dbEntry = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
                    if (dbEntry == null) return NotFound();

                    dbEntry.Matricule = employe.Matricule;
                    dbEntry.Nom = employe.Nom;
                    dbEntry.Prenom = employe.Prenom;
                    dbEntry.Email = employe.Email;
                    dbEntry.Poste = employe.Poste;
                    dbEntry.Departement = employe.Departement;
                    dbEntry.SalaireBase = employe.SalaireBase;
                    dbEntry.JoursCongesRestants = employe.JoursCongesRestants;

                    await _context.SaveChangesAsync();

                    TempData["AlertMessage"] = "Profil mis à jour.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employe.Id)) return NotFound();
                    else throw;
                }
            }
            return View(employe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var employe = await _context.Employees.FindAsync(id);
            if (employe != null)
            {
                var conges = _context.Conges.Where(c => c.EmployeId == id);
                _context.Conges.RemoveRange(conges);
                _context.Employees.Remove(employe);
                await _context.SaveChangesAsync();

                TempData["AlertMessage"] = "Collaborateur supprimé.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}