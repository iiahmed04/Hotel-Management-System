using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.QueryParameters;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;

namespace HMS.Services.Abstraction
{
    public interface IRoomService
    {
        Task<GenericResponse<IEnumerable<RoomDTO>>> GetAllRoomsForGuestAsync(string? roomType, string? sort);

        Task<GenericResponse<RoomDetailsDTO>> GetRoomDetailstAsync(int roomdId);

        Task<GenericResponse<IEnumerable<AdminRoomDTO>>> GetAllRoomsForAdminOrStaffAsync(RoomQueryParam? roomQueryParam);

        Task<GenericResponse<bool>> CreateRoomAsync(CreateRoomDTO createRoomDTO);

        Task<GenericResponse<bool>> UpdateRoomAsync(int roomId, UpdateRoomDTO updateRoomDTO);

        Task<GenericResponse<bool>> DeleteRoomAsync(int roomId);

        Task<GenericResponse<bool>> UploadRoomImagesAsync(int roomId, List<IFormFile> files);

        Task<GenericResponse<bool>> DeleteRoomImageAsync(int roomId, int imageId);
    }
}
