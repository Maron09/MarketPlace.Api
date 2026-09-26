using System.Text;
using Amazon.S3;
using FluentValidation;
using FluentValidation.AspNetCore;
using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Cart;
using Marketplace.Modules.Catalog;
using Marketplace.Modules.Catalog.Api;
using Marketplace.Modules.Catalog.Infrastructure;
using Marketplace.Modules.Identity;
using Marketplace.Modules.Identity.Application;
using Marketplace.Modules.Identity.Infrastructure;
using Marketplace.Modules.Inventory;
using Marketplace.Modules.Vendors;
using Marketplace.SharedKernel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

try
{
    Log.Information("Starting Marketplace API");

    builder.Services.AddIdentityModule(builder.Configuration);
    builder.Services.AddVendorsModule();
    builder.Services.AddCatalogModule(builder.Configuration);
    builder.Services.AddCartModule();
    builder.Services.AddInventoryModule();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUser, CurrentUser>();

    builder.Services.AddDbContext<MarketplaceDbContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("Marketplace")));

    var jwtSection = builder.Configuration.GetSection("Jwt");
    var signingKey = builder.Configuration["Jwt:SigningKey"]
        ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSection["Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
            };
        });

    builder.Services.AddAuthorization();

    builder.Services.AddControllers()
        .AddApplicationPart(typeof(IdentityModuleExtensions).Assembly)
        .AddApplicationPart(typeof(VendorModuleExtensions).Assembly)
        .AddApplicationPart(typeof(CatalogModuleExtensions).Assembly)
        .AddApplicationPart(typeof(CartModuleExtension).Assembly)
        .AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

    builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddOpenApi();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await AdminSeeder.SeedAsync(dbContext, passwordHasher, "admin@marketplace.local", "AdminPass123!"); // Note: find a safer way to implement in production
        Log.Information("Admin account seeded (or already existed)");

        var s3Client = scope.ServiceProvider.GetRequiredService<IAmazonS3>();
        var minioSettings = scope.ServiceProvider.GetRequiredService<IOptions<MinioSetings>>();
        await BucketInitializer.EnsureBucketExistsAsync(s3Client, minioSettings);
        Log.Information("MinIO bucket check completed successfully");
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}