using AutoMapper;
using HMS.Core.Entities.ServiceEntities;
using HMS.Shared.DTOs.ServiceDTOs;

namespace HMS.Services.Profiles.ServicesProfiles
{
    internal class ServicesProfile : Profile
    {
        public ServicesProfile()
        {
            CreateMap<Service, HotelServicesDTO>();

            CreateMap<Service, HotelServicesAdminDTO>();

            CreateMap<CreateOrUpdateHotelServiceDTO, Service>();
        }
    }
}
