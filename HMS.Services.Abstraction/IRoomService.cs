using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.Responses;

namespace HMS.Services.Abstraction
{
    public interface IRoomService
    {
        Task<GenericResponse<IEnumerable<RoomDTO>>> GetAllRoomsForGuestAsync(string? roomType, string? sort);

        Task<GenericResponse<RoomDetailsDTO>> GetRoomDetailstAsync(int roomdId);
    }
}
