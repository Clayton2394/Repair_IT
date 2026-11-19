using Microsoft.EntityFrameworkCore;
using RepairIT.CatalogService.Configuration;
using RepairIT.Shared.Configuration;
using RepairIT.CatalogService.Data;
using RepairIT.CatalogService.Repositories;
using RepairIT.CatalogService.Services;

const string PolitiqueCorsClient = "ClientBlazor";

var builder = WebApplication.CreateBuilder(args);

// Configuration typée (pattern Options)
builder.Services.Configure<CatalogueOptions>(builder.Configuration.GetSection(CatalogueOptions.Section));
builder.Services.Configure<BaseDeDonneesOptions>(builder.Configuration.GetSection(BaseDeDonneesOptions.Section));

// Persistance : SQLite en V1 (sera remplacé par PostgreSQL en V2)
builder.Services.AddDbContext<CatalogueDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Catalogue")));

// Injection des dépendances
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IOrdinateurRepository, OrdinateurRepository>();
builder.Services.AddScoped<IOrdinateurService, OrdinateurService>();
builder.Services.AddScoped<CatalogueDbInitializer>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors(options => options.AddPolicy(PolitiqueCorsClient, policy => policy
    .WithOrigins(builder.Configuration.GetSection("Cors:OriginesAutorisees").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<CatalogueDbInitializer>().InitialiserAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(PolitiqueCorsClient);
app.MapControllers();

app.Run();
