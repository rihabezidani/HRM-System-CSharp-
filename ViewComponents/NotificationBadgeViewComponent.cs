using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rh.Models; 
using Rh.Data; 

namespace Rh.ViewComponents
{
    public class NotificationBadgeViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context; 

        public NotificationBadgeViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            
            List<Conge> notifs = new List<Conge>();

            if (role == "Admin")
            {
                
                notifs = await _context.Conges
                    .Include(c => c.Employe)
                    .Where(c => c.Statut == "En attente")
                    .OrderByDescending(c => c.DateDemande)
                    .ToListAsync();
            }
            else if (userId != null)
            {
                notifs = await _context.Conges
                    .Where(c => c.EmployeId == userId && c.Statut != "En attente")
                    .OrderByDescending(c => c.DateDemande)
                    .ToListAsync();
            }

            return View(notifs);
        }
    }
}