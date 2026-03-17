using HMS.Services.Abstraction;
using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        //GET : BaseUrl/api/Rooms/public
        [HttpGet("public")]
        public async Task<ActionResult<GenericResponse<IEnumerable<RoomDTO>>>> GetAllRooms(string? roomType, string? sort)
        {
            var result = await _roomService.GetAllRoomsForGuestAsync(roomType, sort);
            return Ok(result);
        }

        //GET : BaseUrl/Rooms/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GenericResponse<RoomDetailsDTO>>> GetRoomDetails(int id)
        {
            var result = await _roomService.GetRoomDetailstAsync(id);
            return Ok(result);
        }
    }
}
