using Microsoft.AspNetCore.Mvc;

namespace CityInformation.API.Controllers;

using Microsoft.AspNetCore.JsonPatch;
using Models;
using Services;

[Route("api/cities/{id:int}/pointsofinterest")]
[ApiController]
public class PointsOfInterestController : Controller {
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly CitiesDataStore _citiesDataStore;

    // Constructor injection
    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, CitiesDataStore citiesDataStore) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
    }

    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointOfInterests(int id) {
        try
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == id);
            if (city is null)
            {
                _logger.LogInformation($"City with id {id} wasn't found when accessing points of interest.");
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }
        catch (Exception e)
        {
            // Exceptions normally return a 500 server error when unhandled
            // careful wille get to the API consumers
            // don't write stack trace
            _logger.LogCritical($"Exception caught while accessing points of interest with id {id}", e);
            return StatusCode(500, "A problem while handling your request.");
        }
    }

    [HttpGet("{pointOfInterestId:int}", Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int id, int pointOfInterestId) {
        var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == id);
        if (city is null)
        {
            return NotFound();
        }

        var poi = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointOfInterestId);
        if (poi is null)
        {
            return NotFound();
        }

        return Ok(poi);
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(int id, PointOfInterestCreateDto dto) {
        var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == id);
        if (city is null)
        {
            return NotFound();
        }

        // demo purposes
        var maxPointOfInterestId = _citiesDataStore.Cities
            .SelectMany(x => x.PointsOfInterest)
            .Max(x => x.Id);

        var poi = new PointOfInterestDto
        {
            Id = ++maxPointOfInterestId,
            Name = dto.Name,
            Description = dto.Description,
        };
        city.PointsOfInterest.Add(poi);

        return CreatedAtRoute("GetPointOfInterest",
        new
        {
            id = city.Id,
            pointOfInterestId = poi.Id
        },
        poi);
    }

    // Full update
    [HttpPut("{pointOfInterestId:int}")]
    public ActionResult UpdatePointOfInterest(int id, int pointOfInterestId, PointOfInterestUpdateDto dto) {
        var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == id);
        if (city is null)
        {
            return NotFound();
        }

        var poi = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointOfInterestId);
        if (poi is null)
        {
            return NotFound();
        }

        poi.Name = dto.Name;
        poi.Description = dto.Description;

        return NoContent();
    }

    [HttpPatch("{pointOfInterestId:int}")]
    public ActionResult PartiallyUpdatePointOfInterest(int id, int pointOfInterestId, JsonPatchDocument<PointOfInterestUpdateDto> dto) {
        var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == id);
        if (city is null)
        {
            return NotFound();
        }

        var poi = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointOfInterestId);
        if (poi is null)
        {
            return NotFound();
        }

        var poiPatch = new PointOfInterestUpdateDto
        {
            Name = poi.Name,
            Description = poi.Description,
        };

        dto.ApplyTo(poiPatch, ModelState); // pass in the model state if there are any problems

        // JsonPatchDocument validation
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // model level validation
        if (!TryValidateModel(poiPatch))
        {
            return BadRequest(ModelState);
        }

        poi.Name = poiPatch.Name;
        poi.Description = poiPatch.Description;

        return NoContent();
    }

    [HttpDelete("{pointOfInterestId:int}")]
    public ActionResult DeletePointOfInterest(int id, int pointOfInterestId) {
        var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == id);
        if (city is null)
        {
            return NotFound();
        }

        var poi = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointOfInterestId);
        if (poi is null)
        {
            return NotFound();
        }

        city.PointsOfInterest.Remove(poi);
        
        _mailService.Send("Point of interest deleted.", $"Point of interest {poi.Name} with id {poi.Id} has been successfully deleted.");
        return NoContent();
    }
}
