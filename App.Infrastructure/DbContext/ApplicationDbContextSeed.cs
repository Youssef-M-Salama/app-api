using App.Core.Domain.Entities;
using App.Core.Domain.IdentityEntities;
using App.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure.DbContext
{
    public static class ApplicationDbContextSeed
    {
        // ================= USERS =================
        private static readonly Guid AdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        private static readonly Guid CharityUser1Id = Guid.Parse("00000000-0000-0000-0000-000000000011");
        private static readonly Guid CharityUser2Id = Guid.Parse("00000000-0000-0000-0000-000000000012");
        private static readonly Guid CharityUser3Id = Guid.Parse("00000000-0000-0000-0000-000000000013");

        private static readonly Guid DonorUser1Id = Guid.Parse("00000000-0000-0000-0000-000000000021");
        private static readonly Guid DonorUser2Id = Guid.Parse("00000000-0000-0000-0000-000000000022");
        private static readonly Guid DonorUser3Id = Guid.Parse("00000000-0000-0000-0000-000000000023");

        // ================= ENTITIES =================
        private static readonly Guid Charity1Id = Guid.Parse("00000000-0000-0000-0000-000000000101");
        private static readonly Guid Charity2Id = Guid.Parse("00000000-0000-0000-0000-000000000102");
        private static readonly Guid Charity3Id = Guid.Parse("00000000-0000-0000-0000-000000000103");

        private static readonly Guid Donor1Id = Guid.Parse("00000000-0000-0000-0000-000000000201");
        private static readonly Guid Donor2Id = Guid.Parse("00000000-0000-0000-0000-000000000202");
        private static readonly Guid Donor3Id = Guid.Parse("00000000-0000-0000-0000-000000000203");

        private static readonly Guid Need1Id = Guid.Parse("00000000-0000-0000-0000-000000000301");
        private static readonly Guid Need2Id = Guid.Parse("00000000-0000-0000-0000-000000000302");
        private static readonly Guid Need3Id = Guid.Parse("00000000-0000-0000-0000-000000000303");
        private static readonly Guid Need4Id = Guid.Parse("00000000-0000-0000-0000-000000000304");

        private static readonly Guid Offer1Id = Guid.Parse("00000000-0000-0000-0000-000000000401");
        private static readonly Guid Offer2Id = Guid.Parse("00000000-0000-0000-0000-000000000402");
        private static readonly Guid Offer3Id = Guid.Parse("00000000-0000-0000-0000-000000000403");
        private static readonly Guid Offer4Id = Guid.Parse("00000000-0000-0000-0000-000000000404");

        private static readonly Guid NeedApp1Id = Guid.Parse("00000000-0000-0000-0000-000000000501");
        private static readonly Guid NeedApp2Id = Guid.Parse("00000000-0000-0000-0000-000000000502");

        private static readonly Guid OfferApp1Id = Guid.Parse("00000000-0000-0000-0000-000000000601");
        private static readonly Guid OfferApp2Id = Guid.Parse("00000000-0000-0000-0000-000000000602");

        // =========================================================
        // ROLES
        // =========================================================
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            string[] roles = { "Admin", "Charity", "DonorOrganization" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new ApplicationRole { Name = role });
            }
        }

        // =========================================================
        // CLEAR DATABASE (INCLUDING USERS SAFE)
        // =========================================================
        public static async Task ClearDatabaseAsync(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            // 1️⃣ Delete relational data FIRST
            await db.OfferApplications.ExecuteDeleteAsync();
            await db.NeedApplications.ExecuteDeleteAsync();
            await db.Offers.ExecuteDeleteAsync();
            await db.CharityNeeds.ExecuteDeleteAsync();
            await db.DonorOrganizations.ExecuteDeleteAsync();
            await db.Charities.ExecuteDeleteAsync();

            // 2️⃣ Delete users safely
            var users = await db.Users.ToListAsync();

            foreach (var user in users)
            {
                await userManager.DeleteAsync(user);
            }
        }

        // =========================================================
        // MAIN ENTRY
        // =========================================================
        public static async Task SeedAllAsync(IServiceProvider serviceProvider)
        {
            await SeedRolesAsync(serviceProvider);

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 🔥 ENABLE THIS ONLY WHEN YOU WANT FULL RESET
            await ClearDatabaseAsync(db, userManager);

            await SeedUsersAsync(userManager);
            await SeedCharitiesAsync(db);
            await SeedDonorsAsync(db);
            await SeedNeedsAsync(db);
            await SeedOffersAsync(db);
            await SeedNeedApplicationsAsync(db);
            await SeedOfferApplicationsAsync(db);
        }

        // =========================================================
        // USERS (HARDCODED PASSWORDS)
        // =========================================================
        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var users = new[]
            {
                new { Id = AdminUserId, Username = "admin", Email = "admin@test.com", Password = "Admin@1234", Role = "Admin", Phone = "01011111111", Whatsapp = "01011111111" },
                new { Id = CharityUser1Id, Username = "charity1", Email = "charity@test.com", Password = "Charity@1234", Role = "Charity", Phone = "01022222222", Whatsapp = "01022222222" },
                new { Id = CharityUser2Id, Username = "charity2", Email = "charity2@test.com", Password = "Charity@1234", Role = "Charity", Phone = "01033333333", Whatsapp = "01033333333" },
                new { Id = CharityUser3Id, Username = "charity3", Email = "charity3@test.com", Password = "Charity@1234", Role = "Charity", Phone = "01044444444", Whatsapp = "01044444444" },
                new { Id = DonorUser1Id, Username = "donor1", Email = "donor@test.com", Password = "Donor@1234", Role = "DonorOrganization", Phone = "01055555555", Whatsapp = "01055555555" },
                new { Id = DonorUser2Id, Username = "donor2", Email = "donor2@test.com", Password = "Donor@1234", Role = "DonorOrganization", Phone = "01066666666", Whatsapp = "01066666666" },
                new { Id = DonorUser3Id, Username = "donor3", Email = "donor3@test.com", Password = "Donor@1234", Role = "DonorOrganization", Phone = "01077777777", Whatsapp = "01077777777" }
            };

            foreach (var u in users)
            {
                if (await userManager.FindByIdAsync(u.Id.ToString()) != null)
                    continue;

                var user = new ApplicationUser
                {
                    Id = u.Id,
                    UserName = u.Username,
                    Email = u.Email,
                    EmailConfirmed = true,
                    IsActive = true,
                    ImageUrl = null,
                    PhoneNumber = u.Phone,
                    Whatsapp = u.Whatsapp,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(user, u.Password);

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, u.Role);
            }
        }

        // =========================================================
        // CHARITIES
        // =========================================================
        private static async Task SeedCharitiesAsync(ApplicationDbContext db)
        {
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity1Id))
            {
                await db.Charities.AddAsync(new Charity { CharityId = Charity1Id, UserId = CharityUser1Id, CharityName = "جمعية الأمل", CharityDescription = "جمعية رائدة في مساعدة الأسر المحتاجة وتوفير الرعاية الصحية.", IsActive = true, IsVerified = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity2Id))
            {
                await db.Charities.AddAsync(new Charity { CharityId = Charity2Id, UserId = CharityUser2Id, CharityName = "رسالة نور", CharityDescription = "نهتم بتعليم الأطفال وتنمية مهارات الشباب في المناطق النائية.", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity3Id))
            {
                await db.Charities.AddAsync(new Charity { CharityId = Charity3Id, UserId = CharityUser3Id, CharityName = "مؤسسة التكافل", CharityDescription = "توزيع المساعدات الغذائية والكساء على الفقراء والمساكين.", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            }

            await db.SaveChangesAsync();
        }

        // =========================================================
        // DONORS
        // =========================================================
        private static async Task SeedDonorsAsync(ApplicationDbContext db)
        {
            if (!await db.DonorOrganizations.AnyAsync(d => d.DonorOrganizationId == Donor1Id))
            {
                await db.DonorOrganizations.AddAsync(new DonorOrganization { DonorOrganizationId = Donor1Id, UserId = DonorUser1Id, DonorOrganizationName = "شركة الخير", DonorOrganizationDescription = "شركة رائدة تخصص جزءاً من أرباحها لدعم المبادرات الخيرية.", IsActive = true, IsVerified = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            }
            if (!await db.DonorOrganizations.AnyAsync(d => d.DonorOrganizationId == Donor2Id))
            {
                await db.DonorOrganizations.AddAsync(new DonorOrganization { DonorOrganizationId = Donor2Id, UserId = DonorUser2Id, DonorOrganizationName = "مجموعة العطاء", DonorOrganizationDescription = "مجموعة تجارية تسعى لنشر الخير ودعم المحتاجين في كل مكان.", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            }
            if (!await db.DonorOrganizations.AnyAsync(d => d.DonorOrganizationId == Donor3Id))
            {
                await db.DonorOrganizations.AddAsync(new DonorOrganization { DonorOrganizationId = Donor3Id, UserId = DonorUser3Id, DonorOrganizationName = "مؤسسة الإحسان", DonorOrganizationDescription = "نهدف إلى تقديم الدعم اللوجستي والمادي للجمعيات الخيرية.", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            }

            await db.SaveChangesAsync();
        }

        // =========================================================
        // NEEDS
        // =========================================================
        private static async Task SeedNeedsAsync(ApplicationDbContext db)
        {
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need1Id))
                await db.CharityNeeds.AddAsync(new CharityNeed { CharityNeedId = Need1Id, CharityId = Charity1Id, ProductName = "أرز", Quantity = 100, Status = CharityNeedStatus.Approved, CreatedAt = DateTime.UtcNow, ProductImage = null });

            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need2Id))
                await db.CharityNeeds.AddAsync(new CharityNeed { CharityNeedId = Need2Id, CharityId = Charity1Id, ProductName = "زيت طعام", Quantity = 50, Status = CharityNeedStatus.Pending, CreatedAt = DateTime.UtcNow, ProductImage = null });

            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need3Id))
                await db.CharityNeeds.AddAsync(new CharityNeed { CharityNeedId = Need3Id, CharityId = Charity2Id, ProductName = "ملابس شتوية", Quantity = 200, Category = ProductCategory.Clothing, Status = CharityNeedStatus.Approved, CreatedAt = DateTime.UtcNow, ProductImage = null });

            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need4Id))
                await db.CharityNeeds.AddAsync(new CharityNeed { CharityNeedId = Need4Id, CharityId = Charity3Id, ProductName = "بطاطين", Quantity = 150, Category = ProductCategory.Other, Status = CharityNeedStatus.Approved, CreatedAt = DateTime.UtcNow, ProductImage = null });

            await db.SaveChangesAsync();
        }

        // =========================================================
        // OFFERS
        // =========================================================
        private static async Task SeedOffersAsync(ApplicationDbContext db)
        {
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer1Id))
                await db.Offers.AddAsync(new Offer { OfferId = Offer1Id, DonorOrganizationId = Donor1Id, ProductName = "مكرونة", Quantity = 200, Status = OfferStatus.Approved, CreatedAt = DateTime.UtcNow, ProductImage = null });

            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer2Id))
                await db.Offers.AddAsync(new Offer { OfferId = Offer2Id, DonorOrganizationId = Donor1Id, ProductName = "سكر", Quantity = 100, Status = OfferStatus.Approved, CreatedAt = DateTime.UtcNow, ProductImage = null });

            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer3Id))
                await db.Offers.AddAsync(new Offer { OfferId = Offer3Id, DonorOrganizationId = Donor2Id, ProductName = "أدوية", Quantity = 50, Category = ProductCategory.Medical, Status = OfferStatus.Pending, CreatedAt = DateTime.UtcNow, ProductImage = null });

            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer4Id))
                await db.Offers.AddAsync(new Offer { OfferId = Offer4Id, DonorOrganizationId = Donor3Id, ProductName = "كتب مدرسية", Quantity = 500, Category = ProductCategory.Education, Status = OfferStatus.Approved, CreatedAt = DateTime.UtcNow, ProductImage = null });

            await db.SaveChangesAsync();
        }

        // =========================================================
        // NEED APPLICATIONS
        // =========================================================
        private static async Task SeedNeedApplicationsAsync(ApplicationDbContext db)
        {
            if (!await db.NeedApplications.AnyAsync(a => a.NeedApplicationId == NeedApp1Id))
                await db.NeedApplications.AddAsync(new NeedApplication { NeedApplicationId = NeedApp1Id, DonorOrganizationId = Donor1Id, CharityNeedId = Need1Id, Status = ApplicationStatus.Pending, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });

            if (!await db.NeedApplications.AnyAsync(a => a.NeedApplicationId == NeedApp2Id))
                await db.NeedApplications.AddAsync(new NeedApplication { NeedApplicationId = NeedApp2Id, DonorOrganizationId = Donor2Id, CharityNeedId = Need3Id, Status = ApplicationStatus.Accepted, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });

            await db.SaveChangesAsync();
        }

        // =========================================================
        // OFFER APPLICATIONS
        // =========================================================
        private static async Task SeedOfferApplicationsAsync(ApplicationDbContext db)
        {
            if (!await db.OfferApplications.AnyAsync(a => a.OfferApplicationId == OfferApp1Id))
                await db.OfferApplications.AddAsync(new OfferApplication { OfferApplicationId = OfferApp1Id, CharityId = Charity1Id, OfferId = Offer1Id, Status = ApplicationStatus.Pending, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });

            if (!await db.OfferApplications.AnyAsync(a => a.OfferApplicationId == OfferApp2Id))
                await db.OfferApplications.AddAsync(new OfferApplication { OfferApplicationId = OfferApp2Id, CharityId = Charity2Id, OfferId = Offer4Id, Status = ApplicationStatus.Accepted, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });

            await db.SaveChangesAsync();
        }
    }
}