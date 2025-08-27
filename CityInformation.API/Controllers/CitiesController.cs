namespace CityInformation.API.Controllers;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models;

[ApiController]
[Route("api/cities")] // Controller level attribute, Route + URI
                                    // [controller] -> can be used which maps to the prefix "Cities"
                                    // in GetCities
public class CitiesController : ControllerBase {
    private readonly CitiesDataStore _citiesDataStore;

    public CitiesController(CitiesDataStore citiesDataStore) {
        _citiesDataStore = citiesDataStore ??  throw new ArgumentNullException(nameof(citiesDataStore));
    }
    
    [HttpGet] // No need to define a route since already defined at controller level
    public ActionResult<List<CityDto>> GetCities() {
        return Ok(_citiesDataStore.Cities);
    }

    [HttpGet("{id:int}")] // routing will be enclosed by curly braces
    public ActionResult<CityDto> GetCiy(int id) {
        var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == id);
        if (city is null)
        {
            return NotFound();
        }
        
        return Ok(city);
    }
}
