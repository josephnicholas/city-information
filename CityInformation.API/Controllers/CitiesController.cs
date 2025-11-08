using AutoMapper;
using CityInformation.API.Services;

namespace CityInformation.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models;

[ApiController]
[Route("api/cities")] // Controller level attribute, Route + URI
// [controller] -> can be used which maps to the prefix "Cities"
// in GetCities
public class CitiesController(ICityInfoRepository repository, IMapper mapper) : ControllerBase
{
    [HttpGet] // No need to define a route since already defined at controller level
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities([FromQuery] string? name)
    {
        var cityEntities = await repository.GetCitiesByNameAsync(name);
        return Ok(mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
    }

    [HttpGet("{id:int}")] // routing will be enclosed by curly braces
    public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
    {
        var city = await repository.GetCityAsync(id, includePointsOfInterest);
        if (city is null)
        {
            return NotFound();
        }

        return includePointsOfInterest
            ? Ok(mapper.Map<CityDto>(city))
            : Ok(mapper.Map<CityWithoutPointsOfInterestDto>(city));
    }
}