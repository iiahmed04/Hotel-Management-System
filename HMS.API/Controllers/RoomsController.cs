using HMS.Services.Abstraction;
using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.QueryParameters;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{

    public class RoomsController : ApiBaseController
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        //GET : BaseUrl/api/Rooms/public
        [HttpGet("public")]
        public async Task<ActionResult<GenericResponse<IEnumerable<RoomDTO>>>> GetAllGuestRooms(string? roomType, string? sort)
        {
            var result = await _roomService.GetAllRoomsForGuestAsync(roomType, sort);
            return HandleResponse(result);
        }

        //GET : BaseUrl/Rooms/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GenericResponse<RoomDetailsDTO>>> GetRoomDetails(int id)
        {
            var result = await _roomService.GetRoomDetailstAsync(id);
            return HandleResponse(result);
        }

        //Get : BaseUrl/Rooms/admin
        [HttpGet("admin")]
        public async Task<ActionResult<GenericResponse<IEnumerable<AdminRoomDTO>>>> GetAllAdminRooms([FromQuery] RoomQueryParam? roomQueryParam)
        {
            var result = await _roomService.GetAllRoomsForAdminOrStaffAsync(roomQueryParam);
            return HandleResponse(result);
        }

        //POST : BaseUrl/Rooms
        [HttpPost]
        public async Task<ActionResult<GenericResponse<bool>>> CreateRoom([FromBody] CreateRoomDTO createRoomDTO)
        {
            var result = await _roomService.CreateRoomAsync(createRoomDTO);
            return HandleResponse(result);

        }

        //PUT : BaseUrl/api/Rooms/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<GenericResponse<bool>>> UpdateRoom([FromRoute] int id, [FromBody] UpdateRoomDTO updateRoomDTO)
        {
            var result = await _roomService.UpdateRoomAsync(id, updateRoomDTO);
            return HandleResponse(result);
        }

        //DELETE : BaseUrl/api/Rooms/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteRoom([FromRoute] int id)
        {
            var result = await _roomService.DeleteRoomAsync(id);
            return HandleResponse(result);
        }

        //POST : BaseUrl/api/Rooms/{id}/images
        [HttpPost("{id}/images")]
        public async Task<ActionResult<GenericResponse<bool>>> UploadRoomImages([FromRoute] int id, [FromForm] List<IFormFile> files)
        {
            var result = await _roomService.UploadRoomImagesAsync(id, files);
            return HandleResponse(result);
        }

        //DELETE : BaseUrl/api/Rooms/{id}/images/imageId
        [HttpDelete("{id}/images/{imageId}")]
        public async Task<ActionResult<GenericResponse<bool>>> DeleteRoomImages([FromRoute] int id, [FromRoute] int imageId)
        {
            var result = await _roomService.DeleteRoomImageAsync(id, imageId);
            return HandleResponse(result);
        }
    }
}
