using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rh.Models
{
    [Table("responsablesrh")] 
    public class ResponsableRH
    {
        [Key]
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string MotDePasseHash { get; set; } = string.Empty;

        public bool EstActif { get; set; }

        public DateTime? DateCreation { get; set; }

        public string? MotDePasseHashChange { get; set; }
    }
}