using AutoMapper;
using HMS.Core.Entities.RoomEntities;
using HMS.Shared.DTOs.RoomDTOs;
using Microsoft.Extensions.Configuration;

namespace HMS.Services.Profiles.RoomProfiles
{
    internal class RoomImageResolver : IValueResolver<Room, RoomDetailsDTO, List<string>>
    {
        private readonly IConfiguration _configuration;

        public RoomImageResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> Resolve(Room source, RoomDetailsDTO destination, List<string> destMember, ResolutionContext context)
            => source.RoomImages.Select(R => $"{_configuration["URLs:BaseUrl"]}/images/rooms/{R.ImageUrl}")
            .ToList();
    }
}
