using AutoMapper;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CityInformation.API.Profiles;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
        CreateMap<Entities.City, Models.CityDto>();
    }
}