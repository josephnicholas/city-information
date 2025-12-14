using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

namespace CityInformation.API.Controllers;
using Models;
using Services;

[Route("api/cities/{id:int}/pointsofinterest")]
[Authorize(Policy = "MustBeFromDumagueteCity")]
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
        var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;
        if (!await _cityInfoRepository.CityNameMatchesCityIdAsync(cityName, id))
        {
            return Forbid();
        }
        
        if (!await _cityInfoRepository.CityExistsAsync(id))
        {
            _logger.LogInformation("City with id {Id} wasn't found when accessing points of interest.", id);
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
            _logger.LogInformation("City with id {Id} wasn't found when creating points of interest.", id);
            return NotFound();
        }

        var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(dto);
        await _cityInfoRepository.AddPointOfInterestForCityAsync(id, finalPointOfInterest);
        await _cityInfoRepository.SaveChangesAsync();

        var mappedBackDto = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);

        return CreatedAtRoute("GetPointOfInterest",
            new
            {
                id,
                pointOfInterestId = mappedBackDto.Id
            }, mappedBackDto);
    }

    // Full update
    [HttpPut("{pointOfInterestId:int}")]
    public async Task<ActionResult> UpdatePointOfInterest(int id, int pointOfInterestId, PointOfInterestUpdateDto dto)
    {
        if (!await _cityInfoRepository.CityExistsAsync(id))
        {
            _logger.LogInformation("City with id {Id} wasn't found when creating points of interest.", id);
            return NotFound();
        }

        var poiEntity = await _cityInfoRepository.GetPointOfInterestAsync(id, pointOfInterestId);
        if (poiEntity is null)
        {
            return NotFound();
        }

        // This will overwrite the values in poiEntity with the values in dto
        _mapper.Map(dto, poiEntity);
        await _cityInfoRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{pointOfInterestId:int}")]
    public async Task<ActionResult> PartiallyUpdatePointOfInterest(int id, int pointOfInterestId,
        JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
    {
        if (!await _cityInfoRepository.CityExistsAsync(id))
        {
            _logger.LogInformation($"City with id {id} wasn't found when partially updating points of interest.");
            return NotFound();
        }

        var poiEntity = await _cityInfoRepository.GetPointOfInterestAsync(id, pointOfInterestId);
        if (poiEntity is null)
        {
            return NotFound();
        }

        var pointOfIntrestToPatch = _mapper.Map<PointOfInterestUpdateDto>(poiEntity);

        patchDocument.ApplyTo(pointOfIntrestToPatch, ModelState); // pass in the model state if there are any problems

        // JsonPatchDocument validation
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // model level validation
        if (!TryValidateModel(patchDocument))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(pointOfIntrestToPatch, poiEntity); // poiEntity now contains the changes applied by the pointOfIntrestToPatch

        await _cityInfoRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{pointOfInterestId:int}")]
    public async Task<ActionResult> DeletePointOfInterest(int id, int pointOfInterestId)
    {
        if (!await _cityInfoRepository.CityExistsAsync(id))
        {
            _logger.LogInformation($"City with id {id} wasn't found when deleting points of interest.");
            return NotFound();
        }

        var poiEntity = await _cityInfoRepository.GetPointOfInterestAsync(id, pointOfInterestId);
        if (poiEntity is null)
        {
            return NotFound();
        }

        _cityInfoRepository.DeletePointOfInterest(poiEntity);
        await _cityInfoRepository.SaveChangesAsync();

        _mailService.Send("Point of interest deleted.",
            $"Point of interest {poiEntity.Name} with id {poiEntity.Id} has been successfully deleted.");
        return NoContent();
    }
}