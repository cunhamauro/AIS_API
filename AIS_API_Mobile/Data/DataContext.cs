using AIS_API_Mobile.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection.Emit;

namespace AIS_API_Mobile.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Aircraft> Aircrafts { get; set; }

        public DbSet<Airport> Airports { get; set; }

        public DbSet<Flight> Flights { get; set; }

        public DbSet<TicketRecord> TicketRecords { get; set; }

        public DbSet<FlightRecord> FlightRecords { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Make IATA unique
            modelBuilder.Entity<Airport>()
                       .HasIndex(a => a.IATA)
                       .IsUnique()
                       .HasDatabaseName("IX_Airports_IATA");

            // Make price with two decimals
            modelBuilder.Entity<Ticket>()
                       .Property(t => t.Price)
                       .HasColumnType("decimal(18,2)");

            // Make price with two decimals
            modelBuilder.Entity<TicketRecord>()
                       .Property(t => t.TicketPrice)
                       .HasColumnType("decimal(18,2)");

            // Make database not generate IDs for ticket records
            modelBuilder.Entity<TicketRecord>()
                       .Property(e => e.Id)
                       .ValueGeneratedNever();

            // Make database not generate IDs for flight records
            modelBuilder.Entity<FlightRecord>()
                       .Property(e => e.Id)
                       .ValueGeneratedNever();

            // Convert Lists to Json to save in database:
            // Converter
            var valueConverter = new ValueConverter<List<string>, string>(
                 v => JsonConvert.SerializeObject(v), // Convert List<string> to JSON string for database persistance
                 v => JsonConvert.DeserializeObject<List<string>>(v) // Convert JSON string back to List<string>
            );

            // Comparer
            var valueComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2), // Compare two lists for equality
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // Create a combined hash code for the list
                c => c.ToList() // Clone the list to avoid references to the same list
            );

            // Configure Aircraft
            modelBuilder.Entity<Aircraft>()
                        .Property(x => x.Seats)
                        .HasConversion(valueConverter)
                        .Metadata.SetValueComparer(valueComparer);

            // Configure Flight
            modelBuilder.Entity<Flight>()
                        .Property(x => x.AvailableSeats)
                        .HasConversion(valueConverter)
                        .Metadata.SetValueComparer(valueComparer);

            base.OnModelCreating(modelBuilder);
        }
    }
}
