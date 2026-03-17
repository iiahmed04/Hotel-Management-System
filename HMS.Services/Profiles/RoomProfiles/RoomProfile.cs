using AutoMapper;
using HMS.Core.Entities.RoomEntities;
using HMS.Shared.DTOs.RoomDTOs;

namespace HMS.Services.Profiles.RoomProfiles
{
    internal class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Room, RoomDTO>();

            CreateMap<Room, RoomDetailsDTO>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.RoomImages.Select(i => i.ImageUrl)));
        }
    }
}
