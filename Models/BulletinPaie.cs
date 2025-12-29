using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rh.Models
{
    public class BulletinPaie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeId { get; set; }

        [ForeignKey("EmployeId")]
        public virtual Employe? Employe { get; set; }

        [Required]
        public string Periode { get; set; } = string.Empty; 

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalaireBrut { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Retenues { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalaireNet { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;
    }
}