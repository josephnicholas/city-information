using CityInformation.API.DBContext;
using CityInformation.API.Entities;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

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
            var result = await dbContext.Cities
                .Include(c => c.PointsOfInterest)
                .Where(c => c.Id == cityId)
                .FirstOrDefaultAsync();

            return result;
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

    public async Task<bool> CityExistsAsync(int cityId)
    {
        return await dbContext.Cities.AnyAsync(c => c.Id == cityId);
    }

    public Task AddPointOfInterestAsync(int cityId, PointOfInterest pointOfInterest)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() >= 0;
    }

    public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
    {
        var city = await GetCityAsync(cityId, false);
        city?.PointsOfInterest.Add(pointOfInterest);
    }

    public void DeletePointOfInterest(PointOfInterest pointOfInterest)
    {
        dbContext.PointsOfInterest.Remove(pointOfInterest);
    }

    public async Task<IEnumerable<City>> GetCitiesByNameAsync(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return await GetCitiesAsync();
        }

        return await dbContext.Cities
            .Where(c => c.Name == name.Trim())
            .OrderBy(c => c.Name)
            .ToArrayAsync();
    }
}