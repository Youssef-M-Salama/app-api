using App.Api.Filters;
using App.Api.StartupExtensions;
using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using App.Infrastructure.DbContext;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

// Configure API Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add<ResultLoggingFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    // Custom ModelState validation response format
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = new List<FieldError>();

        foreach (var modelState in context.ModelState)
        {
            foreach (var error in modelState.Value.Errors)
            {
                errors.Add(new FieldError
                {
                    Field = modelState.Key,
                    Message = error.ErrorMessage
                });
            }
        }

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "Validation failed",
            Error = new ErrorInfo
            {
                Code = ErrorCode.VALIDATION_ERROR.ToString(),
                Details = new ValidationErrorDetails { Errors = errors }
            }
        };

        return new BadRequestObjectResult(response);
    };
});



// Configure API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Configure custom services (Identity, JWT, DbContext, etc.)
builder.Services.ConfigureServices(builder.Configuration);

// Configure Swagger with XML documentation and JWT support
builder.Services.ConfigureSwagger();

var app = builder.Build();

// Seed roles on startup
await SeedDatabaseAsync(app);
// Configure middleware pipeline
// if (app.Environment.IsDevelopment())
// {
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1.0");
});
// }
app.UseCors("AllowAll");
app.UseStaticFiles(); // serve wwwroot
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("fixed-sliding");

app.Run();

/// <summary>
/// Seeds database with initial roles
/// </summary>
static async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        await ApplicationDbContextSeed.SeedAllAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogCritical(ex, "An error occurred while seeding the database");
    }
}