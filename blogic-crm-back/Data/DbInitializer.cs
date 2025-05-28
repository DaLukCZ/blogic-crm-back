using blogic_crm_back.Data;
using blogic_crm_back.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class DbInitializer
{
    public static void Seed(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        context.Database.Migrate();

        var adminRole = context.Roles.First(r => r.Name == "Admin");
        var advisorRole = context.Roles.First(r => r.Name == "Poradce");
        var clientRole = context.Roles.First(r => r.Name == "Klient");

        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new() {
                    Username = "admin", RoleId = adminRole.Id
                },
                new() {
                    Username = "DaLuk", RoleId = adminRole.Id
                },
                new() {
                    Username = "martin.poradce", RoleId = advisorRole.Id,
                    FirstName = "Martin", LastName = "Svoboda", Email = "martin.poradce@example.com",
                    CountryCode = "+420", Number = "728111222", SSN = "8111161520"
                },
                new() {
                    Username = "lenka.poradce", RoleId = advisorRole.Id,
                    FirstName = "Lenka", LastName = "Králová", Email = "lenka.poradce@example.com",
                    CountryCode = "+420", Number = "604223344", SSN = "6559019896"
                },
                new() {
                    Username = "ondrej.poradce", RoleId = advisorRole.Id,
                    FirstName = "Ondřej", LastName = "Dvořák", Email = "ondrej.poradce@example.com",
                    CountryCode = "+420", Number = "739334455", SSN = "7103234875"
                },
                new() {
                    Username = "eva.klient", RoleId = clientRole.Id,
                    FirstName = "Eva", LastName = "Veselá", Email = "eva.klient@example.com",
                    CountryCode = "+420", Number = "733112233", SSN = "7462126441"
                },
                new() {
                    Username = "tomas.klient", RoleId = clientRole.Id,
                    FirstName = "Tomáš", LastName = "Malý", Email = "tomas.klient@example.com",
                    CountryCode = "+420", Number = "702998877", SSN = "8403187991"
                },
                new() {
                    Username = "klara.klient", RoleId = clientRole.Id,
                    FirstName = "Klára", LastName = "Němcová", Email = "klara.klient@example.com",
                    CountryCode = "+420", Number = "601889900", SSN = "7556253991"
                }
            };

            foreach (var user in users)
            {
                user.PasswordHash = passwordHasher.HashPassword(user, "Heslo123");
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            var advisors = users.Where(u => u.RoleId == advisorRole.Id).ToList();
            var clients = users.Where(u => u.RoleId == clientRole.Id).ToList();

            var institutions = new List<Institution>
            {
                new() { Name = "ČSOB" },
                new() { Name = "Aegon" },
                new() { Name = "AXA" },
                new() { Name = "Kooperativa" },
                new() { Name = "Česká spořitelna" },
                new() { Name = "Uniqa" }
            };

            context.Institutions.AddRange(institutions);
            context.SaveChanges();

            var contracts = new List<Contract>
            {
                // Aktivní smlouvy
                new() { ReferenceNumber = "SML-0001", InstitutionId = institutions[0].Id, DateSigned = DateTime.UtcNow.AddDays(-60), DateValidFrom = DateTime.UtcNow.AddDays(-55), DateValidTo = DateTime.UtcNow.AddMonths(2) },
                new() { ReferenceNumber = "SML-0002", InstitutionId = institutions[1].Id, DateSigned = DateTime.UtcNow.AddDays(-30), DateValidFrom = DateTime.UtcNow.AddDays(-28), DateValidTo = DateTime.UtcNow.AddMonths(3) },
                new() { ReferenceNumber = "SML-0003", InstitutionId = institutions[2].Id, DateSigned = DateTime.UtcNow.AddDays(-15), DateValidFrom = DateTime.UtcNow.AddDays(-10), DateValidTo = DateTime.UtcNow.AddMonths(1) },

                // Nadcházející smlouvy
                new() { ReferenceNumber = "SML-0004", InstitutionId = institutions[3].Id, DateSigned = DateTime.UtcNow.AddDays(-1), DateValidFrom = DateTime.UtcNow.AddDays(2), DateValidTo = DateTime.UtcNow.AddMonths(4) },
                new() { ReferenceNumber = "SML-0005", InstitutionId = institutions[4].Id, DateSigned = DateTime.UtcNow, DateValidFrom = DateTime.UtcNow.AddDays(5), DateValidTo = DateTime.UtcNow.AddMonths(6) },
                new() { ReferenceNumber = "SML-0006", InstitutionId = institutions[5].Id, DateSigned = DateTime.UtcNow.AddDays(-2), DateValidFrom = DateTime.UtcNow.AddDays(3), DateValidTo = DateTime.UtcNow.AddMonths(5) },

                // Ukončené smlouvy
                new() { ReferenceNumber = "SML-0007", InstitutionId = institutions[0].Id, DateSigned = DateTime.UtcNow.AddMonths(-12), DateValidFrom = DateTime.UtcNow.AddMonths(-12), DateValidTo = DateTime.UtcNow.AddMonths(-6) },
                new() { ReferenceNumber = "SML-0008", InstitutionId = institutions[1].Id, DateSigned = DateTime.UtcNow.AddMonths(-10), DateValidFrom = DateTime.UtcNow.AddMonths(-10), DateValidTo = DateTime.UtcNow.AddMonths(-2) },
                new() { ReferenceNumber = "SML-0009", InstitutionId = institutions[1].Id, DateSigned = DateTime.UtcNow.AddMonths(-7), DateValidFrom = DateTime.UtcNow.AddMonths(-7), DateValidTo = DateTime.UtcNow.AddMonths(-1) },
                new() { ReferenceNumber = "SML-0010", InstitutionId = institutions[1].Id, DateSigned = DateTime.UtcNow.AddMonths(-5), DateValidFrom = DateTime.UtcNow.AddMonths(-5), DateValidTo = DateTime.UtcNow.AddDays(-20) },
                new() { ReferenceNumber = "SML-0011", InstitutionId = institutions[2].Id, DateSigned = DateTime.UtcNow.AddMonths(-8), DateValidFrom = DateTime.UtcNow.AddMonths(-7), DateValidTo = DateTime.UtcNow.AddDays(-10) },
                new() { ReferenceNumber = "SML-0012", InstitutionId = institutions[4].Id, DateSigned = DateTime.UtcNow.AddMonths(-6), DateValidFrom = DateTime.UtcNow.AddMonths(-6), DateValidTo = DateTime.UtcNow.AddMonths(-3) }
            };


            context.Contracts.AddRange(contracts);
            context.SaveChanges();

            var contractUsers = new List<ContractUser>
            {
                new() { ContractId = contracts[0].Id, UserId = clients[0].Id },
                new() { ContractId = contracts[0].Id, UserId = advisors[0].Id },

                new() { ContractId = contracts[1].Id, UserId = clients[1].Id },
                new() { ContractId = contracts[1].Id, UserId = advisors[1].Id },

                new() { ContractId = contracts[2].Id, UserId = clients[2].Id },
                new() { ContractId = contracts[2].Id, UserId = advisors[2].Id },

                new() { ContractId = contracts[3].Id, UserId = clients[0].Id },
                new() { ContractId = contracts[3].Id, UserId = advisors[1].Id },

                new() { ContractId = contracts[4].Id, UserId = clients[1].Id },
                new() { ContractId = contracts[4].Id, UserId = advisors[2].Id },

                new() { ContractId = contracts[5].Id, UserId = clients[2].Id },
                new() { ContractId = contracts[5].Id, UserId = advisors[0].Id },
                new() { ContractId = contracts[5].Id, UserId = advisors[1].Id },

                new() { ContractId = contracts[6].Id, UserId = clients[0].Id },
                new() { ContractId = contracts[6].Id, UserId = advisors[0].Id },

                new() { ContractId = contracts[7].Id, UserId = clients[1].Id },
                new() { ContractId = contracts[7].Id, UserId = advisors[1].Id },

                new() { ContractId = contracts[8].Id, UserId = clients[1].Id },
                new() { ContractId = contracts[8].Id, UserId = advisors[1].Id },

                new() { ContractId = contracts[9].Id, UserId = clients[1].Id },
                new() { ContractId = contracts[9].Id, UserId = advisors[2].Id },

                new() { ContractId = contracts[10].Id, UserId = clients[2].Id },
                new() { ContractId = contracts[10].Id, UserId = advisors[0].Id },

                new() { ContractId = contracts[11].Id, UserId = clients[0].Id },
                new() { ContractId = contracts[11].Id, UserId = advisors[2].Id }
            };


            context.ContractUser.AddRange(contractUsers);
            context.SaveChanges();
        }
    }
}
