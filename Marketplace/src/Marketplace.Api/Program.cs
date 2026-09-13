using FluentValidation;
using FluentValidation.AspNetCore;
using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Catalog;
using Marketplace.Modules.Catalog.Api;
using Marketplace.Modules.Identity;
using Marketplace.Modules.Vendors;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddVendorsModule();
builder.Services.AddCatalogModule();

builder.Services.AddDbContext<MarketplaceDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Marketplace")));

// Add services to the container.

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

app.UseAuthorization();

app.MapControllers();

app.Run();
