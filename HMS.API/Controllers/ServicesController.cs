using HMS.Services.Abstraction;
using HMS.Shared.DTOs.ServiceDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    public class ServicesController : ApiBaseController
    {
        private readonly IHotelServicesManagementService _hotelServicesManagementService;

        public ServicesController(IHotelServicesManagementService hotelServicesManagementService)
        {
            _hotelServicesManagementService = hotelServicesManagementService;
        }

        // GET: BaseUrl/api/Services
        [Authorize(Roles = "Guest")]
        [HttpGet]
        public async Task<
            ActionResult<GenericResponse<IEnumerable<HotelServicesDTO>>>
        > GetAllHotelServicesForGuest(string? name)
        {
            var result = await _hotelServicesManagementService.GetAllHotelServicesForGuestAsync();
            return HandleResponse(result);
        }

        // GET : BaseUrl/api/Services/{id}
        [Authorize(Roles = "Guest")]
        [HttpGet("{id}")]
        public async Task<
            ActionResult<GenericResponse<HotelServicesDTO>>
        > GetHotelServiceByIdForGuestAsync([FromRoute] int id)
        {
            var result = await _hotelServicesManagementService.GetHotelServiceByIdForGuestAsync(id);
            return HandleResponse(result);
        }

        // GET : BaseUrl/api/Services/admin
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<
            ActionResult<GenericResponse<IEnumerable<HotelServicesAdminDTO>>>
        > GetAllServicesForAdmin(bool? IsAvailable)
        {
            var result = await _hotelServicesManagementService.GetAllServiceForAdminAsync(
                IsAvailable
            );
            return HandleResponse(result);
        }

        // POST : BaseUrl/api/Services
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<GenericResponse<bool>>> CreateHotelServiceByAdmin(
            [FromBody] CreateOrUpdateHotelServiceDTO createHotelServiceDTO
        )
        {
            var result = await _hotelServicesManagementService.CreateHotelServiceByAdminAsync(
                createHotelServiceDTO
            );
            return HandleResponse(result);
        }

        // PUT : BaseUrl/api/Services/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<GenericResponse<bool>>> UpdateHotelServiceByAdmin(
            [FromRoute] int id,
            [FromBody] CreateOrUpdateHotelServiceDTO updateHotelServiceDTO
        )
        {
            var result = await _hotelServicesManagementService.UpdateHotelServiceByAdminAsync(
                id,
                updateHotelServiceDTO
            );
            return HandleResponse(result);
        }
    }
}
