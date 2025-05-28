using blogic_crm_back.Models;
using Microsoft.EntityFrameworkCore;

namespace blogic_crm_back.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractUser> ContractUser { get; set; }
        public DbSet<Institution> Institutions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ContractUser>()
                .HasKey(cu => new { cu.ContractId, cu.UserId });

            modelBuilder.Entity<ContractUser>()
                .HasOne(cu => cu.Contract)
                .WithMany(c => c.Users)
                .HasForeignKey(cu => cu.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ContractUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.AdvisorContracts)
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - Role relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Contract - Institution relationship
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Institution)
                .WithMany(i => i.Contracts)
                .HasForeignKey(c => c.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed default roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Klient" },
                new Role { Id = 2, Name = "Poradce" },
                new Role { Id = 3, Name = "Admin" }
            );
        }
    }
}
