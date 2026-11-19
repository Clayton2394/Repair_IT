using Microsoft.EntityFrameworkCore;
using RepairIT.CatalogService.Models;

namespace RepairIT.CatalogService.Data;

public class CatalogueDbContext(DbContextOptions<CatalogueDbContext> options) : DbContext(options)
{
    public DbSet<Ordinateur> Ordinateurs => Set<Ordinateur>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ordinateur>(ordinateur =>
        {
            ordinateur.ToTable("Ordinateurs");
            ordinateur.HasKey(o => o.Id);
            ordinateur.Property(o => o.Marque).IsRequired().HasMaxLength(50);
            ordinateur.Property(o => o.Modele).IsRequired().HasMaxLength(100);
            ordinateur.Property(o => o.Processeur).IsRequired().HasMaxLength(100);
            ordinateur.Property(o => o.Prix).HasPrecision(10, 2);
            ordinateur.Property(o => o.Etat).HasConversion<string>().HasMaxLength(20);
            ordinateur.Property(o => o.Description).HasMaxLength(500);
            ordinateur.HasIndex(o => new { o.Marque, o.Modele });
        });
    }
}
