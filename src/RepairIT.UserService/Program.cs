using Microsoft.EntityFrameworkCore;
using RepairIT.Shared.Configuration;
using RepairIT.UserService.Data;
using RepairIT.UserService.Repositories;
using RepairIT.UserService.Services;

const string PolitiqueCorsClient = "ClientBlazor";

var builder = WebApplication.CreateBuilder(args);

// Configuration typée (pattern Options)
builder.Services.Configure<BaseDeDonneesOptions>(builder.Configuration.GetSection(BaseDeDonneesOptions.Section));

// Persistance : SQLite en V1 (sera remplacé par PostgreSQL en V2)
builder.Services.AddDbContext<UtilisateursDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Utilisateurs")));

// Injection des dépendances
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IUtilisateurService, UtilisateurService>();
builder.Services.AddScoped<UtilisateursDbInitializer>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors(options => options.AddPolicy(PolitiqueCorsClient, policy => policy
    .WithOrigins(builder.Configuration.GetSection("Cors:OriginesAutorisees").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<UtilisateursDbInitializer>().InitialiserAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(PolitiqueCorsClient);
app.MapControllers();

app.Run();
