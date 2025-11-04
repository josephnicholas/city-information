using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace CityInformation.API.Controllers;

using System.Drawing;
using Microsoft.AspNetCore.JsonPatch;
using Models;
using Services;

[Route("api/cities/{id:int}/pointsofinterest")]
[ApiController]
public class PointsOfInterestController : Controller
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;
    private readonly ICityInfoRepository _cityInfoRepository;
    private readonly IMapper _mapper;

    // Constructor injection
    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService,
        ICityInfoRepository cityInfoRepository, IMapper mapper)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointOfInterests(int id)
    {
        if (!await _cityInfoRepository.CityExistsAsync(id))
        {
            _logger.LogInformation($"City with id {id} wasn't found when accessing points of interest.");
            return NotFound();
        }

        var pointsOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestAsync(id);
        return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
    }

    [HttpGet("{pointOfInterestId:int}", Name = "GetPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int id, int pointOfInterestId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(id))
        {
            _logger.LogInformation($"City with id {id} wasn't found when accessing points of interest.");
            return NotFound();
        }
        var pointOfInterest = await _cityInfoRepository.GetPointOfInterestAsync(id, pointOfInterestId);

        if (pointOfInterest is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
    }

    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int id, PointOfInterestCreateDto dto)
    {
        if (!await _cityInfoRepository.CityExistsAsync(id))
        {
            _logger.LogInformation($"City with id {id} wasn't found when creating points of interest.");
            return NotFound();
        }

        var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(dto);
        await _cityInfoRepository.AddPointOfInterestForCityAsync(id, finalPointOfInterest);
        await _cityInfoRepository.SaveChangesAsync();

        var mappedBackDto = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);

        return CreatedAtRoute("GetPointOfInterest",
            new
            {
                id = id,
                pointOfInterestId = mappedBackDto.Id
            }, mappedBackDto);
    }

    // // Full update
    // [HttpPut("{pointOfInterestId:int}")]
    // public ActionResult UpdatePointOfInterest(int id, int pointOfInterestId, PointOfInterestUpdateDto dto)
    // {
    //     var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == id);
    //     if (city is null)
    //     {
    //         return NotFound();
    //     }

    //     var poi = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointOfInterestId);
    //     if (poi is null)
    //     {
    //         return NotFound();
    //     }

    //     poi.Name = dto.Name;
    //     poi.Description = dto.Description;

    //     return NoContent();
    // }

    // [HttpPatch("{pointOfInterestId:int}")]
    // public ActionResult PartiallyUpdatePointOfInterest(int id, int pointOfInterestId,
    //     JsonPatchDocument<PointOfInterestUpdateDto> dto)
    // {
    //     var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == id);
    //     if (city is null)
    //     {
    //         return NotFound();
    //     }

    //     var poi = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointOfInterestId);
    //     if (poi is null)
    //     {
    //         return NotFound();
    //     }

    //     var poiPatch = new PointOfInterestUpdateDto
    //     {
    //         Name = poi.Name,
    //         Description = poi.Description,
    //     };

    //     dto.ApplyTo(poiPatch, ModelState); // pass in the model state if there are any problems

    //     // JsonPatchDocument validation
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(ModelState);
    //     }

    //     // model level validation
    //     if (!TryValidateModel(poiPatch))
    //     {
    //         return BadRequest(ModelState);
    //     }

    //     poi.Name = poiPatch.Name;
    //     poi.Description = poiPatch.Description;

    //     return NoContent();
    // }

    // [HttpDelete("{pointOfInterestId:int}")]
    // public ActionResult DeletePointOfInterest(int id, int pointOfInterestId)
    // {
    //     var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == id);
    //     if (city is null)
    //     {
    //         return NotFound();
    //     }

    //     var poi = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointOfInterestId);
    //     if (poi is null)
    //     {
    //         return NotFound();
    //     }

    //     city.PointsOfInterest.Remove(poi);

    //     _mailService.Send("Point of interest deleted.",
    //         $"Point of interest {poi.Name} with id {poi.Id} has been successfully deleted.");
    //     return NoContent();
    // }
}