using CityInformation.API.Entities;

namespace CityInformation.API.Services;

public interface ICityInfoRepository
{
    public Task<IEnumerable<City>> GetCitiesAsync();
    public Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesByNameAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
    public Task<City?> GetCityAsync(int cityId, bool includePoi);
    public Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId);
    public Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId);
    public Task<bool> CityExistsAsync(int cityId);
    public Task AddPointOfInterestAsync(int cityId, PointOfInterest pointOfInterest);
    public Task<bool> SaveChangesAsync();
    public Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);
    public void DeletePointOfInterest(PointOfInterest pointOfInterest);
    public Task<bool> CityNameMatchesCityIdAsync(string? cityName, int cityId);
}