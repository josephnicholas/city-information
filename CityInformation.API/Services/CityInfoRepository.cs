using CityInformation.API.DBContext;
using CityInformation.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInformation.API.Services;

public class CityInfoRepository(CityInfoContext dbContext) : ICityInfoRepository
{
    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await dbContext.Cities.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<City?> GetCityAsync(int cityId, bool includePoi)
    {
        if (includePoi)
        {
            return await dbContext.Cities
                .Include(c => c.PointsOfInterests)
                .Where(c => c.Id == cityId)
                .FirstOrDefaultAsync();
        }
        return await dbContext.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId)
    {
        return await dbContext.PointsOfInterest
            .Where(p => p.CityId == cityId)
            .ToListAsync();
    }

    public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId)
    {
        return await dbContext.PointsOfInterest
            .Where(p => p.Id == pointOfInterestId && p.CityId == cityId)
            .FirstOrDefaultAsync();
    }
}