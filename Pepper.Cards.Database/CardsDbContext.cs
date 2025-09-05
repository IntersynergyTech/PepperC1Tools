using Microsoft.EntityFrameworkCore;
using Pepper.Cards.Data.DbModels;

namespace Pepper.Cards.Database;

public class CardsDbContext : DbContext
{
    public CardsDbContext(DbContextOptions<CardsDbContext> options) : base(options)
    {
    }

    public DbSet<DeckStyle> DeckStyles { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<TablePosition> TablePositions { get; set; }
    public DbSet<TablePositionReader> TablePositionReaders { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<GameHand> Hands { get; set; }
    public DbSet<GameType> GameTypes { get; set; }
    public DbSet<HandStep> HandSteps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configure your entities here

        modelBuilder.Entity<DeckStyle>().HasKey(ds => ds.Id);

        modelBuilder.Entity<DeckStyle>().Property(ds => ds.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<Card>().HasKey(c => c.Id);

        modelBuilder.Entity<Card>().HasIndex(c => c.TagUid).IsUnique();

        modelBuilder.Entity<Card>().Property(c => c.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<Card>().HasOne(c => c.DeckStyle);

        modelBuilder.Entity<TablePosition>().HasKey(c => c.Id);

        modelBuilder.Entity<TablePosition>().Property(c => c.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<TablePosition>()
            .HasMany<TablePositionReader>(c => c.Readers)
            .WithOne(r => r.TablePosition)
            .HasForeignKey(r => r.TablePositionId);

        modelBuilder.Entity<TablePositionReader>().HasKey(r => r.Id);

        modelBuilder.Entity<TablePositionReader>().Property(r => r.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<TablePositionReader>()
            .HasOne(r => r.TablePosition)
            .WithMany(t => t.Readers)
            .HasForeignKey(r => r.TablePositionId);

        modelBuilder.Entity<Game>().HasKey(g => g.Id);
        modelBuilder.Entity<Game>().Property(g => g.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Game>().HasOne(g => g.Type).WithMany().HasForeignKey(g => g.TypeId);
        modelBuilder.Entity<Game>().HasMany(g => g.Hands).WithOne(h => h.Game).HasForeignKey(h => h.GameId);

        modelBuilder.Entity<GameType>().HasKey(gt => gt.Id);
        modelBuilder.Entity<GameType>().Property(gt => gt.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<GameType>().HasIndex(gt => gt.Name).IsUnique();

        modelBuilder.Entity<GameHand>().HasKey(h => h.Id);
        modelBuilder.Entity<GameHand>().Property(h => h.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<GameHand>().HasOne(h => h.Game).WithMany(g => g.Hands).HasForeignKey(h => h.GameId);
        modelBuilder.Entity<GameHand>().HasMany(h => h.Steps).WithOne(s => s.Hand).HasForeignKey(s => s.HandId);

        modelBuilder.Entity<HandStep>().HasKey(s => s.Id);
        modelBuilder.Entity<HandStep>().Property(s => s.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<HandStep>().HasOne(s => s.Hand).WithMany(h => h.Steps).HasForeignKey(s => s.HandId);
        modelBuilder.Entity<HandStep>().HasOne(s => s.FromPosition).WithMany().HasForeignKey(s => s.FromPositionId);
        modelBuilder.Entity<HandStep>().HasOne(s => s.ToPosition).WithMany().HasForeignKey(s => s.ToPositionId);
        modelBuilder.Entity<HandStep>().HasOne(s => s.Card).WithMany().HasForeignKey(s => s.CardId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true");
        }
    }
}
