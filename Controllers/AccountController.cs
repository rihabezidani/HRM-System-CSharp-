using Microsoft.AspNetCore.Mvc;
using Rh.Models;
using Rh.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Rh.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // 1. TENTATIVE CONNEXION RESPONSABLE RH (ADMIN)
            var admin = await _context.ResponsablesRH
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (admin != null && BCrypt.Net.BCrypt.Verify(model.Password, admin.MotDePasseHash))
            {
                // Comme ResponsableRH n'a pas de champ Nom, on utilise l'Email pour l'affichage
                CreerSession(admin.Id, admin.Email, "Admin", admin.Email);
                return RedirectToAction("Index", "Home");
            }

            // 2. TENTATIVE CONNEXION EMPLOYÉ
            var employe = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == model.Email && e.Password == model.Password);

            if (employe != null)
            {
                // On utilise le Nom et Prenom disponibles dans le modèle Employe
                string nomAffichage = $"{employe.Prenom} {employe.Nom}";
                CreerSession(employe.Id, employe.Email, employe.Role, nomAffichage);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email ou mot de passe incorrect";
            return View(model);
        }

        private void CreerSession(int id, string email, string role, string nomAffichage)
        {
            HttpContext.Session.SetInt32("UserId", id);
            HttpContext.Session.SetString("UserEmail", email);
            HttpContext.Session.SetString("UserRole", role);
            HttpContext.Session.SetString("UserName", nomAffichage);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}