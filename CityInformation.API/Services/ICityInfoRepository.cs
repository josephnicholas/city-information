using CityInformation.API.Entities;

namespace CityInformation.API.Services;

public interface ICityInfoRepository
{
    public Task<IEnumerable<City>> GetCitiesAsync();
    public Task<City?> GetCityAsync(int cityId, bool includePoi);
    public Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId);
    public Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId);
}