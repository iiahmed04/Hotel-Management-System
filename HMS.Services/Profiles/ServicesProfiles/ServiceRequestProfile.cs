using AutoMapper;
using HMS.Core.Entities.ServiceEntities;
using HMS.Shared.DTOs.ServiceDTOs;

namespace HMS.Services.Profiles.ServicesProfiles
{
    internal class ServiceRequestProfile : Profile
    {
        public ServiceRequestProfile()
        {
            CreateMap<CreateServiceRequestByGuestDTO, ServiceRequest>();

            CreateMap<ServiceRequest, ServiceRequestDTO>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(
                    dest => dest.RequestedAt,
                    opt => opt.MapFrom(src => src.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"))
                );
        }
    }
}
