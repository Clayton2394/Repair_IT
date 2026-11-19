using Microsoft.EntityFrameworkCore;
using RepairIT.UserService.Models;

namespace RepairIT.UserService.Data;

public class UtilisateursDbContext(DbContextOptions<UtilisateursDbContext> options) : DbContext(options)
{
    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Utilisateur>(utilisateur =>
        {
            utilisateur.ToTable("Utilisateurs");
            utilisateur.HasKey(u => u.Id);
            utilisateur.Property(u => u.UserKeycloak).IsRequired().HasMaxLength(100);
            utilisateur.Property(u => u.Nom).IsRequired().HasMaxLength(50);
            utilisateur.Property(u => u.Prenom).IsRequired().HasMaxLength(50);
            utilisateur.Property(u => u.Email).IsRequired().HasMaxLength(100);
            utilisateur.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
            utilisateur.HasIndex(u => u.Email).IsUnique();
        });
    }
}
