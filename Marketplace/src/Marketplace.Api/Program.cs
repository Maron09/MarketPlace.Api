using FluentValidation;
using FluentValidation.AspNetCore;
using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Catalog;
using Marketplace.Modules.Catalog.Api;
using Marketplace.Modules.Identity;
using Marketplace.Modules.Vendors;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddVendorsModule();
builder.Services.AddCatalogModule();

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
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
