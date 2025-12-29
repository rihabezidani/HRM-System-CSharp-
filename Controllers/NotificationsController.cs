using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rh.Data;

namespace Rh.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            // Récupère les notifications de l'utilisateur (les plus récentes en premier)
            var notifications = await _context.Notifications
                .Where(n => n.EmployeId == userId)
                .OrderByDescending(n => n.DateCreation)
                .ToListAsync();

            // Optionnel : Marquer toutes les notifications comme lues quand on ouvre la page
            var nonLues = notifications.Where(n => !n.EstLu).ToList();
            if (nonLues.Any())
            {
                nonLues.ForEach(n => n.EstLu = true);
                await _context.SaveChangesAsync();
            }

            return View(notifications);
        }
    }
}