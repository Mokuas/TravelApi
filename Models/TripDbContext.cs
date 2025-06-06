using Microsoft.EntityFrameworkCore;

namespace Trip.Models;

public class TripDbContext : DbContext
{
    public TripDbContext(DbContextOptions<TripDbContext> options) : base(options)
    {
    }
    
    public DbSet<Client> Client { get; set; }
    public DbSet<Trip> Trip { get; set; }
    public DbSet<Country> Country { get; set; }
    public DbSet<ClientTrip> Client_Trip { get; set; }
    public DbSet<CountryTrip> Country_Trip { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trip>().ToTable("Trip");
        modelBuilder.Entity<Client>().ToTable("Client");
        modelBuilder.Entity<Country>().ToTable("Country");
        modelBuilder.Entity<ClientTrip>().ToTable("Client_Trip");
        modelBuilder.Entity<CountryTrip>().ToTable("Country_Trip");

        modelBuilder.Entity<Client>().HasKey(c => c.IdClient);
        modelBuilder.Entity<Trip>().HasKey(t => t.IdTrip);
        modelBuilder.Entity<Country>().HasKey(c => c.IdCountry);
        modelBuilder.Entity<ClientTrip>().HasKey(ct => new { ct.IdClient, ct.IdTrip });
        modelBuilder.Entity<CountryTrip>().HasKey(ct => new { ct.IdCountry, ct.IdTrip });
        
        modelBuilder.Entity<ClientTrip>()
            .HasOne(ct => ct.Client)
            .WithMany(c => c.ClientTrips)
            .HasForeignKey(ct => ct.IdClient);

        modelBuilder.Entity<ClientTrip>()
            .HasOne(ct => ct.Trip)
            .WithMany(t => t.ClientTrips)
            .HasForeignKey(ct => ct.IdTrip);
        
        modelBuilder.Entity<CountryTrip>()
            .HasOne(ct => ct.Country)
            .WithMany(c => c.CountryTrips)
            .HasForeignKey(ct => ct.IdCountry);

        modelBuilder.Entity<CountryTrip>()
            .HasOne(ct => ct.Trip)
            .WithMany(t => t.CountryTrips)
            .HasForeignKey(ct => ct.IdTrip);
    }
}
