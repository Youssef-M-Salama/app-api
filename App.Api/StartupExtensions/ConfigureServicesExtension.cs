using App.Core.Domain.IdentityEntities;
using App.Core.RepositoryContracts;
using App.Core.ServiceContracts;
using App.Core.Services;
using App.Infrastructure.DbContext;
using App.Infrastructure.Repository;
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
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Application Services
            services.AddTransient<IJwtService, JwtService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICharityNeedRepository, CharityNeedRepository>();
            services.AddScoped<IPublicService, PublicService>();

            // Database Context
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // Identity with password policy from appsettings
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                ConfigurePasswordPolicy(options, configuration);
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // JWT Authentication
            services.ConfigureJwtAuthentication(configuration);

            return services;
        }

        /// <summary>
        /// Configure Swagger with XML documentation and JWT authorization
        /// </summary>
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // Include XML comments
                var xmlFile = Path.Combine(AppContext.BaseDirectory, "api.xml");
                if (File.Exists(xmlFile))
                {
                    options.IncludeXmlComments(xmlFile);
                }

                // API Information
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "1.0",
                    Title = "Charity-Donor Platform API",
                    Description = "RESTful API for connecting charities with donor organizations",
                    Contact = new OpenApiContact
                    {
                        Name = "API Support",
                        Email = "support@example.com"
                    }
                });

                // JWT Authentication in Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.\n\n" +
                                  "Enter your token in the text input below.\n\n" +
                                  "Example: '12345abcdef'"
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
                            ?? throw new InvalidOperationException("JWT Key not configured in appsettings.json")
                        )
                    ),
                    ClockSkew = TimeSpan.Zero // Remove default 5 minute tolerance
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

            // Additional Identity options
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false; // Email confirmation not required in v1
        }
    }
}