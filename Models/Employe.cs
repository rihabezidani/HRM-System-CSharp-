using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rh.Models
{
    public class Employe
    {
        public int Id { get; set; }

        [Required]
        public string Matricule { get; set; } = null!;

        [Required]
        public string Prenom { get; set; } = null!;

        [Required]
        public string Nom { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime? DateNaissance { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        public string Role { get; set; } = "Employe";

        [Required]
        public string? Telephone { get; set; } 
        public string? Adresse { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateEmbauche { get; set; }

        [Required]
        public string Poste { get; set; } = null!;

        [Required]
        public string Departement { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")] 
        public decimal SalaireBase { get; set; }

        public int JoursCongesRestants { get; set; } = 25;

        public bool? EstActif { get; set; } = true;

        public DateTime? DateArchivage { get; set; }
        public string? MotifArchivage { get; set; }

        

        public int? ResponsableRHId { get; set; }

        [ForeignKey("ResponsableRHId")]
        public virtual ResponsableRH? Responsable { get; set; }

        
        public virtual ICollection<Conge> Conges { get; set; } = new HashSet<Conge>();
        public virtual ICollection<BulletinPaie> BulletinsPaie { get; set; } = new HashSet<BulletinPaie>();
    }
}