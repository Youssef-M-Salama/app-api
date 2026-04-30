using App.Core.Domain.IdentityEntities;
using App.Core.Domain.RepositoryContracts;
using App.Core.ServiceContracts;
using App.Core.Services;
using App.Core.Settings;
using App.Infrastructure.DbContext;
using App.Infrastructure.Repository;
using App.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

namespace App.Api.StartupExtensions
{
    public static class ConfigureServicesExtension
    {
        /// <summary>
        /// Configure all application services (DbContext, Identity, JWT, Application Services)
        /// </summary>
        public static IServiceCollection ConfigureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ── Application Services ──────────────────────────────────────────
            services.AddTransient<IJwtService, JwtService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPublicService, PublicService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ICharityService, CharityService>();
            services.AddScoped<IDonorOrganizationService, DonorOrganizationService>();

            // ── Repositories ─────────────────────────────────────────────────
            services.AddScoped<ICharityNeedRepository, CharityNeedRepository>();
            services.AddScoped<IOfferRepository, OfferRepository>();
            services.AddScoped<ICharityRepository, CharityRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IDonorOrganizationRepository, DonorOrganizationRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<INeedApplicationRepository, NeedApplicationRepository>();
            services.AddScoped<IOfferApplicationRepository, OfferApplicationRepository>();

            // ── Settings ──────────────────────────────────────────────────────
            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));


            //Origns
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // ── Database Context ─────────────────────────────────────────────
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PublicProductionConnection"));
            });

            // Identity — use AddIdentityCore so it does NOT override the JWT auth scheme.
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                ConfigurePasswordPolicy(options, configuration);
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddSignInManager<SignInManager<ApplicationUser>>();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(
                    configuration.GetValue<int>("AppSettings:EmailVerificationTokenExpirationHours"));
            });

            // JWT Authentication — registered after Identity so nothing overrides it
            services.ConfigureJwtAuthentication(configuration);

            return services;
        }

        /// <summary>
        /// Configure Swagger with XML documentation and JWT authorization.
        /// </summary>
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                var xmlFile = Path.Combine(AppContext.BaseDirectory, "api.xml");
                if (File.Exists(xmlFile))
                    options.IncludeXmlComments(xmlFile);

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "1.0",
                    Title = "Waffer (وافر)",
                    Description = "Waffer (وافر) - RESTful API for connecting charities with donor organizations",
                    Contact = new OpenApiContact
                    {
                        Name = "API Support",
                        Email = "support@example.com"
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter your JWT token. Example: eyJhbGci...",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", document),
                        new List<string>()
                    }
                });
            });

            return services;
        }

        /// <summary>
        /// Configure JWT Bearer authentication
        /// </summary>
        private static IServiceCollection ConfigureJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            configuration["Jwt:Key"]
                            ?? throw new InvalidOperationException(
                                "JWT Key is not configured in appsettings.json")
                        )
                    ),
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }

        /// <summary>
        /// Configure password policy from appsettings.json
        /// </summary>
        private static void ConfigurePasswordPolicy(
            IdentityOptions options,
            IConfiguration configuration)
        {
            var passwordPolicy = configuration.GetSection("PasswordPolicy");

            options.Password.RequiredLength = passwordPolicy.GetValue<int>("RequiredLength");
            options.Password.RequireNonAlphanumeric = passwordPolicy.GetValue<bool>("RequireNonAlphanumeric");
            options.Password.RequireUppercase = passwordPolicy.GetValue<bool>("RequireUppercase");
            options.Password.RequireLowercase = passwordPolicy.GetValue<bool>("RequireLowercase");
            options.Password.RequireDigit = passwordPolicy.GetValue<bool>("RequireDigit");
            options.Password.RequiredUniqueChars = passwordPolicy.GetValue<int>("RequiredUniqueChars");

            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        }
    }
}