using ComedyEvents.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComedyEvents.Context
{
    public class EventContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public EventContext(DbContextOptions options, IConfiguration configration) : base (options)
        {
            _configuration = configration;
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Comedian> Comedians { get; set; }
        public DbSet<Gig> Gigs { get; set; }
        public DbSet<Venue> Venues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("ComedyEvent"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Event>()
                .HasData(new
                {
                    EventId = 1,
                    EventName = "Funny Comedy Night",
                    EventDate = new DateTime(2019, 05, 19),
                    VenueId = 1
                });

            builder.Entity<Venue>()
                .HasData(new
                {
                    VenueId = 1,
                    VenueName = "Mohegan Sun",
                    Street = "123 Main Street",
                    City = "Wilkes Barre",
                    State = "PA",
                    ZipCode = "18702",
                    Seating = 125,
                    ServesAlcohol = true
                });

            builder.Entity<Gig>()
                .HasData(new
                {
                    GigId = 1,
                    EventId = 1,
                    ComedianId = 1,
                    GigHeadline = "Pavols Funny Show",
                    GigLengthInMinutes = 60
                }, new
                {
                    GigId = 2,
                    EventId = 1,
                    ComedianId = 2,
                    GigHeadline = "Lifetime Of Fun",
                    GigLengthInMinutes = 45
                });

            builder.Entity<Comedian>()
                .HasData(new
                {
                    ComedianId = 1,
                    FirstName = "Pavol",
                    LastName = "Almasi",
                    ContactPhone = "111-222-3333"
                }, new
                {
                    ComedianId = 2,
                    FirstName = "Robin",
                    LastName = "Williams",
                    ContactPhone = "444-555-6666"
                });
        }

    }
}
