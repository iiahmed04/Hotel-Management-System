using System.Security.Claims;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.ServiceDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    public class ServiceRequestsController : ApiBaseController
    {
        private readonly IHotelServicesManagementService _hotelServicesManagementService;

        public ServiceRequestsController(
            IHotelServicesManagementService hotelServicesManagementService
        )
        {
            _hotelServicesManagementService = hotelServicesManagementService;
        }

        // POST: api/ServiceRequest
        [Authorize(Roles = "Guest")]
        [HttpPost]
        public async Task<ActionResult<GenericResponse<bool>>> CreateServiceRequestByGuest(
            [FromBody] CreateServiceRequestByGuestDTO createServiceRequestByGuestDTO
        )
        {
            var result = await _hotelServicesManagementService.CreateServiceRequestByGuestAsync(
                createServiceRequestByGuestDTO
            );
            return HandleResponse(result);
        }

        // GET: api/ServiceRequests/my
        [Authorize(Roles = "Guest")]
        [HttpGet("my")]
        public async Task<
            ActionResult<GenericResponse<IEnumerable<ServiceRequestDTO>>>
        > GetAllServiceRequestForCurrentGuest()
        {
            var guestId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result =
                await _hotelServicesManagementService.GetAllServiceRequestsForCurrentGuestAsync(
                    guestId!
                );
            return HandleResponse(result);
        }

        // PUT : api/ServiceRequests/{id}/cancel
        [Authorize(Roles = "Guest")]
        [HttpPut("{id}/cancel")]
        public async Task<ActionResult<GenericResponse<bool>>> CancelServiceRequestByGuest(int id)
        {
            var guestId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _hotelServicesManagementService.DeleteServiceRequestByGuestAsync(
                id,
                guestId!
            );
            return HandleResponse(result);
        }
    }
}
