using App.Core.Domain.Entities;
using App.Core.Domain.IdentityEntities;
using App.Core.Enums;
using App.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure.DbContext
{
    public static class ApplicationDbContextSeed
    {
        // =====================================================================
        // FIXED GUIDs
        // =====================================================================

        // Users
        private static readonly Guid AdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private static readonly Guid CharityUser1Id = Guid.Parse("00000000-0000-0000-0000-000000000011");
        private static readonly Guid CharityUser2Id = Guid.Parse("00000000-0000-0000-0000-000000000012");
        private static readonly Guid CharityUser3Id = Guid.Parse("00000000-0000-0000-0000-000000000013");
        private static readonly Guid DonorUser1Id = Guid.Parse("00000000-0000-0000-0000-000000000021");
        private static readonly Guid DonorUser2Id = Guid.Parse("00000000-0000-0000-0000-000000000022");
        private static readonly Guid DonorUser3Id = Guid.Parse("00000000-0000-0000-0000-000000000023");

        // Charities
        private static readonly Guid Charity1Id = Guid.Parse("00000000-0000-0000-0000-000000000101");
        private static readonly Guid Charity2Id = Guid.Parse("00000000-0000-0000-0000-000000000102");
        private static readonly Guid Charity3Id = Guid.Parse("00000000-0000-0000-0000-000000000103");

        // Donor Organizations
        private static readonly Guid Donor1Id = Guid.Parse("00000000-0000-0000-0000-000000000201");
        private static readonly Guid Donor2Id = Guid.Parse("00000000-0000-0000-0000-000000000202");
        private static readonly Guid Donor3Id = Guid.Parse("00000000-0000-0000-0000-000000000203");

        // Charity Needs
        private static readonly Guid Need1Id = Guid.Parse("00000000-0000-0000-0000-000000000301");
        private static readonly Guid Need2Id = Guid.Parse("00000000-0000-0000-0000-000000000302");
        private static readonly Guid Need3Id = Guid.Parse("00000000-0000-0000-0000-000000000303");
        private static readonly Guid Need4Id = Guid.Parse("00000000-0000-0000-0000-000000000304");
        private static readonly Guid Need5Id = Guid.Parse("00000000-0000-0000-0000-000000000305");
        private static readonly Guid Need6Id = Guid.Parse("00000000-0000-0000-0000-000000000306");
        private static readonly Guid Need7Id = Guid.Parse("00000000-0000-0000-0000-000000000307");

        // Offers
        private static readonly Guid Offer1Id = Guid.Parse("00000000-0000-0000-0000-000000000401");
        private static readonly Guid Offer2Id = Guid.Parse("00000000-0000-0000-0000-000000000402");
        private static readonly Guid Offer3Id = Guid.Parse("00000000-0000-0000-0000-000000000403");
        private static readonly Guid Offer4Id = Guid.Parse("00000000-0000-0000-0000-000000000404"); // pending — awaiting admin approval
        private static readonly Guid Offer5Id = Guid.Parse("00000000-0000-0000-0000-000000000405"); // expired
        private static readonly Guid Offer6Id = Guid.Parse("00000000-0000-0000-0000-000000000406"); // approved — for OfferApp4

        // Need Applications
        private static readonly Guid NeedApp1Id = Guid.Parse("00000000-0000-0000-0000-000000000501");
        private static readonly Guid NeedApp2Id = Guid.Parse("00000000-0000-0000-0000-000000000502");
        private static readonly Guid NeedApp3Id = Guid.Parse("00000000-0000-0000-0000-000000000503");
        private static readonly Guid NeedApp4Id = Guid.Parse("00000000-0000-0000-0000-000000000504");

        // Offer Applications
        private static readonly Guid OfferApp1Id = Guid.Parse("00000000-0000-0000-0000-000000000601");
        private static readonly Guid OfferApp2Id = Guid.Parse("00000000-0000-0000-0000-000000000602");
        private static readonly Guid OfferApp3Id = Guid.Parse("00000000-0000-0000-0000-000000000603");
        private static readonly Guid OfferApp4Id = Guid.Parse("00000000-0000-0000-0000-000000000604");

        // =====================================================================
        // STEP 1 — Roles
        // =====================================================================
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            string[] roles = { "Admin", "Charity", "DonorOrganization" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
        }

        // =====================================================================
        // STEP 2 — All seed data
        // =====================================================================
        public static async Task SeedAllAsync(IServiceProvider serviceProvider)
        {
            await SeedRolesAsync(serviceProvider);

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            await SeedUsersAsync(userManager);
            await SeedCharitiesAsync(db);
            await SeedDonorOrganizationsAsync(db);
            await SeedCharityNeedsAsync(db);
            await SeedOffersAsync(db);
            await SeedNeedApplicationsAsync(db);
            await SeedOfferApplicationsAsync(db);
        }

        // =====================================================================
        // USERS
        // =====================================================================
        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var users = new (ApplicationUser User, string Password, string Role)[]
            {
                (new ApplicationUser
                {
                    Id          = AdminUserId,
                    UserName    = "admin",
                    Email       = "admin@platform.com",
                    PhoneNumber = "01000000001",
                    Whatsapp    = "01000000001",
                    City        = "Cairo",
                    Governorate = "Cairo",
                    PostalCode  = "11511",
                    IsActive    = true,
                    CreatedAt   = DateTime.UtcNow,
                    UpdatedAt   = DateTime.UtcNow
                }, "Admin@1234", "Admin"),

                (new ApplicationUser
                {
                    Id          = CharityUser1Id,
                    UserName    = "charity1",
                    Email       = "charity1@example.com",
                    PhoneNumber = "01100000001",
                    Whatsapp    = "01100000001",
                    City        = "Cairo",
                    Governorate = "Cairo",
                    PostalCode  = "11511",
                    IsActive    = true,
                    CreatedAt   = DateTime.UtcNow,
                    UpdatedAt   = DateTime.UtcNow
                }, "Charity@1234", "Charity"),

                (new ApplicationUser
                {
                    Id          = CharityUser2Id,
                    UserName    = "charity2",
                    Email       = "charity2@example.com",
                    PhoneNumber = "01100000002",
                    Whatsapp    = "01100000002",
                    City        = "Alexandria",
                    Governorate = "Alexandria",
                    PostalCode  = "21500",
                    IsActive    = true,
                    CreatedAt   = DateTime.UtcNow,
                    UpdatedAt   = DateTime.UtcNow
                }, "Charity@1234", "Charity"),

                (new ApplicationUser
                {
                    Id          = CharityUser3Id,
                    UserName    = "charity3",
                    Email       = "charity3@example.com",
                    PhoneNumber = "01100000003",
                    Whatsapp    = "01100000003",
                    City        = "Giza",
                    Governorate = "Giza",
                    PostalCode  = "12511",
                    IsActive    = false,
                    CreatedAt   = DateTime.UtcNow,
                    UpdatedAt   = DateTime.UtcNow
                }, "Charity@1234", "Charity"),

                (new ApplicationUser
                {
                    Id          = DonorUser1Id,
                    UserName    = "donor1",
                    Email       = "donor1@example.com",
                    PhoneNumber = "01200000001",
                    Whatsapp    = "01200000001",
                    City        = "Cairo",
                    Governorate = "Cairo",
                    PostalCode  = "11511",
                    IsActive    = true,
                    CreatedAt   = DateTime.UtcNow,
                    UpdatedAt   = DateTime.UtcNow
                }, "Donor@1234", "DonorOrganization"),

                (new ApplicationUser
                {
                    Id          = DonorUser2Id,
                    UserName    = "donor2",
                    Email       = "donor2@example.com",
                    PhoneNumber = "01200000002",
                    Whatsapp    = "01200000002",
                    City        = "Alexandria",
                    Governorate = "Alexandria",
                    PostalCode  = "21500",
                    IsActive    = true,
                    CreatedAt   = DateTime.UtcNow,
                    UpdatedAt   = DateTime.UtcNow
                }, "Donor@1234", "DonorOrganization"),

                (new ApplicationUser
                {
                    Id          = DonorUser3Id,
                    UserName    = "donor3",
                    Email       = "donor3@example.com",
                    PhoneNumber = "01200000003",
                    Whatsapp    = "01200000003",
                    City        = "Giza",
                    Governorate = "Giza",
                    PostalCode  = "12511",
                    IsActive    = false,
                    CreatedAt   = DateTime.UtcNow,
                    UpdatedAt   = DateTime.UtcNow
                }, "Donor@1234", "DonorOrganization"),
            };

            foreach (var (user, password, role) in users)
            {
                if (await userManager.FindByIdAsync(user.Id.ToString()) != null) continue;

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, role);
            }
        }

        // =====================================================================
        // CHARITIES
        // =====================================================================
        private static async Task SeedCharitiesAsync(ApplicationDbContext db)
        {
            if (await db.Charities.AnyAsync()) return;

            await db.Charities.AddRangeAsync(
                // verified + active
                new Charity
                {
                    CharityId = Charity1Id,
                    UserId = CharityUser1Id,
                    CharityName = "Hope Foundation",
                    CharityDescription = "Providing food and shelter to families in need across Cairo.",
                    IsVerified = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                // unverified + active — pending admin approval
                new Charity
                {
                    CharityId = Charity2Id,
                    UserId = CharityUser2Id,
                    CharityName = "Nour Charity",
                    CharityDescription = "Supporting underprivileged children with education and clothing.",
                    IsVerified = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                // verified + inactive — deactivated by admin
                new Charity
                {
                    CharityId = Charity3Id,
                    UserId = CharityUser3Id,
                    CharityName = "Al-Rahma Organization",
                    CharityDescription = "Medical aid and rehabilitation for disabled individuals.",
                    IsVerified = true,
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            await db.SaveChangesAsync();
        }

        // =====================================================================
        // DONOR ORGANIZATIONS
        // =====================================================================
        private static async Task SeedDonorOrganizationsAsync(ApplicationDbContext db)
        {
            if (await db.DonorOrganizations.AnyAsync()) return;

            await db.DonorOrganizations.AddRangeAsync(
                // verified + active
                new DonorOrganization
                {
                    DonorOrganizationId = Donor1Id,
                    UserId = DonorUser1Id,
                    DonorOrganizationName = "EgyFood Corp",
                    IsVerified = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                // unverified + active — pending admin approval
                new DonorOrganization
                {
                    DonorOrganizationId = Donor2Id,
                    UserId = DonorUser2Id,
                    DonorOrganizationName = "ClothForAll",
                    IsVerified = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                // verified + inactive — deactivated by admin
                new DonorOrganization
                {
                    DonorOrganizationId = Donor3Id,
                    UserId = DonorUser3Id,
                    DonorOrganizationName = "MedSupply Egypt",
                    IsVerified = true,
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            await db.SaveChangesAsync();
        }

        // =====================================================================
        // CHARITY NEEDS
        // Statuses : Pending, Approved, Rejected, Fulfilled
        // Priorities: Urgent, High, Normal, Low
        // =====================================================================
        private static async Task SeedCharityNeedsAsync(ApplicationDbContext db)
        {
            if (await db.CharityNeeds.AnyAsync()) return;

            await db.CharityNeeds.AddRangeAsync(
                // Approved — food — Urgent
                new CharityNeed
                {
                    CharityNeedId = Need1Id,
                    CharityId = Charity1Id,
                    AdminId = AdminUserId,
                    Category = "food",
                    ProductName = "Rice Bags",
                    Quantity = 200,
                    Priority = CharityNeedPriority.Urgent,
                    Status = CharityNeedStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-9)
                },
                // Approved — clothing — High
                new CharityNeed
                {
                    CharityNeedId = Need2Id,
                    CharityId = Charity1Id,
                    AdminId = AdminUserId,
                    Category = "clothing",
                    ProductName = "Winter Jackets",
                    Quantity = 100,
                    Priority = CharityNeedPriority.High,
                    Status = CharityNeedStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    UpdatedAt = DateTime.UtcNow.AddDays(-7)
                },
                // Approved — medical — Normal
                new CharityNeed
                {
                    CharityNeedId = Need3Id,
                    CharityId = Charity1Id,
                    AdminId = AdminUserId,
                    Category = "medical",
                    ProductName = "Wheelchairs",
                    Quantity = 10,
                    Priority = CharityNeedPriority.Normal,
                    Status = CharityNeedStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                },
                // Approved — education — Low
                new CharityNeed
                {
                    CharityNeedId = Need4Id,
                    CharityId = Charity1Id,
                    AdminId = AdminUserId,
                    Category = "education",
                    ProductName = "School Backpacks",
                    Quantity = 50,
                    Priority = CharityNeedPriority.Low,
                    Status = CharityNeedStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-4)
                },
                // Pending — waiting admin approval
                new CharityNeed
                {
                    CharityNeedId = Need5Id,
                    CharityId = Charity1Id,
                    AdminId = null,
                    Category = "food",
                    ProductName = "Canned Goods",
                    Quantity = 300,
                    Priority = CharityNeedPriority.Urgent,
                    Status = CharityNeedStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                // Rejected — admin rejected
                new CharityNeed
                {
                    CharityNeedId = Need6Id,
                    CharityId = Charity1Id,
                    AdminId = AdminUserId,
                    Category = "clothing",
                    ProductName = "Summer Clothes",
                    Quantity = 80,
                    Priority = CharityNeedPriority.Normal,
                    Status = CharityNeedStatus.Rejected,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow.AddDays(-14)
                },
                // Fulfilled — donation completed
                new CharityNeed
                {
                    CharityNeedId = Need7Id,
                    CharityId = Charity1Id,
                    AdminId = AdminUserId,
                    Category = "medical",
                    ProductName = "First Aid Kits",
                    Quantity = 30,
                    Priority = CharityNeedPriority.High,
                    Status = CharityNeedStatus.Fulfilled,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-20)
                }
            );

            await db.SaveChangesAsync();
        }

        // =====================================================================
        // OFFERS
        // Statuses: Pending, Approved, Rejected, Expired, Fulfilled
        // =====================================================================
        private static async Task SeedOffersAsync(ApplicationDbContext db)
        {
            if (await db.Offers.AnyAsync()) return;

            await db.Offers.AddRangeAsync(
                // Approved — food
                new Offer
                {
                    OfferId = Offer1Id,
                    DonorOrganizationId = Donor1Id,
                    AdminId = AdminUserId,
                    Category = "food",
                    ProductName = "Pasta Boxes",
                    Quantity = 500,
                    ProductImage = null,
                    ExpiryDate = DateTime.UtcNow.AddMonths(3),
                    Status = OfferStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    UpdatedAt = DateTime.UtcNow.AddDays(-6)
                },
                // Approved — clothing
                new Offer
                {
                    OfferId = Offer2Id,
                    DonorOrganizationId = Donor1Id,
                    AdminId = AdminUserId,
                    Category = "clothing",
                    ProductName = "Children's Clothes",
                    Quantity = 200,
                    ProductImage = null,
                    ExpiryDate = DateTime.UtcNow.AddMonths(2),
                    Status = OfferStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-4)
                },
                // Approved — medical
                new Offer
                {
                    OfferId = Offer3Id,
                    DonorOrganizationId = Donor1Id,
                    AdminId = AdminUserId,
                    Category = "medical",
                    ProductName = "Surgical Masks",
                    Quantity = 1000,
                    ProductImage = null,
                    ExpiryDate = DateTime.UtcNow.AddMonths(6),
                    Status = OfferStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                // Approved — education (for OfferApp4)
                new Offer
                {
                    OfferId = Offer6Id,
                    DonorOrganizationId = Donor1Id,
                    AdminId = AdminUserId,
                    Category = "education",
                    ProductName = "Notebooks",
                    Quantity = 300,
                    ProductImage = null,
                    ExpiryDate = DateTime.UtcNow.AddMonths(4),
                    Status = OfferStatus.Approved,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                // Pending — waiting admin approval
                new Offer
                {
                    OfferId = Offer4Id,
                    DonorOrganizationId = Donor1Id,
                    AdminId = null,
                    Category = "education",
                    ProductName = "Textbooks",
                    Quantity = 150,
                    ProductImage = null,
                    ExpiryDate = DateTime.UtcNow.AddMonths(4),
                    Status = OfferStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                // Expired
                new Offer
                {
                    OfferId = Offer5Id,
                    DonorOrganizationId = Donor1Id,
                    AdminId = null,
                    Category = "food",
                    ProductName = "Bread Loaves",
                    Quantity = 100,
                    ProductImage = null,
                    ExpiryDate = DateTime.UtcNow.AddDays(-5),
                    Status = OfferStatus.Expired,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                }
            );

            await db.SaveChangesAsync();
        }

        // =====================================================================
        // NEED APPLICATIONS  (DonorOrganization → CharityNeed)
        // All reference Approved charity needs only
        // Statuses: Pending, Accepted, Rejected
        // =====================================================================
        private static async Task SeedNeedApplicationsAsync(ApplicationDbContext db)
        {
            if (await db.NeedApplications.AnyAsync()) return;

            await db.NeedApplications.AddRangeAsync(
                // Pending — waiting charity decision
                new NeedApplication
                {
                    NeedApplicationId = NeedApp1Id,
                    DonorOrganizationId = Donor1Id,
                    CharityNeedId = Need1Id,  // Approved ✅
                    Status = ApplicationStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3)
                },
                // Accepted — charity accepted donor
                new NeedApplication
                {
                    NeedApplicationId = NeedApp2Id,
                    DonorOrganizationId = Donor1Id,
                    CharityNeedId = Need2Id,  // Approved ✅
                    Status = ApplicationStatus.Accepted,
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                },
                // Rejected — charity rejected donor
                new NeedApplication
                {
                    NeedApplicationId = NeedApp3Id,
                    DonorOrganizationId = Donor1Id,
                    CharityNeedId = Need3Id,  // Approved ✅
                    Status = ApplicationStatus.Rejected,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-4)
                },
                // Pending — waiting charity decision
                new NeedApplication
                {
                    NeedApplicationId = NeedApp4Id,
                    DonorOrganizationId = Donor1Id,
                    CharityNeedId = Need4Id,  // Approved ✅
                    Status = ApplicationStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                }
            );

            await db.SaveChangesAsync();
        }

        // =====================================================================
        // OFFER APPLICATIONS  (Charity → Offer)
        // All reference Approved offers only
        // Statuses: Pending, Accepted, Rejected
        // =====================================================================
        private static async Task SeedOfferApplicationsAsync(ApplicationDbContext db)
        {
            if (await db.OfferApplications.AnyAsync()) return;

            await db.OfferApplications.AddRangeAsync(
                // Pending — waiting donor decision
                new OfferApplication
                {
                    OfferApplicationId = OfferApp1Id,
                    OfferId = Offer1Id,   // Approved ✅
                    CharityId = Charity1Id,
                    Status = ApplicationStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3)
                },
                // Accepted — donor accepted charity
                new OfferApplication
                {
                    OfferApplicationId = OfferApp2Id,
                    OfferId = Offer2Id,   // Approved ✅
                    CharityId = Charity1Id,
                    Status = ApplicationStatus.Accepted,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-4)
                },
                // Rejected — donor rejected charity
                new OfferApplication
                {
                    OfferApplicationId = OfferApp3Id,
                    OfferId = Offer3Id,   // Approved ✅
                    CharityId = Charity1Id,
                    Status = ApplicationStatus.Rejected,
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3)
                },
                // Pending — waiting donor decision
                new OfferApplication
                {
                    OfferApplicationId = OfferApp4Id,
                    OfferId = Offer6Id,   // Approved ✅ — new offer, no duplicate constraint
                    CharityId = Charity1Id,
                    Status = ApplicationStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                }
            );

            await db.SaveChangesAsync();
        }
    }
}