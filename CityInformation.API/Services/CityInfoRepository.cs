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

    public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesByNameAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
    {
        // Remove this since this violate the purpose of paging
        // if (string.IsNullOrEmpty(name) && string.IsNullOrWhiteSpace(searchQuery))
        // {
        //     return await GetCitiesAsync();
        // }

        // collection to start from
        var citiesCollection = dbContext.Cities as IQueryable<City>; // IQueryable enables the Where, OrderBy, etc. clauses

        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            citiesCollection = citiesCollection.Where(c => c.Name == name);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            citiesCollection = citiesCollection.Where(c => c.Name.Contains(searchQuery)
            || (!string.IsNullOrEmpty(c.Description) && c.Description.Contains(searchQuery)));
        }

        var totalItemCount = await citiesCollection.CountAsync();
        var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

        var page = await citiesCollection
            .OrderBy(c => c.Name)
            .Skip(pageSize * (pageNumber - 1)) // skip the amount of items in the prev page
            .Take(pageSize) // number of elements to return
            .ToArrayAsync(); // Execution on databsae commands is done here

        return (page, paginationMetadata);
    }
}

// Example page skip
// 10 pageSize
// 2 pageNumber
// 10 * (2 - 1) -> number of items to skip(10)