using System.Security.Claims;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.ServiceDTOs;
using HMS.Shared.QueryParameters;
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

        // GET: BaseUrl/api/ServiceRequests
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<
            GenericResponse<IEnumerable<ServiceRequestForAdminDTO>>
        > GetAllServiceRequestsForAdmin([FromQuery] ServiceRequestQueryParam? queryParam)
        {
            var result = _hotelServicesManagementService.GetAllServiceRequestsForAdminAsync(
                queryParam
            );
            return HandleResponse(result.Result);
        }

        //GET: BaseUrl/api/ServiceRequests/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<
            ActionResult<GenericResponse<ServiceRequestForAdminDTO>>
        > GetServiceRequestByIdForAdmin(int id)
        {
            var result = await _hotelServicesManagementService.GetServiceRequestByIdForAdminAsync(
                id
            );
            return HandleResponse(result);
        }

        // PUT: BaseUrl/api/ServiceRequests/{id}/assign-staff
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/assign-staff")]
        public async Task<ActionResult<GenericResponse<bool>>> AssignStaffToServiceRequestByAdmin(
            [FromRoute] int id,
            [FromBody] string staffId
        )
        {
            var result =
                await _hotelServicesManagementService.AssignStaffToServiceRequestByAdminAsync(
                    id,
                    staffId
                );
            return HandleResponse(result);
        }

        //PUT: BaseUrl/api/ServiceRequests/{id}/update-status
        [Authorize(Roles = "Staff")]
        [HttpPut("{id}/update-status")]
        public async Task<ActionResult<GenericResponse<bool>>> UpdateServiceRequestStatusByStaff(
            [FromRoute] int id,
            [FromQuery] string status
        )
        {
            var staffUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result =
                await _hotelServicesManagementService.UpdateServiceRequestStatusByStaffAsync(
                    id,
                    staffUserId!,
                    status
                );

            return HandleResponse(result);
        }
    }
}
