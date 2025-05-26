using blogic_crm_back.Models;
using Microsoft.EntityFrameworkCore;

namespace blogic_crm_back.Data
{
    // Kontext databáze – propojení EF modelů s tabulkami
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractAdvisor> ContractAdvisors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ContractAdvisor>()
                .HasKey(ca => new { ca.ContractId, ca.AdvisorId });

            modelBuilder.Entity<ContractAdvisor>()
                .HasOne(ca => ca.Contract)
                .WithMany(c => c.ContractAdvisors)
                .HasForeignKey(ca => ca.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ContractAdvisor>()
                .HasOne(ca => ca.Advisor)
                .WithMany(u => u.ContractAdvisors)
                .HasForeignKey(ca => ca.AdvisorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(u => u.ClientContracts)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Manager)
                .WithMany(u => u.ManagedContracts)
                .HasForeignKey(c => c.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "klient" },
                new Role { Id = 2, Name = "poradce" },
                new Role { Id = 3, Name = "admin" }
            );
        }
    }
}
