namespace CityInformation.API.DBContext;

using Entities;
using Microsoft.EntityFrameworkCore;

public class CityInfoContext(DbContextOptions<CityInfoContext> options) : DbContext(options) {
    public DbSet<City> Cities { get; set; }
    public DbSet<PointOfInterest> PointsOfInterest { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<City>().HasData(
            new City("New York City")
            {
                Id = 1,
                Description = "The Big Apple"
            },
            new City("Dumaguete City")
            {
                Id = 2,
                Description = "Silliman University"
            },
            new City("Paris")
            {
                Id = 3,
                Description = "The Eiffel Tower"
            }
        );

        modelBuilder.Entity<PointOfInterest>().HasData(
            new PointOfInterest("Central Park")
            {
                Id = 1,
                CityId = 1,
                Description = "The one where the bow bridge is located"
            },
            new PointOfInterest("Brooklyn Bridge")
            {
                Id = 2,
                CityId = 1,
                Description = "The historical bridge in New York City"
            },
            new PointOfInterest("Silliman University")
            {
                Id = 3,
                CityId = 2,
                Description = "The historical school in Dumaguete City"
            }
        );
        
        base.OnModelCreating(modelBuilder);
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    //     optionsBuilder.UseSqlite("Data Source=city-info.db");
    //     base.OnConfiguring(optionsBuilder);
    // }
}
