using AutoMapper;
using CityInformation.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace CityInformation.API.Controllers;

using System.Text.Json;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Models;
using Serilog;

[ApiController]
// [Authorize]
[Route("api/cities")] // Controller level attribute, Route + URI
[ApiVersion(1)]
[ApiVersion(2)]
// [controller] -> can be used which maps to the prefix "Cities"
// in GetCities
public class CitiesController(ICityInfoRepository repository, IMapper mapper) : ControllerBase
{
    private const int MaximumPageSize = 20;

    [HttpGet] // No need to define a route since already defined at controller level
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(
        [FromQuery] string? name,
        [FromQuery] string? searchQuery,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageSize > MaximumPageSize)
        {
            pageSize = MaximumPageSize;
        }

        var (cityEntities, paginationMetadata) = await repository
            .GetCitiesByNameAsync(name, searchQuery, pageNumber, pageSize);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
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