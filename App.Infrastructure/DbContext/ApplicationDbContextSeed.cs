using App.Core.Domain.Entities;
using App.Core.Domain.Enums;
using App.Core.Domain.IdentityEntities;
using App.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure.DbContext
{
    public static class ApplicationDbContextSeed
    {
        // ================= GUID CONSTANTS =================
        private static readonly Guid AdminUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        // Charity Users & Entities
        private static readonly Guid Charity1UserId = Guid.Parse("634d9e91-3a5c-5208-88a1-409f784f3571");
        private static readonly Guid Charity1Id = Guid.Parse("5858cec1-30f3-5951-8f02-332f9a4d593b");
        private static readonly Guid Charity2UserId = Guid.Parse("4a2848a7-c154-5d3f-9a5c-48c868c35730");
        private static readonly Guid Charity2Id = Guid.Parse("3848f02a-ff67-5a72-8eb3-9ca550ee36cd");
        private static readonly Guid Charity3UserId = Guid.Parse("a30fb48b-7ee3-55e8-98cb-1f916a0fcf7a");
        private static readonly Guid Charity3Id = Guid.Parse("1dcf0a84-28a9-5d03-90b5-4abd703bd071");
        private static readonly Guid Charity4UserId = Guid.Parse("89bb650a-25f6-5ffa-83ad-47590ce64c0e");
        private static readonly Guid Charity4Id = Guid.Parse("406bccbd-9040-5486-b329-625456c25260");
        private static readonly Guid Charity5UserId = Guid.Parse("5ebe24a6-80da-5fa9-8616-36c4ba941f3a");
        private static readonly Guid Charity5Id = Guid.Parse("c5b05833-7f40-5988-9b82-94055a52b855");
        private static readonly Guid Charity6UserId = Guid.Parse("b7e6d7b3-1152-515a-a032-ab782a7b5136");
        private static readonly Guid Charity6Id = Guid.Parse("269d5119-1125-535e-9dff-c8db78ee3335");
        private static readonly Guid Charity7UserId = Guid.Parse("0f0b5d26-f36a-5470-ba02-1cb831339e0c");
        private static readonly Guid Charity7Id = Guid.Parse("d88f8190-9743-57b6-ac48-eac091d9cb7f");
        private static readonly Guid Charity8UserId = Guid.Parse("6e8b1840-2789-5f2e-abbe-ec4ca72459d5");
        private static readonly Guid Charity8Id = Guid.Parse("68aabac5-a36b-59ef-9c27-b75d51e56750");
        private static readonly Guid Charity9UserId = Guid.Parse("6edb36ac-8c3c-5fd1-9545-320536e4f8d1");
        private static readonly Guid Charity9Id = Guid.Parse("169d510a-c38e-54f7-a9d9-164f97fa1a53");
        private static readonly Guid Charity10UserId = Guid.Parse("f4a2ea1a-7fa9-5667-a395-a8176403d4da");
        private static readonly Guid Charity10Id = Guid.Parse("37cbf781-d916-5a8b-9798-b073e0686581");
        private static readonly Guid Charity11UserId = Guid.Parse("030024f3-2687-5c89-9f06-a02709b67c8c");
        private static readonly Guid Charity11Id = Guid.Parse("5c88050e-d35e-52b6-9610-a35caca235d0");
        private static readonly Guid Charity12UserId = Guid.Parse("e0bd082d-4c31-589f-be80-bccc262aadae");
        private static readonly Guid Charity12Id = Guid.Parse("a45c87c0-a076-5c65-877a-f0a1c234bab3");
        private static readonly Guid Charity13UserId = Guid.Parse("f6e7eeb2-4329-504b-b6ff-1088ddd0dfa4");
        private static readonly Guid Charity13Id = Guid.Parse("667c3218-46c8-504f-95e0-b4a59cd3ed76");
        private static readonly Guid Charity_pendingUserId = Guid.Parse("dd6de8db-a964-5ad6-a74d-92e67a050ab9");
        private static readonly Guid Charity_pendingId = Guid.Parse("a8adcdd2-139f-514f-be6d-17820d32802d");
        private static readonly Guid Charity_inreviewUserId = Guid.Parse("54d8ab5f-c49c-565c-b5af-9568f776e228");
        private static readonly Guid Charity_inreviewId = Guid.Parse("44ecb279-b2c8-56ac-9e58-0d32ecaad082");
        private static readonly Guid Charity_rejectedUserId = Guid.Parse("1d0ee717-1a7e-5876-9371-4a26303b029a");
        private static readonly Guid Charity_rejectedId = Guid.Parse("0b70b44b-30f6-53a8-866b-e0b9ec03d540");

        // Donor Users & Entities
        private static readonly Guid Donor1UserId = Guid.Parse("7bbdb82b-ba16-53ee-8f0e-8c8abab17e51");
        private static readonly Guid Donor1Id = Guid.Parse("898b0bf9-79b7-5ea4-831b-6e010fc590cf");
        private static readonly Guid Donor2UserId = Guid.Parse("86f8209a-99eb-5eb3-9f5d-afe1096711c3");
        private static readonly Guid Donor2Id = Guid.Parse("282b6e92-62a2-5202-9778-032d08ade0da");
        private static readonly Guid Donor3UserId = Guid.Parse("6388ac8c-fb8a-5bc1-9eb1-4f78e1686f98");
        private static readonly Guid Donor3Id = Guid.Parse("64a9bf0e-e5a1-53d1-be63-f3e2ab1b08e2");
        private static readonly Guid Donor4UserId = Guid.Parse("dca49853-4455-51f7-9f67-363c5cb2613b");
        private static readonly Guid Donor4Id = Guid.Parse("decf91d1-c18d-5350-bd7a-93575471d94d");
        private static readonly Guid Donor5UserId = Guid.Parse("bace7f9f-eb45-527c-92f9-5ab93409efab");
        private static readonly Guid Donor5Id = Guid.Parse("8ae0ee37-5ced-5565-844e-4a529d4de9bd");
        private static readonly Guid Donor6UserId = Guid.Parse("77b33fbb-19e8-5ae8-9b5d-7c0b51e7c15d");
        private static readonly Guid Donor6Id = Guid.Parse("30d60487-de98-5b23-8a60-30ddcfc71f2f");
        private static readonly Guid Donor7UserId = Guid.Parse("3ff7b44c-0539-5b77-b27e-0f7d4319626a");
        private static readonly Guid Donor7Id = Guid.Parse("d4b91ea5-4402-5c5a-867e-93e1947b11b5");
        private static readonly Guid Donor_pendingUserId = Guid.Parse("e047a547-4f3b-5492-8f3a-93518a4e62ed");
        private static readonly Guid Donor_pendingId = Guid.Parse("88925918-1a51-5661-b73c-c401df6500b0");
        private static readonly Guid Donor_inreviewUserId = Guid.Parse("c92f29f1-46cb-5a8b-885d-32d692ffa63d");
        private static readonly Guid Donor_inreviewId = Guid.Parse("733d18d9-00a6-5936-9f42-624f35872e91");
        private static readonly Guid Donor_rejectedUserId = Guid.Parse("a1af6b66-e4a1-5192-93d4-a7e26339cd8f");
        private static readonly Guid Donor_rejectedId = Guid.Parse("373ce004-68f2-5901-bb09-fefda7407375");

        // Needs Guids
        private static readonly Guid Need1Id = Guid.Parse("df86946d-9958-5d1d-beec-6462853e533f");
        private static readonly Guid Need2Id = Guid.Parse("9d2f3f8f-14ed-545d-b957-6fffe831946e");
        private static readonly Guid Need3Id = Guid.Parse("763f5269-0772-5e3d-bfb7-e33ef4db39db");
        private static readonly Guid Need4Id = Guid.Parse("fc5aeeed-b6db-5446-8d9f-48c7eab4edf1");
        private static readonly Guid Need5Id = Guid.Parse("57bfc2f7-3764-5a9d-8a66-00863bd6af37");
        private static readonly Guid Need6Id = Guid.Parse("0a208b6c-8acc-5bfb-b15e-2f69a8424033");
        private static readonly Guid Need7Id = Guid.Parse("b9ddca61-d1bf-5728-a3dc-416d8282e7db");
        private static readonly Guid Need8Id = Guid.Parse("54c263a0-fc25-55e2-b66e-7e03c35b8a9f");
        private static readonly Guid Need9Id = Guid.Parse("4e621faa-6e90-524a-bfa6-d24ac1e9f575");
        private static readonly Guid Need10Id = Guid.Parse("3231b8d2-b4fb-52da-b3e0-f4714e18d39a");
        private static readonly Guid Need11Id = Guid.Parse("31e65727-d374-585a-bf17-2e7d261cb409");
        private static readonly Guid Need12Id = Guid.Parse("cf8cdc32-7549-593a-8980-6c9904c3ee50");
        private static readonly Guid Need13Id = Guid.Parse("141e39f9-2b4d-59d3-b7ae-a46fc5a634e5");
        private static readonly Guid Need14Id = Guid.Parse("5e49f498-9384-5a84-9d41-4bf4ec05f6b1");
        private static readonly Guid Need15Id = Guid.Parse("53753512-47fc-5afa-961c-4175a7d6daf2");
        private static readonly Guid Need16Id = Guid.Parse("c6a5c07a-943d-5be6-9583-ea8ad9aff77d");
        private static readonly Guid Need17Id = Guid.Parse("3b41ec60-03be-5cec-8585-3c18b013f9a4");
        private static readonly Guid Need18Id = Guid.Parse("f800277b-0837-58d2-883f-528e0f03a1c9");
        private static readonly Guid Need19Id = Guid.Parse("638457d7-202e-5d83-97ba-4d3375edc528");
        private static readonly Guid Need20Id = Guid.Parse("4c8f191e-fb88-5ca3-878b-677cfd9b0369");
        private static readonly Guid Need21Id = Guid.Parse("657befdc-e24a-5d3a-ae64-328ce4c2e701");
        private static readonly Guid Need22Id = Guid.Parse("1e27ae3f-4e6e-582a-a5bc-b283d03ccb65");
        private static readonly Guid Need23Id = Guid.Parse("81a81ec5-0094-5e0e-af9e-e74ad4b37726");
        private static readonly Guid Need24Id = Guid.Parse("aa8ef470-22a6-57d1-9e84-938daefef3bc");
        private static readonly Guid Need25Id = Guid.Parse("fb9e88ea-23f3-5a7e-8921-90db821caa98");
        private static readonly Guid Need26Id = Guid.Parse("b651c59e-e291-5803-b10f-38cddb9078e0");
        private static readonly Guid Need27Id = Guid.Parse("f20cb629-7f40-555c-840c-61d8693588d0");
        private static readonly Guid Need28Id = Guid.Parse("1978f86a-f392-5203-ac57-df8af60ffcff");

        // Offers Guids
        private static readonly Guid Offer1Id = Guid.Parse("b0be249d-19ce-5801-929d-0e77d38935a8");
        private static readonly Guid Offer2Id = Guid.Parse("26c56383-6a5a-53bf-9e81-296dfbd7c062");
        private static readonly Guid Offer3Id = Guid.Parse("24c99f31-2e2a-5261-a023-347e37cf2817");
        private static readonly Guid Offer4Id = Guid.Parse("20f95a22-b204-53c8-8444-a6a57d3bac1f");
        private static readonly Guid Offer5Id = Guid.Parse("f273d266-e0d7-5668-8ed4-bffcf20dd2e2");
        private static readonly Guid Offer6Id = Guid.Parse("3acf7c9f-ffd9-574b-a5c0-68d57767d478");
        private static readonly Guid Offer7Id = Guid.Parse("4d5717ed-a5da-5d09-84d8-bcf3f7178f30");
        private static readonly Guid Offer8Id = Guid.Parse("89e9193a-8ab3-5618-a009-2f8a67baf5a7");
        private static readonly Guid Offer9Id = Guid.Parse("94eaab6b-566d-5403-9778-f4e3a2934d7b");
        private static readonly Guid Offer10Id = Guid.Parse("12a54a1c-a915-5724-a5f0-a418da544776");
        private static readonly Guid Offer11Id = Guid.Parse("8089a1f2-3438-58a7-9998-2ea5715c538c");
        private static readonly Guid Offer12Id = Guid.Parse("bc481921-262e-51a8-b3ff-9614fc8cadad");
        private static readonly Guid Offer13Id = Guid.Parse("44386774-d51b-5394-b2db-89aa7ff8de87");
        private static readonly Guid Offer14Id = Guid.Parse("5af50ec4-f551-5882-8991-2c57ae347dfc");
        private static readonly Guid Offer15Id = Guid.Parse("e297878d-5782-51aa-9054-2c7e6963e175");
        private static readonly Guid Offer16Id = Guid.Parse("51743e40-6f77-57bb-801c-2be5c9b2a7dc");

        // Application Guids
        private static readonly Guid NeedApp1Id = Guid.Parse("00000000-0000-0000-0000-000000000501");
        private static readonly Guid NeedApp2Id = Guid.Parse("00000000-0000-0000-0000-000000000502");
        private static readonly Guid OfferApp1Id = Guid.Parse("00000000-0000-0000-0000-000000000601");
        private static readonly Guid OfferApp2Id = Guid.Parse("00000000-0000-0000-0000-000000000602");
        private static readonly Guid NeedApp3Id = Guid.Parse("00000000-0000-0000-0000-000000000503");

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
        // CLEAR DATABASE
        // =========================================================
        public static async Task ClearDatabaseAsync(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            await db.OfferApplications.ExecuteDeleteAsync();
            await db.NeedApplications.ExecuteDeleteAsync();
            await db.Offers.ExecuteDeleteAsync();
            await db.CharityNeeds.ExecuteDeleteAsync();
            await db.DonorOrganizations.ExecuteDeleteAsync();
            await db.Charities.ExecuteDeleteAsync();

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

            // Always clear relational data & users for a fresh seed conforming to Excel & images schema
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
        // USERS (SEED METHOD)
        // =========================================================
        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            // Coordinates are real, sourced from Wikipedia/official geographic data for each city.
            // Format: (Latitude, Longitude)
            //
            // Cairo / حلوان          → Helwan district, Cairo:            29.8453, 31.3333
            // الجيزة / 6 أكتوبر     → 6th of October City:               29.9830, 30.9670
            // الإسكندرية / برج العرب → Borg El Arab, Alexandria Gov.:     30.8489, 29.6117
            // القليوبية / بنها       → Banha, Qalyubiyya Gov.:            30.4608, 31.1875
            // الشرقية / الزقازيق    → Zagazig, Sharqia Gov.:             30.5670, 31.5000
            // الدقهلية / المنصورة  → Mansoura, Dakahlia Gov.:            31.0370, 31.3810
            // الغربية / طنطا        → Tanta, Gharbia Gov.:               30.7833, 31.0000
            // المنوفية / شبين الكوم → Shebin El Kom, Monufia Gov.:       30.5586, 31.0100
            // البحيرة / دمنهور      → Damanhur, Beheira Gov.:            31.0340, 30.4680
            // كفر الشيخ / كفر الشيخ → Kafr El Sheikh city:               31.1073, 30.9388
            // دمياط / دمياط الجديدة → New Damietta, Damietta Gov.:       31.4368, 31.6670
            // بورسعيد / بورفؤاد     → Port Fouad, Port Said Gov.:        31.2580, 32.3250
            // الإسماعيلية / فايد    → Fayed, Ismailia Gov.:              30.3130, 32.2940
            // السويس / عتاقة        → Ataqah, Suez Gov.:                 29.9700, 32.5430
            // البحر الأحمر / الغردقة → Hurghada, Red Sea Gov.:           27.2579, 33.8116
            // الفيوم / سنورس        → Sinnuris, Fayoum Gov.:             29.4130, 30.8690
            // بني سويف / الواسطى   → El Wasta, Beni Suef Gov.:          29.0010, 31.2020
            // المنيا / ملوي         → Mallawi, Minya Gov.:               27.7310, 30.8410
            // أسيوط / ديروط        → Dairut, Asyut Gov.:                27.5570, 30.8070
            // سوهاج / طهطا          → Tahta, Sohag Gov.:                 26.7710, 31.5030
            // قنا / نجع حمادي      → Nag Hammadi, Qena Gov.:            26.0490, 32.2490
            // الأقصر / إسنا         → Esna, Luxor Gov.:                  25.2930, 32.5520
            // أسوان / كوم أمبو      → Kom Ombo, Aswan Gov.:              24.4790, 32.9450
            // الوادي الجديد / الداخلة → Dakhla Oasis, New Valley Gov.:  25.4890, 29.0030
            // مطروح / العلمين       → El Alamein, Matrouh Gov.:          30.8400, 28.9560
            // جنوب سيناء / شرم الشيخ → Sharm El Sheikh, S. Sinai Gov.: 27.9150, 34.3300

            var users = new[]
            {
                new { Id = AdminUserId,             Username = "admin",             Email = "admin@test.com",             Password = "Admin@1234",   Role = "Admin",              Phone = "+201011111111", ImageUrl = (string)null,                                       VerifyMyAccount = false, Governorate = "القاهرة",       City = "القاهرة",        Latitude = 30.0444,  Longitude = 31.2357 },

                // ── Charities ──────────────────────────────────────────────────────────────────
                new { Id = Charity1UserId,          Username = "charity1",          Email = "charity1@test.com",          Password = "Charity@1234", Role = "Charity",            Phone = "+201000000001", ImageUrl = "seed/alnoor_charity_logo.jpeg",                     VerifyMyAccount = true,  Governorate = "القاهرة",       City = "حلوان",          Latitude = 29.8453,  Longitude = 31.3333 },
                new { Id = Charity2UserId,          Username = "charity2",          Email = "charity2@test.com",          Password = "Charity@1234", Role = "Charity",            Phone = "+201000000002", ImageUrl = (string)null,                                       VerifyMyAccount = true,  Governorate = "الجيزة",        City = "6 أكتوبر",       Latitude = 29.9830,  Longitude = 30.9670 },
                new { Id = Charity3UserId,          Username = "charity3",          Email = "charity3@test.com",          Password = "Charity@1234", Role = "Charity",            Phone = "+201000000003", ImageUrl = "seed/life_association_logo.jpeg",                   VerifyMyAccount = true,  Governorate = "الإسكندرية",    City = "برج العرب",      Latitude = 30.8489,  Longitude = 29.6117 },
                new { Id = Charity4UserId,          Username = "charity4",          Email = "charity4@test.com",          Password = "Charity@1234", Role = "Charity",            Phone = "+201000000004", ImageUrl = "seed/mercy_association_logo.jpeg",                  VerifyMyAccount = true,  Governorate = "القليوبية",     City = "بنها",           Latitude = 30.4608,  Longitude = 31.1875 },
                new { Id = Charity5UserId,          Username = "charity5",          Email = "charity5@test.com",          Password = "Charity@1234", Role = "Charity",            Phone = "+201000000005", ImageUrl = "seed/goodness_association_logo.jpeg",               VerifyMyAccount = true,  Governorate = "الشرقية",       City = "الزقازيق",       Latitude = 30.5670,  Longitude = 31.5000 },
                new { Id = Charity6UserId,          Username = "charity6",          Email = "charity6@test.com",          Password = "Charity@1234", Role = "Charity",            Phone = "+201000000006", ImageUrl = "seed/future_association_logo.jpeg",                 VerifyMyAccount = true,  Governorate = "الدقهلية",      City = "المنصورة",       Latitude = 31.0370,  Longitude = 31.3810 },
                new { Id = Charity7UserId,          Username = "charity7",          Email = "charity7@test.com",          Password = "Charity@1234", Role = "Charity",            Phone = "+201000000007", ImageUrl = "seed/cooperation_association_logo.jpeg",            VerifyMyAccount = true,  Governorate = "الغربية",       City = "طنطا",           Latitude = 30.7833,  Longitude = 31.0000 },
                new { Id = Charity8UserId,          Username = "charity8",          Email = "charity8@test.com",          Password = "Charity@1234", Role = "Charity",            Phone = "+201000000008", ImageUrl = "seed/faith_association_logo.jpeg",                  VerifyMyAccount = true,  Governorate = "المنوفية",      City = "شبين الكوم",     Latitude = 30.5586,  Longitude = 31.0100 },
                new { Id = Charity9UserId,          Username = "charity9",          Email = "charity9@test.com",          Password = "Charity@1234", Role = "Charity",            Phone = "+201000000009", ImageUrl = "seed/white_hands_association_logo.jpeg",            VerifyMyAccount = true,  Governorate = "البحيرة",       City = "دمنهور",         Latitude = 31.0340,  Longitude = 30.4680 },
                new { Id = Charity10UserId,         Username = "charity10",         Email = "charity10@test.com",         Password = "Charity@1234", Role = "Charity",            Phone = "+201000000010", ImageUrl = "seed/light_of_life_association_logo.jpeg",          VerifyMyAccount = true,  Governorate = "كفر الشيخ",     City = "كفر الشيخ",      Latitude = 31.1073,  Longitude = 30.9388 },
                new { Id = Charity11UserId,         Username = "charity11",         Email = "charity11@test.com",         Password = "Charity@1234", Role = "Charity",            Phone = "+201000000011", ImageUrl = "seed/dignity_association_logo.jpeg",                VerifyMyAccount = true,  Governorate = "دمياط",         City = "دمياط الجديدة",  Latitude = 31.4368,  Longitude = 31.6670 },
                new { Id = Charity12UserId,         Username = "charity12",         Email = "charity12@test.com",         Password = "Charity@1234", Role = "Charity",            Phone = "+201000000012", ImageUrl = "seed/happiness_association_logo.jpeg",              VerifyMyAccount = true,  Governorate = "بورسعيد",       City = "بورفؤاد",        Latitude = 31.2580,  Longitude = 32.3250 },
                new { Id = Charity13UserId,         Username = "charity13",         Email = "charity13@test.com",         Password = "Charity@1234", Role = "Charity",            Phone = "+201000000013", ImageUrl = "seed/new_hope_logo.jpeg",                           VerifyMyAccount = true,  Governorate = "الإسماعيلية",   City = "فايد",           Latitude = 30.3130,  Longitude = 32.2940 },
                new { Id = Charity_pendingUserId,   Username = "charity_pending",   Email = "charity_pending@test.com",   Password = "Charity@1234", Role = "Charity",            Phone = "+201099000001", ImageUrl = (string)null,                                       VerifyMyAccount = false, Governorate = "السويس",        City = "عتاقة",          Latitude = 29.9700,  Longitude = 32.5430 },
                new { Id = Charity_inreviewUserId,  Username = "charity_inreview",  Email = "charity_inreview@test.com",  Password = "Charity@1234", Role = "Charity",            Phone = "+201099000002", ImageUrl = (string)null,                                       VerifyMyAccount = true,  Governorate = "البحر الأحمر",  City = "الغردقة",        Latitude = 27.2579,  Longitude = 33.8116 },
                new { Id = Charity_rejectedUserId,  Username = "charity_rejected",  Email = "charity_rejected@test.com",  Password = "Charity@1234", Role = "Charity",            Phone = "+201099000003", ImageUrl = (string)null,                                       VerifyMyAccount = true,  Governorate = "الفيوم",        City = "سنورس",          Latitude = 29.4130,  Longitude = 30.8690 },

                // ── Donors ─────────────────────────────────────────────────────────────────────
                new { Id = Donor1UserId, Username = "donor1", Email = "donor1@test.com", Password = "Donor@1234", Role = "DonorOrganization", Phone = "+201100000001", ImageUrl = "seed/hand_in_hand_logo.jpeg", VerifyMyAccount = true,
      Governorate = "القاهرة", City = "حلوان",
      Latitude = 29.8489, Longitude = 31.3367 },
                new { Id = Donor2UserId,            Username = "donor2",            Email = "donor2@test.com",            Password = "Donor@1234",   Role = "DonorOrganization",  Phone = "+201100000002", ImageUrl = "seed/always_good_logo.jpeg",                        VerifyMyAccount = true,  Governorate = "المنيا",        City = "ملوي",           Latitude = 27.7310,  Longitude = 30.8410 },
                new { Id = Donor3UserId,            Username = "donor3",            Email = "donor3@test.com",            Password = "Donor@1234",   Role = "DonorOrganization",  Phone = "+201100000003", ImageUrl = "seed/sanad_foundation_logo.jpeg",                   VerifyMyAccount = true,  Governorate = "أسيوط",         City = "ديروط",          Latitude = 27.5570,  Longitude = 30.8070 },
                new { Id = Donor4UserId,            Username = "donor4",            Email = "donor4@test.com",            Password = "Donor@1234",   Role = "DonorOrganization",  Phone = "+201100000004", ImageUrl = "seed/giving_without_limits_logo.jpeg",              VerifyMyAccount = true,  Governorate = "سوهاج",         City = "طهطا",           Latitude = 26.7710,  Longitude = 31.5030 },
                new { Id = Donor5UserId,            Username = "donor5",            Email = "donor5@test.com",            Password = "Donor@1234",   Role = "DonorOrganization",  Phone = "+201100000005", ImageUrl = "seed/smile_of_hope_logo.jpeg",                      VerifyMyAccount = true,  Governorate = "قنا",           City = "نجع حمادي",      Latitude = 26.0490,  Longitude = 32.2490 },
                new { Id = Donor6UserId,            Username = "donor6",            Email = "donor6@test.com",            Password = "Donor@1234",   Role = "DonorOrganization",  Phone = "+201100000006", ImageUrl = "seed/peace_foundation_logo.jpeg",                   VerifyMyAccount = true,  Governorate = "الأقصر",        City = "إسنا",           Latitude = 25.2930,  Longitude = 32.5520 },
                new { Id = Donor7UserId,            Username = "donor7",            Email = "donor7@test.com",            Password = "Donor@1234",   Role = "DonorOrganization",  Phone = "+201100000007", ImageUrl = "seed/light_of_life_logo.jpeg",                      VerifyMyAccount = true,  Governorate = "أسوان",         City = "كوم أمبو",       Latitude = 24.4790,  Longitude = 32.9450 },
                new { Id = Donor_pendingUserId,     Username = "donor_pending",     Email = "donor_pending@test.com",     Password = "Donor@1234",   Role = "DonorOrganization",  Phone = "+201199000001", ImageUrl = (string)null,                                       VerifyMyAccount = false, Governorate = "الوادي الجديد", City = "الداخلة",        Latitude = 25.4890,  Longitude = 29.0030 },
                new { Id = Donor_inreviewUserId,    Username = "donor_inreview",    Email = "donor_inreview@test.com",    Password = "Donor@1234",   Role = "DonorOrganization",  Phone = "+201199000002", ImageUrl = (string)null,                                       VerifyMyAccount = true,  Governorate = "مطروح",         City = "العلمين",        Latitude = 30.8400,  Longitude = 28.9560 },
                new { Id = Donor_rejectedUserId,    Username = "donor_rejected",    Email = "donor_rejected@test.com",    Password = "Donor@1234",   Role = "DonorOrganization",  Phone = "+201199000003", ImageUrl = (string)null,                                       VerifyMyAccount = true,  Governorate = "جنوب سيناء",    City = "شرم الشيخ",      Latitude = 27.9150,  Longitude = 34.3300 },
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
                    VerifyMyAccount = u.VerifyMyAccount,
                    ImageUrl = u.ImageUrl,
                    PhoneNumber = u.Phone,
                    Whatsapp = u.Phone,
                    City = u.City,
                    Governorate = u.Governorate,
                    PostalCode = "12345",
                    Latitude = u.Latitude,
                    Longitude = u.Longitude,
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
            var dummyPdf = "seed/dummy.pdf";

            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity1Id))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity1Id,
                    UserId = Charity1UserId,
                    CharityName = "جمعية النور الخيرية",
                    CharityDescription = "جمعية خيرية تعمل على دعم الأسر الأكثر احتياجاً من خلال توفير الغذاء والملابس وتنظيم حملات موسمية للمساعدات الإنسانية داخل المجتمع. كما تعمل الجمعية على تنظيم حملات موسمية لدعم الأسر الأكثر احتياجاً، وتسعى إلى توفير بيئة أفضل للمجتمع من خلال المبادرات التطوعية والتكافل الاجتماعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    RegistrationNumber = "CH-2026-001",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity2Id))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity2Id,
                    UserId = Charity2UserId,
                    CharityName = "جمعية الأمل",
                    CharityDescription = "جمعية تهدف إلى تحسين حياة الأسر الفقيرة عبر تقديم مساعدات غذائية وطبية ودعم حالات الطوارئ بشكل مستمر. كما تعمل الجمعية على تنظيم حملات موسمية لدعم الأسر الأكثر احتياجاً، وتسعى إلى توفير بيئة أفضل للمجتمع من خلال المبادرات التطوعية والتكافل الاجتماعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    RegistrationNumber = "CH-2026-002",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity3Id))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity3Id,
                    UserId = Charity3UserId,
                    CharityName = "جمعية الحياة",
                    CharityDescription = "جمعية إنسانية تهدف إلى تقديم الدعم للفئات الضعيفة من خلال برامج غذائية وتعليمية ومبادرات مجتمعية. كما تعمل الجمعية على تنظيم حملات موسمية لدعم الأسر الأكثر احتياجاً، وتسعى إلى توفير بيئة أفضل للمجتمع من خلال المبادرات التطوعية والتكافل الاجتماعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    RegistrationNumber = "CH-2026-003",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity4Id))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity4Id,
                    UserId = Charity4UserId,
                    CharityName = "جمعية الرحمة",
                    CharityDescription = "جمعية تطوعية تركز على توزيع السلال الغذائية للأسر المحتاجة مع تنفيذ حملات خيرية موسمية. كما تعمل الجمعية على تنظيم حملات موسمية لدعم الأسر الأكثر احتياجاً، وتسعى إلى توفير بيئة أفضل للمجتمع من خلال المبادرات التطوعية والتكافل الاجتماعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    RegistrationNumber = "CH-2026-004",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity5Id))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity5Id,
                    UserId = Charity5UserId,
                    CharityName = "جمعية الخير",
                    CharityDescription = "جمعية تقدم خدمات دعم اجتماعي وإنساني تشمل الغذاء والملبس والمساعدات العاجلة للأسر محدودة الدخل. كما تعمل الجمعية على تنظيم حملات موسمية لدعم الأسر الأكثر احتياجاً، وتسعى إلى توفير بيئة أفضل للمجتمع من خلال المبادرات التطوعية والتكافل الاجتماعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    RegistrationNumber = "CH-2026-005",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity6Id))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity6Id,
                    UserId = Charity6UserId,
                    CharityName = "جمعية المستقبل",
                    CharityDescription = "جمعية تهدف إلى بناء مستقبل أفضل للأسر المحتاجة عبر توفير الدعم الغذائي والتعليمي والمبادرات التنموية. كما تعمل الجمعية على تنظيم حملات موسمية لدعم الأسر الأكثر احتياجاً، وتسعى إلى توفير بيئة أفضل للمجتمع من خلال المبادرات التطوعية والتكافل الاجتماعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    RegistrationNumber = "CH-2026-006",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity7Id))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity7Id,
                    UserId = Charity7UserId,
                    CharityName = "جمعية التعاون",
                    CharityDescription = "جمعية تعتمد على العمل الجماعي لتقديم مساعدات غذائية وطبية وتعزيز روح التكافل بين أفراد المجتمع. كما تعمل الجمعية على تنظيم حملات موسمية لدعم الأسر الأكثر احتياجاً، وتسعى إلى توفير بيئة أفضل للمجتمع من خلال المبادرات التطوعية والتكافل الاجتماعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    RegistrationNumber = "CH-2026-007",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity8Id))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity8Id,
                    UserId = Charity8UserId,
                    CharityName = "جمعية الإيمان",
                    CharityDescription = "جمعية خيرية تقدم مساعدات غذائية وملابس للفقراء مع التركيز على دعم الأسر في الأوقات الصعبة. كما تعمل الجمعية على تنظيم حملات موسمية لدعم الأسر الأكثر احتياجاً، وتسعى إلى توفير بيئة أفضل للمجتمع من خلال المبادرات التطوعية والتكافل الاجتماعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    RegistrationNumber = "CH-2026-008",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity9Id))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity9Id,
                    UserId = Charity9UserId,
                    CharityName = "جمعية الأيادي البيضاء",
                    CharityDescription = "جمعية تطوعية تنفذ حملات خيرية متنوعة تشمل توزيع الطعام والملابس ودعم الفئات المحتاجة. كما تعمل الجمعية على تنظيم حملات موسمية لدعم الأسر الأكثر احتياجاً، وتسعى إلى توفير بيئة أفضل للمجتمع من خلال المبادرات التطوعية والتكافل الاجتماعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    RegistrationNumber = "CH-2026-009",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity10Id))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity10Id,
                    UserId = Charity10UserId,
                    CharityName = "جمعية نور الحياة",
                    CharityDescription = "جمعية تهدف إلى تحسين جودة حياة المحتاجين من خلال دعم غذائي وطبي ومشاريع تنموية صغيرة. كما تعمل الجمعية على تنظيم حملات موسمية لدعم الأسر الأكثر احتياجاً، وتسعى إلى توفير بيئة أفضل للمجتمع من خلال المبادرات التطوعية والتكافل الاجتماعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    RegistrationNumber = "CH-2026-010",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity11Id))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity11Id,
                    UserId = Charity11UserId,
                    CharityName = "جمعية الكرامة",
                    CharityDescription = "جمعية تقدم مساعدات إنسانية عاجلة للأسر الفقيرة مع التركيز على توفير الاحتياجات الأساسية. كما تعمل الجمعية على تنظيم حملات موسمية لدعم الأسر الأكثر احتياجاً، وتسعى إلى توفير بيئة أفضل للمجتمع من خلال المبادرات التطوعية والتكافل الاجتماعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    RegistrationNumber = "CH-2026-011",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity12Id))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity12Id,
                    UserId = Charity12UserId,
                    CharityName = "جمعية السعادة",
                    CharityDescription = "جمعية خيرية تسعى لنشر السعادة بين الأسر المحتاجة عبر تقديم سلال غذائية ومساعدات متنوعة. كما تعمل الجمعية على تنظيم حملات موسمية لدعم الأسر الأكثر احتياجاً، وتسعى إلى توفير بيئة أفضل للمجتمع من خلال المبادرات التطوعية والتكافل الاجتماعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    RegistrationNumber = "CH-2026-012",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity13Id))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity13Id,
                    UserId = Charity13UserId,
                    CharityName = "جمعية الأمل الجديد",
                    CharityDescription = "جمعية تهدف إلى دعم الأسر الفقيرة وبناء فرص أفضل لهم من خلال برامج مساعدات مستمرة. كما تعمل الجمعية على تنظيم حملات موسمية لدعم الأسر الأكثر احتياجاً، وتسعى إلى توفير بيئة أفضل للمجتمع من خلال المبادرات التطوعية والتكافل الاجتماعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    RegistrationNumber = "CH-2026-013",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity_pendingId))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity_pendingId,
                    UserId = Charity_pendingUserId,
                    CharityName = "جمعية الأمل والتحدي",
                    CharityDescription = "جمعية تحت المراجعة لمساعدات متحدي الإعاقة.",
                    IsActive = true,
                    VerificationState = VerificationState.Pending,
                    RegistrationNumber = "CH-2026-014",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity_inreviewId))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity_inreviewId,
                    UserId = Charity_inreviewUserId,
                    CharityName = "جمعية المستقبل المشرق",
                    CharityDescription = "جمعية قيد الدراسة والبحث الميداني.",
                    IsActive = true,
                    VerificationState = VerificationState.InReview,
                    RegistrationNumber = "CH-2026-015",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Charities.AnyAsync(c => c.CharityId == Charity_rejectedId))
            {
                await db.Charities.AddAsync(new Charity
                {
                    CharityId = Charity_rejectedId,
                    UserId = Charity_rejectedUserId,
                    CharityName = "جمعية بسمة المحتاج",
                    CharityDescription = "جمعية تم رفض طلبها لعدم استيفاء المستندات المطلوبة.",
                    IsActive = true,
                    VerificationState = VerificationState.Rejected,
                    RegistrationNumber = "CH-2026-016",
                    RegistrationDate = new DateTime(2012, 4, 12),
                    HeadquartersAddress = "جمهورية مصر العربية",
                    AuthorizedPersonName = "أ/ ممثل الجمعية",
                    AuthorizedPersonPosition = "المدير المسؤول",
                    RegistrationCertificateUrl = dummyPdf,
                    BylawsUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            await db.SaveChangesAsync();
        }


        // =========================================================
        // DONORS
        // =========================================================
        private static async Task SeedDonorsAsync(ApplicationDbContext db)
        {
            var dummyPdf = "seed/dummy.pdf";

            if (!await db.DonorOrganizations.AnyAsync(d => d.DonorOrganizationId == Donor1Id))
            {
                await db.DonorOrganizations.AddAsync(new DonorOrganization
                {
                    DonorOrganizationId = Donor1Id,
                    UserId = Donor1UserId,
                    DonorOrganizationName = "مؤسسة إيد في إيد",
                    DonorOrganizationDescription = "مؤسسة خيرية بتشتغل على دعم الأسر المحتاجة من خلال توفير الغذاء والاحتياجات الأساسية بشكل منتظم.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    CommercialRegistrationNumber = "CR-2026-001",
                    CommercialRegistrationDate = new DateTime(2015, 6, 20),
                    TaxNumber = "TAX-222-001",
                    BusinessLicenseNumber = "LIC-001",
                    HeadquartersAddress = "المنطقة الصناعية، مصر",
                    CommercialRegisterUrl = dummyPdf,
                    TaxCardUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.DonorOrganizations.AnyAsync(d => d.DonorOrganizationId == Donor2Id))
            {
                await db.DonorOrganizations.AddAsync(new DonorOrganization
                {
                    DonorOrganizationId = Donor2Id,
                    UserId = Donor2UserId,
                    DonorOrganizationName = "مؤسسة الخير دايم",
                    DonorOrganizationDescription = "مؤسسة بتسعى لنشر الخير المستدام عن طريق توفير المواد الغذائية والملابس للأسر الأولى بالرعاية.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    CommercialRegistrationNumber = "CR-2026-002",
                    CommercialRegistrationDate = new DateTime(2015, 6, 20),
                    TaxNumber = "TAX-222-002",
                    BusinessLicenseNumber = "LIC-002",
                    HeadquartersAddress = "المنطقة الصناعية، مصر",
                    CommercialRegisterUrl = dummyPdf,
                    TaxCardUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.DonorOrganizations.AnyAsync(d => d.DonorOrganizationId == Donor3Id))
            {
                await db.DonorOrganizations.AddAsync(new DonorOrganization
                {
                    DonorOrganizationId = Donor3Id,
                    UserId = Donor3UserId,
                    DonorOrganizationName = "مؤسسة سند",
                    DonorOrganizationDescription = "مؤسسة تهدف إلى مساندة الأسر البسيطة وتخفيف الأعباء المعيشية عنهم في جميع المواسم.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    CommercialRegistrationNumber = "CR-2026-003",
                    CommercialRegistrationDate = new DateTime(2015, 6, 20),
                    TaxNumber = "TAX-222-003",
                    BusinessLicenseNumber = "LIC-003",
                    HeadquartersAddress = "المنطقة الصناعية، مصر",
                    CommercialRegisterUrl = dummyPdf,
                    TaxCardUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.DonorOrganizations.AnyAsync(d => d.DonorOrganizationId == Donor4Id))
            {
                await db.DonorOrganizations.AddAsync(new DonorOrganization
                {
                    DonorOrganizationId = Donor4Id,
                    UserId = Donor4UserId,
                    DonorOrganizationName = "مؤسسة عطاء بلا حدود",
                    DonorOrganizationDescription = "مؤسسة إنسانية بتقدم المساعدات الغذائية والطبية لكل من يحتاج بدون تمييز.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    CommercialRegistrationNumber = "CR-2026-004",
                    CommercialRegistrationDate = new DateTime(2015, 6, 20),
                    TaxNumber = "TAX-222-004",
                    BusinessLicenseNumber = "LIC-004",
                    HeadquartersAddress = "المنطقة الصناعية، مصر",
                    CommercialRegisterUrl = dummyPdf,
                    TaxCardUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.DonorOrganizations.AnyAsync(d => d.DonorOrganizationId == Donor5Id))
            {
                await db.DonorOrganizations.AddAsync(new DonorOrganization
                {
                    DonorOrganizationId = Donor5Id,
                    UserId = Donor5UserId,
                    DonorOrganizationName = "مؤسسة بسمة أمل",
                    DonorOrganizationDescription = "مؤسسة خيرية بتؤمن إن المساعدة البسيطة تقدر تصنع فرق كبير في حياة الناس.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    CommercialRegistrationNumber = "CR-2026-005",
                    CommercialRegistrationDate = new DateTime(2015, 6, 20),
                    TaxNumber = "TAX-222-005",
                    BusinessLicenseNumber = "LIC-005",
                    HeadquartersAddress = "المنطقة الصناعية، مصر",
                    CommercialRegisterUrl = dummyPdf,
                    TaxCardUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.DonorOrganizations.AnyAsync(d => d.DonorOrganizationId == Donor6Id))
            {
                await db.DonorOrganizations.AddAsync(new DonorOrganization
                {
                    DonorOrganizationId = Donor6Id,
                    UserId = Donor6UserId,
                    DonorOrganizationName = "مؤسسة السلام",
                    DonorOrganizationDescription = "يركز على إنتاج السلع الاستهلاكية التي تلبي احتياجات الأسر اليومية. وتهدف المؤسسة إلى تطوير الإنتاج المحلي وتوفير منتجات عالية الجودة مع دعم فرص العمل وتحسين الاقتصاد المجتمعي.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    CommercialRegistrationNumber = "CR-2026-006",
                    CommercialRegistrationDate = new DateTime(2015, 6, 20),
                    TaxNumber = "TAX-222-006",
                    BusinessLicenseNumber = "LIC-006",
                    HeadquartersAddress = "المنطقة الصناعية، مصر",
                    CommercialRegisterUrl = dummyPdf,
                    TaxCardUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.DonorOrganizations.AnyAsync(d => d.DonorOrganizationId == Donor7Id))
            {
                await db.DonorOrganizations.AddAsync(new DonorOrganization
                {
                    DonorOrganizationId = Donor7Id,
                    UserId = Donor7UserId,
                    DonorOrganizationName = "مؤسسة نور الحياة",
                    DonorOrganizationDescription = "مؤسسة خيرية بتسعى لتوصيل الدعم لمستحقيه في أسرع وقت وبأفضل صورة.",
                    IsActive = true,
                    VerificationState = VerificationState.Verified,
                    CommercialRegistrationNumber = "CR-2026-007",
                    CommercialRegistrationDate = new DateTime(2015, 6, 20),
                    TaxNumber = "TAX-222-007",
                    BusinessLicenseNumber = "LIC-007",
                    HeadquartersAddress = "المنطقة الصناعية، مصر",
                    CommercialRegisterUrl = dummyPdf,
                    TaxCardUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.DonorOrganizations.AnyAsync(d => d.DonorOrganizationId == Donor_pendingId))
            {
                await db.DonorOrganizations.AddAsync(new DonorOrganization
                {
                    DonorOrganizationId = Donor_pendingId,
                    UserId = Donor_pendingUserId,
                    DonorOrganizationName = "مؤسسة الدعم والمساندة",
                    DonorOrganizationDescription = "مؤسسة مانحة قيد الانتظار لمراجعة الأوراق.",
                    IsActive = true,
                    VerificationState = VerificationState.Pending,
                    CommercialRegistrationNumber = "CR-2026-008",
                    CommercialRegistrationDate = new DateTime(2015, 6, 20),
                    TaxNumber = "TAX-222-008",
                    BusinessLicenseNumber = "LIC-008",
                    HeadquartersAddress = "المنطقة الصناعية، مصر",
                    CommercialRegisterUrl = dummyPdf,
                    TaxCardUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.DonorOrganizations.AnyAsync(d => d.DonorOrganizationId == Donor_inreviewId))
            {
                await db.DonorOrganizations.AddAsync(new DonorOrganization
                {
                    DonorOrganizationId = Donor_inreviewId,
                    UserId = Donor_inreviewUserId,
                    DonorOrganizationName = "شركة العطاء القابضة",
                    DonorOrganizationDescription = "شركة مانحة قيد البحث المالي.",
                    IsActive = true,
                    VerificationState = VerificationState.InReview,
                    CommercialRegistrationNumber = "CR-2026-009",
                    CommercialRegistrationDate = new DateTime(2015, 6, 20),
                    TaxNumber = "TAX-222-009",
                    BusinessLicenseNumber = "LIC-009",
                    HeadquartersAddress = "المنطقة الصناعية، مصر",
                    CommercialRegisterUrl = dummyPdf,
                    TaxCardUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.DonorOrganizations.AnyAsync(d => d.DonorOrganizationId == Donor_rejectedId))
            {
                await db.DonorOrganizations.AddAsync(new DonorOrganization
                {
                    DonorOrganizationId = Donor_rejectedId,
                    UserId = Donor_rejectedUserId,
                    DonorOrganizationName = "مجموعة الخير الدولية",
                    DonorOrganizationDescription = "مؤسسة تم رفض طلب توثيقها لعدم صلاحية السجل التجاري.",
                    IsActive = true,
                    VerificationState = VerificationState.Rejected,
                    CommercialRegistrationNumber = "CR-2026-010",
                    CommercialRegistrationDate = new DateTime(2015, 6, 20),
                    TaxNumber = "TAX-222-010",
                    BusinessLicenseNumber = "LIC-010",
                    HeadquartersAddress = "المنطقة الصناعية، مصر",
                    CommercialRegisterUrl = dummyPdf,
                    TaxCardUrl = dummyPdf,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            await db.SaveChangesAsync();
        }


        // =========================================================
        // NEEDS
        // =========================================================
        private static async Task SeedNeedsAsync(ApplicationDbContext db)
        {

            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need1Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need1Id,
                    CharityId = Charity1Id,
                    ProductName = "حملة أرز",
                    Quantity = 50.0m,
                    Unit = MeasurementUnit.Ton,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/alnoor_charity_rice.jpeg",
                    Description = "إعلان عن حملة لجمع وتوفير الأرز للأسر المحتاجة بهدف دعم الأمن الغذائي وتخفيف العبء عن الفئات الفقيرة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need2Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need2Id,
                    CharityId = Charity1Id,
                    ProductName = "حملة سكر",
                    Quantity = 300.0m,
                    Unit = MeasurementUnit.Kg,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/alnoor_charity_sugar.jpeg",
                    Description = "مبادرة إنسانية لتوفير السكر وتوزيعه على الأسر المحتاجة ضمن برنامج دعم غذائي مستمر. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need3Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need3Id,
                    CharityId = Charity2Id,
                    ProductName = "سلال غذائية",
                    Quantity = 200.0m,
                    Unit = MeasurementUnit.Box,
                    Category = ProductCategory.Other,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/hope_association_food_baskets.jpeg",
                    Description = "مشروع لتجهيز سلال غذائية متكاملة تحتوي على مواد أساسية لدعم الأسر الفقيرة. وتشمل (أرز، سكر، مكرونة، زيت، شاي، وصلصة لتخفيف الأعباء وتوفير الاحتياجات الأساسية).",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need4Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need4Id,
                    CharityId = Charity2Id,
                    ProductName = "ملابس شتوية",
                    Quantity = 250.0m,
                    Unit = MeasurementUnit.Piece,
                    Category = ProductCategory.Clothing,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/hope_association_winter_clothes.jpeg",
                    Description = "حملة لتوفير ملابس شتوية للأسر المحتاجة لمساعدتهم في مواجهة برودة الشتاء. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need5Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need5Id,
                    CharityId = Charity3Id,
                    ProductName = "بطاطين",
                    Quantity = 150.0m,
                    Unit = MeasurementUnit.Piece,
                    Category = ProductCategory.Other,
                    Status = CharityNeedStatus.Pending,
                    ProductImage = "seed/life_association_blankets.jpeg",
                    Description = "مبادرة إنسانية لتوزيع بطاطين على الأسر الفقيرة لتوفير الدفء خلال فصل الشتاء. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need6Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need6Id,
                    CharityId = Charity3Id,
                    ProductName = "مساعدات طبية",
                    Quantity = 500.0m,
                    Unit = MeasurementUnit.Can,
                    Category = ProductCategory.Medical,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/life_association_medicine.jpeg",
                    Description = "تجهيز وتوزيع مستلزمات طبية أساسية لدعم الحالات المحتاجة والرعاية الصحية الأولية. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need7Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need7Id,
                    CharityId = Charity4Id,
                    ProductName = "زيت طعام",
                    Quantity = 400.0m,
                    Unit = MeasurementUnit.Liter,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/mercy_association_oil.jpeg",
                    Description = "حملة لتوفير زيت الطعام للأسر المحتاجة ضمن سلة غذائية متكاملة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need8Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need8Id,
                    CharityId = Charity4Id,
                    ProductName = "مكرونة",
                    Quantity = 350.0m,
                    Unit = MeasurementUnit.Kg,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/mercy_association_pasta.jpeg",
                    Description = "توفير كميات من المكرونة كجزء من الدعم الغذائي للأسر الفقيرة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need9Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need9Id,
                    CharityId = Charity5Id,
                    ProductName = "أرز",
                    Quantity = 500.0m,
                    Unit = MeasurementUnit.Kg,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/goodness_association_rice.jpeg",
                    Description = "مبادرة لدعم الأسر من خلال توفير الأرز ضمن سلال غذائية متكاملة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need10Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need10Id,
                    CharityId = Charity5Id,
                    ProductName = "أدوية",
                    Quantity = 120.0m,
                    Unit = MeasurementUnit.Can,
                    Category = ProductCategory.Medical,
                    Status = CharityNeedStatus.Fulfilled,
                    ProductImage = "seed/goodness_association_medicine.jpeg",
                    Description = "توفير أدوية أساسية لدعم الحالات المرضية غير القادرة على العلاج. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need11Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need11Id,
                    CharityId = Charity6Id,
                    ProductName = "ملابس أطفال",
                    Quantity = 180.0m,
                    Unit = MeasurementUnit.Piece,
                    Category = ProductCategory.Clothing,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/future_association_kids_clothes.jpeg",
                    Description = "جمع وتوزيع ملابس للأطفال المحتاجين لدعم احتياجاتهم اليومية. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need12Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need12Id,
                    CharityId = Charity6Id,
                    ProductName = "مياه شرب",
                    Quantity = 600.0m,
                    Unit = MeasurementUnit.Liter,
                    Category = ProductCategory.Other,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/future_association_water.jpeg",
                    Description = "توفير مياه شرب نظيفة للأسر في المناطق الأكثر احتياجاً. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need13Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need13Id,
                    CharityId = Charity7Id,
                    ProductName = "سلة رمضان",
                    Quantity = 220.0m,
                    Unit = MeasurementUnit.Box,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/cooperation_association_ramadan_basket.jpeg",
                    Description = "حملة رمضانية لتوزيع سلال غذائية على الأسر المحتاجة خلال شهر رمضان. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need14Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need14Id,
                    CharityId = Charity7Id,
                    ProductName = "حملة دفء",
                    Quantity = 200.0m,
                    Unit = MeasurementUnit.Box,
                    Category = ProductCategory.Clothing,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/cooperation_association_winter_clothes.jpeg",
                    Description = "مبادرة شتوية لتوفير بطاطين وملابس ثقيلة للأسر الفقيرة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need15Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need15Id,
                    CharityId = Charity8Id,
                    ProductName = "مواد غذائية",
                    Quantity = 300.0m,
                    Unit = MeasurementUnit.Box,
                    Category = ProductCategory.Other,
                    Status = CharityNeedStatus.Pending,
                    ProductImage = "seed/faith_association_food_stuffs.jpeg",
                    Description = "دعم غذائي شامل يهدف إلى توفير المواد الأساسية للأسر المحتاجة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need16Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need16Id,
                    CharityId = Charity8Id,
                    ProductName = "فول",
                    Quantity = 450.0m,
                    Unit = MeasurementUnit.Can,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/faith_association_beans.jpeg",
                    Description = "حملة توفير الفول  كجزء من المساعدات الغذائية الأساسية. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need17Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need17Id,
                    CharityId = Charity8Id,
                    ProductName = "أرز",
                    Quantity = 250.0m,
                    Unit = MeasurementUnit.Kg,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/faith_association_rice.jpeg",
                    Description = "توفير الأرز كعنصر غذائي أساسي لدعم الأمن الغذائي للأسر الفقيرة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need18Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need18Id,
                    CharityId = Charity9Id,
                    ProductName = "السكر",
                    Quantity = 200.0m,
                    Unit = MeasurementUnit.Kg,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/white_hands_association_sugar.jpeg",
                    Description = "توزيع السكر ضمن برامج الدعم الغذائي للأسر المحتاجة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need19Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need19Id,
                    CharityId = Charity9Id,
                    ProductName = "زيت",
                    Quantity = 150.0m,
                    Unit = MeasurementUnit.Liter,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/white_hands_association_oil.jpeg",
                    Description = "توفير زيت الطعام كجزء من المساعدات الغذائية الأساسية. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need20Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need20Id,
                    CharityId = Charity9Id,
                    ProductName = "بسلة",
                    Quantity = 200.0m,
                    Unit = MeasurementUnit.Can,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Fulfilled,
                    ProductImage = "seed/white_hands_association_peas.jpeg",
                    Description = "توفير بعبوة بسلة (معلبة أو مجمدة) كعنصر غذائي أساسي لدعم الأمن الغذائي للأسر الفقيرة.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need21Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need21Id,
                    CharityId = Charity10Id,
                    ProductName = "شنط مدرسة",
                    Quantity = 400.0m,
                    Unit = MeasurementUnit.Piece,
                    Category = ProductCategory.Education,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/light_of_life_association_bags.jpeg",
                    Description = "تقديم شنط مدرسة لدعم الأسر المحتاجة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need22Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need22Id,
                    CharityId = Charity10Id,
                    ProductName = "صلصة طماطم",
                    Quantity = 180.0m,
                    Unit = MeasurementUnit.Can,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/light_of_life_association_sauce.jpeg",
                    Description = "توزيع حقائب غذائية تحتوي على احتياجات أساسية للأسر الفقيرة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need23Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need23Id,
                    CharityId = Charity11Id,
                    ProductName = "شاي",
                    Quantity = 300.0m,
                    Unit = MeasurementUnit.Can,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/dignity_association_tea.jpeg",
                    Description = "مبادرة لدعم الأسر المحتاجة بمساعدات متنوعة غذائية واجتماعية. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need24Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need24Id,
                    CharityId = Charity11Id,
                    ProductName = "مستلزمات منزلية",
                    Quantity = 130.0m,
                    Unit = MeasurementUnit.Box,
                    Category = ProductCategory.Other,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/dignity_association_home_supplies.jpeg",
                    Description = "توفير مستلزمات منزلية أساسية( اواني واطباق واكواب ) للأسر الفقيرة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need25Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need25Id,
                    CharityId = Charity12Id,
                    ProductName = "بطاطس",
                    Quantity = 500.0m,
                    Unit = MeasurementUnit.Kg,
                    Category = ProductCategory.Food,
                    Status = CharityNeedStatus.Pending,
                    ProductImage = "seed/happiness_association_potatoes.jpeg",
                    Description = "سلة غذائية متكاملة تحتوي على أهم المواد الأساسية. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need26Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need26Id,
                    CharityId = Charity12Id,
                    ProductName = "بطاطين",
                    Quantity = 250.0m,
                    Unit = MeasurementUnit.Piece,
                    Category = ProductCategory.Other,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/happiness_blankets.jpeg",
                    Description = "مبادرة شتوية لتوزيع بطاطين على الأسر المحتاجة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need27Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need27Id,
                    CharityId = Charity13Id,
                    ProductName = "أدوية أساسية",
                    Quantity = 90.0m,
                    Unit = MeasurementUnit.Box,
                    Category = ProductCategory.Medical,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/new_hope_medicine.jpeg",
                    Description = "توفير أدوية ضرورية لدعم الحالات الصحية المحتاجة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.CharityNeeds.AnyAsync(n => n.CharityNeedId == Need28Id))
            {
                await db.CharityNeeds.AddAsync(new CharityNeed
                {
                    CharityNeedId = Need28Id,
                    CharityId = Charity13Id,
                    ProductName = "مياه شرب",
                    Quantity = 1000.0m,
                    Unit = MeasurementUnit.Liter,
                    Category = ProductCategory.Other,
                    Status = CharityNeedStatus.Approved,
                    ProductImage = "seed/new_hope_water.jpeg",
                    Description = "دعم طارئ يشمل مياه شرب ومواد غذائية للأسر المتضررة. تهدف هذه المبادرة إلى تخفيف الأعباء المعيشية عن الأسر المحتاجة وتوفير الاحتياجات الأساسية لهم بشكل منتظم وآمن.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            await db.SaveChangesAsync();
        }


        // =========================================================
        // OFFERS
        // =========================================================
        private static async Task SeedOffersAsync(ApplicationDbContext db)
        {

            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer1Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer1Id,
                    DonorOrganizationId = Donor1Id,
                    ProductName = "سلال غذائية",
                    Quantity = 1000.0m,
                    Unit = MeasurementUnit.Box,
                    Category = ProductCategory.Other,
                    Status = OfferStatus.Approved,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/hand_in_hand_food_basket.jpeg",
                    Description = "المتوفر حاليًا: سلال غذائية متكاملة وتشمل (أرز، سكر، مكرونة، زيت، شاي، وصلصة لتخفيف الأعباء وتوفير الاحتياجات الأساسية).",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer2Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer2Id,
                    DonorOrganizationId = Donor1Id,
                    ProductName = "أرز",
                    Quantity = 300.0m,
                    Unit = MeasurementUnit.Kg,
                    Category = ProductCategory.Food,
                    Status = OfferStatus.Approved,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/hand_in_hand_rice.jpeg",
                    Description = "متوفر لدينا كمية من الأرز",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer3Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer3Id,
                    DonorOrganizationId = Donor1Id,
                    ProductName = "زيت",
                    Quantity = 370.0m,
                    Unit = MeasurementUnit.Liter,
                    Category = ProductCategory.Food,
                    Status = OfferStatus.Approved,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/hand_in_hand_oil.jpeg",
                    Description = "متوفر لدينا كمية من الزيت",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer4Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer4Id,
                    DonorOrganizationId = Donor2Id,
                    ProductName = "سكر",
                    Quantity = 200.0m,
                    Unit = MeasurementUnit.Kg,
                    Category = ProductCategory.Food,
                    Status = OfferStatus.Approved,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/always_good_sugar.jpeg",
                    Description = "متوفر لدينا كمية من السكر",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer5Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer5Id,
                    DonorOrganizationId = Donor2Id,
                    ProductName = "بطاطين",
                    Quantity = 450.0m,
                    Unit = MeasurementUnit.Piece,
                    Category = ProductCategory.Other,
                    Status = OfferStatus.Pending,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/always_good_blankets.jpeg",
                    Description = "متوفر لدينا كمية من البطاطين للأسر المحتاجة.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer6Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer6Id,
                    DonorOrganizationId = Donor2Id,
                    ProductName = "ملابس شتوية",
                    Quantity = 100.0m,
                    Unit = MeasurementUnit.Piece,
                    Category = ProductCategory.Clothing,
                    Status = OfferStatus.Approved,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/always_good_winter_clothes.jpeg",
                    Description = "متوفر لدينا كمية من  الملابس الشتوية للأسر المحتاجة.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer7Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer7Id,
                    DonorOrganizationId = Donor3Id,
                    ProductName = "بطاطس",
                    Quantity = 2.0m,
                    Unit = MeasurementUnit.Ton,
                    Category = ProductCategory.Food,
                    Status = OfferStatus.Approved,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/sanad_foundation_potatoes.jpeg",
                    Description = "متوفر لدينا: بطاطس طازجة بجودة عالية لدعم الأسر المحتاجة.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer8Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer8Id,
                    DonorOrganizationId = Donor3Id,
                    ProductName = "صلصة طماطم",
                    Quantity = 600.0m,
                    Unit = MeasurementUnit.Kg,
                    Category = ProductCategory.Food,
                    Status = OfferStatus.Approved,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/sanad_foundation_tomato_sauce.jpeg",
                    Description = "متوفر لدينا كمية من صلصة الطماطم",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer9Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer9Id,
                    DonorOrganizationId = Donor4Id,
                    ProductName = "مساعدات طبية",
                    Quantity = 50.0m,
                    Unit = MeasurementUnit.Can,
                    Category = ProductCategory.Medical,
                    Status = OfferStatus.Approved,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/giving_without_limits_medical.jpeg",
                    Description = "المتوفر حاليًا: مساعدات طبية.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer10Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer10Id,
                    DonorOrganizationId = Donor4Id,
                    ProductName = "شاي",
                    Quantity = 30.0m,
                    Unit = MeasurementUnit.Can,
                    Category = ProductCategory.Food,
                    Status = OfferStatus.Fulfilled,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/giving_without_limits_tea.jpeg",
                    Description = "متوفر لدينا كمية من الشاي",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer11Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer11Id,
                    DonorOrganizationId = Donor5Id,
                    ProductName = "بطاطس",
                    Quantity = 50.0m,
                    Unit = MeasurementUnit.Kg,
                    Category = ProductCategory.Food,
                    Status = OfferStatus.Approved,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/smile_of_hope_potatoes.jpeg",
                    Description = "متوفر لدينا: بطاطس طازجة بجودة عالية لدعم الأسر المحتاجة.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer12Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer12Id,
                    DonorOrganizationId = Donor5Id,
                    ProductName = "عبوات(بسله)",
                    Quantity = 30.0m,
                    Unit = MeasurementUnit.Can,
                    Category = ProductCategory.Other,
                    Status = OfferStatus.Approved,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/smile_of_hope_peas.jpeg",
                    Description = "متوفر لدينا: عبوات بقوليات متنوعة لدعم المطبخ اليومي.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer13Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer13Id,
                    DonorOrganizationId = Donor6Id,
                    ProductName = "مياه شرب",
                    Quantity = 1000.0m,
                    Unit = MeasurementUnit.Liter,
                    Category = ProductCategory.Other,
                    Status = OfferStatus.Approved,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/peace_foundation_water.jpeg",
                    Description = "متوفر لدينا كمية من المياه الشرب.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer14Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer14Id,
                    DonorOrganizationId = Donor6Id,
                    ProductName = "مكرونة",
                    Quantity = 20.0m,
                    Unit = MeasurementUnit.Kg,
                    Category = ProductCategory.Food,
                    Status = OfferStatus.Approved,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/peace_association_pasta.jpeg",
                    Description = "متوفر لدينا كمية من المكرونة.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer15Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer15Id,
                    DonorOrganizationId = Donor7Id,
                    ProductName = "سلة رمضان",
                    Quantity = 50.0m,
                    Unit = MeasurementUnit.Box,
                    Category = ProductCategory.Food,
                    Status = OfferStatus.Pending,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/light_of_life_ramadan_bag.jpeg",
                    Description = "متاح الآن: شنط غذائية جاهزة للتسليم الفوري وتشمل (أرز، سكر، مكرونة، زيت، شاي، وصلصة لتخفيف الأعباء وتوفير الاحتياجات الأساسية).",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            if (!await db.Offers.AnyAsync(o => o.OfferId == Offer16Id))
            {
                await db.Offers.AddAsync(new Offer
                {
                    OfferId = Offer16Id,
                    DonorOrganizationId = Donor7Id,
                    ProductName = "سلال غذائية",
                    Quantity = 30.0m,
                    Unit = MeasurementUnit.Box,
                    Category = ProductCategory.Other,
                    Status = OfferStatus.Approved,
                    ExpiryDate = DateTime.UtcNow.AddDays(90),
                    ProductImage = "seed/light_of_life_foundation_food_basket.jpeg",
                    Description = "متوفر لدينا كمية من سلال غذائية وتشمل (أرز، سكر، مكرونة، زيت، شاي، وصلصة لتخفيف الأعباء وتوفير الاحتياجات الأساسية).",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            await db.SaveChangesAsync();
        }


        // =========================================================
        // NEED APPLICATIONS
        // =========================================================
        private static async Task SeedNeedApplicationsAsync(ApplicationDbContext db)
        {
            // Seed a couple of pending/accepted/fulfilled need applications for testing
            if (!await db.NeedApplications.AnyAsync(a => a.NeedApplicationId == NeedApp1Id))
            {
                await db.NeedApplications.AddAsync(new NeedApplication
                {
                    NeedApplicationId = NeedApp1Id,
                    DonorOrganizationId = Donor1Id,
                    CharityNeedId = Need1Id,
                    Status = ApplicationStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (!await db.NeedApplications.AnyAsync(a => a.NeedApplicationId == NeedApp2Id))
            {
                await db.NeedApplications.AddAsync(new NeedApplication
                {
                    NeedApplicationId = NeedApp2Id,
                    DonorOrganizationId = Donor2Id,
                    CharityNeedId = Need2Id,
                    Status = ApplicationStatus.Accepted,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Fulfilled need application (completed transaction)
            if (!await db.NeedApplications.AnyAsync(a => a.NeedApplicationId == NeedApp3Id))
            {
                await db.NeedApplications.AddAsync(new NeedApplication
                {
                    NeedApplicationId = NeedApp3Id,
                    DonorOrganizationId = Donor3Id,
                    CharityNeedId = Need3Id,
                    Status = ApplicationStatus.Fulfilled,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await db.SaveChangesAsync();
        }


        // =========================================================
        // OFFER APPLICATIONS
        // =========================================================
        private static async Task SeedOfferApplicationsAsync(ApplicationDbContext db)
        {
            if (!await db.OfferApplications.AnyAsync(a => a.OfferApplicationId == OfferApp1Id))
            {
                await db.OfferApplications.AddAsync(new OfferApplication
                {
                    OfferApplicationId = OfferApp1Id,
                    CharityId = Charity1Id,
                    OfferId = Offer1Id,
                    Status = ApplicationStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (!await db.OfferApplications.AnyAsync(a => a.OfferApplicationId == OfferApp2Id))
            {
                await db.OfferApplications.AddAsync(new OfferApplication
                {
                    OfferApplicationId = OfferApp2Id,
                    CharityId = Charity2Id,
                    OfferId = Offer2Id,
                    Status = ApplicationStatus.Accepted,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await db.SaveChangesAsync();
        }
    }
}