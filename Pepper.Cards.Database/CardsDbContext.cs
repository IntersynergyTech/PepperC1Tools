using Microsoft.EntityFrameworkCore;
using Pepper.Cards.Data.Models;

namespace Pepper.Cards.Database;

public class CardsDbContext : DbContext
{
    public CardsDbContext(DbContextOptions<CardsDbContext> options) : base(options)
    {
    }

    public DbSet<DeckStyle> DeckStyles { get; set; }
    public DbSet<Card> Cards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure your entities here

        modelBuilder.Entity<DeckStyle>().HasKey(ds => ds.Id);

        modelBuilder.Entity<DeckStyle>().Property(ds => ds.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<Card>().HasKey(c => c.Id);

        modelBuilder.Entity<Card>().HasIndex(c => c.TagUid).IsUnique();

        modelBuilder.Entity<Card>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd()
            .HasAnnotation("Sqlite:Autoincrement", true);

        modelBuilder.Entity<Card>().HasOne(c => c.DeckStyle);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=cards.db");
        }
    }
}
