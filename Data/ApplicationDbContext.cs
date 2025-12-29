using Microsoft.EntityFrameworkCore;
using Rh.Models;

namespace Rh.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ResponsableRH> ResponsablesRH { get; set; }

        public DbSet<Employe> Employees { get; set; }

        public DbSet<Conge> Conges { get; set; }
        public DbSet<BulletinPaie> BulletinsPaie { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResponsableRH>().ToTable("responsablesrh");

            modelBuilder.Entity<Employe>().ToTable("employees");

            modelBuilder.Entity<Conge>().ToTable("conges");

            modelBuilder.Entity<BulletinPaie>().ToTable("bulletinspaie");

           
    }
    }
}