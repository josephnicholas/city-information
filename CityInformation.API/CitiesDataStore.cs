namespace CityInformation.API;

using Models;

public class CitiesDataStore {
    public List<CityDto> Cities { get; set; }
    public static CitiesDataStore Current { get; } = new ();
    
    CitiesDataStore() {
        Cities = [
            new()
            {
                Id = 0,
                Name = "New York",
                Description = "New York",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new()
                    {
                        Id = 0,
                        Name = "Central Park",
                        Description = "The most visited park in NYC"
                    },
                    new()
                    {
                        Id = 1,
                        Name = "Empire State Building",
                        Description = "The tallest building in NYC"
                    }
                }
            },
            new()
            {
                Id = 1,
                Name = "Dumaguete City",
                Description = "My Hometown",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new()
                    {
                        Id = 0,
                        Name = "Rizal Boulevard",
                        Description = "The best place to exercise and do leisure activities"
                    },
                    new()
                    {
                        Id = 1,
                        Name = "Silliman University",
                        Description = "My Alma mater"
                    }
                }
            },
            new()
            {
                Id = 2,
                Name = "London",
                Description = "London",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new()
                    {
                        Id = 0,
                        Name = "London Eye",
                        Description = "The best place to visit"
                    }
                }
            }
        ];
    }
}
