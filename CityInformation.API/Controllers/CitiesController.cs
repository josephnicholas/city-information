using CityInformation.API.Services;

namespace CityInformation.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models;

[ApiController]
[Route("api/cities")] // Controller level attribute, Route + URI
                                    // [controller] -> can be used which maps to the prefix "Cities"
                                    // in GetCities
public class CitiesController(ICityInfoRepository repository) : ControllerBase {
    [HttpGet] // No need to define a route since already defined at controller level
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
    {
        var cityEntities = await repository.GetCitiesAsync();
        var results = new List<CityWithoutPointsOfInterestDto>();
        foreach (var cityEntity in cityEntities)
        {
            results.Add(new()
            {
                Id = cityEntity.Id,
                Description = cityEntity.Description,
                Name = cityEntity.Name
            });
        }
        
        return Ok(results);
    }

    [HttpGet("{id:int}")] // routing will be enclosed by curly braces
    public async Task<ActionResult<CityDto>> GetCity(int id) {
        var city = await repository.GetCityAsync(id, false);
        if (city is null)
        {
            return NotFound();
        }
        return Ok(city);
    }
}
