using Microsoft.AspNetCore.Mvc;

namespace CityInformation.API.Controllers;

using Microsoft.AspNetCore.JsonPatch;
using Models;

[Route("api/cities/{id:int}/pointsofinterest")]
[ApiController]
public class PointsOfInterestController : Controller {
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointOfInterests(int id) {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
        if (city is null) {
            return NotFound();
        }
        
        return Ok(city.PointsOfInterest);
    }

    [HttpGet("{pointOfInterestId:int}", Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int id, int pointOfInterestId) {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
        if (city is null) {
            return NotFound();
        }
        
        var poi = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointOfInterestId);
        if (poi is null) {
            return NotFound();
        }
        
        return Ok(poi);
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(int id, PointOfInterestCreateDto dto) {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
        if (city is null)
        {
            return NotFound();
        }
        
        // demo purposes
        var maxPointOfInterestId = CitiesDataStore.Current.Cities
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
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
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
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
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
        
        dto.ApplyTo(poiPatch,  ModelState); // pass in the model state if there are any problems

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
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == id);
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
        return NoContent();
    }
}
