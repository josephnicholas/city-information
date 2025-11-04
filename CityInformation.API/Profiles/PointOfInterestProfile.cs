using AutoMapper;

namespace CityInformation.API.Profiles;

public class PointOfInterestProfile : Profile
{
    public PointOfInterestProfile()
    {
        CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
        CreateMap<Models.PointOfInterestCreateDto, Entities.PointOfInterest>();
    }
}