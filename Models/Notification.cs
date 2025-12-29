using System.ComponentModel.DataAnnotations;

namespace Rh.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string Message { get; set; } = default!; 

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public bool EstLu { get; set; } = false;

        public int EmployeId { get; set; }

       
        public virtual Employe? Employe { get; set; }
    }
}