using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rh.Models
{
    public class Conge
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeId { get; set; }

        [ForeignKey("EmployeId")]
        public virtual Employe? Employe { get; set; }

        [Required]
        [Display(Name = "Date de début")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [Required]
        [Display(Name = "Date de fin")]
        [DataType(DataType.Date)]
        public DateTime DateFin { get; set; }

        [Required]
        [Display(Name = "Type de congé")]
        public string TypeConge { get; set; } = "Annuel";

        [Required]
        public string Statut { get; set; } = "En attente";

        
        [Display(Name = "Motif ou Commentaire")]
        public string? Motif { get; set; }

        [Display(Name = "Date de la demande")]
        public DateTime DateDemande { get; set; } = DateTime.Now;
    }
}