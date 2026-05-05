using HMS.Shared.DTOs.ServiceDTOs;
using HMS.Shared.QueryParameters;
using HMS.Shared.Responses;

namespace HMS.Services.Abstraction
{
    public interface IHotelServicesManagementService
    {
        Task<GenericResponse<IEnumerable<HotelServicesDTO>>> GetAllHotelServicesForGuestAsync();

        Task<GenericResponse<HotelServicesDTO>> GetHotelServiceByIdForGuestAsync(int serviceId);

        Task<GenericResponse<IEnumerable<HotelServicesAdminDTO>>> GetAllServiceForAdminAsync(
            bool? IsAvailable
        );

        Task<GenericResponse<bool>> CreateHotelServiceByAdminAsync(
            CreateOrUpdateHotelServiceDTO createHotelServiceDTO
        );

        Task<GenericResponse<bool>> UpdateHotelServiceByAdminAsync(
            int id,
            CreateOrUpdateHotelServiceDTO updateHotelServiceDTO
        );

        Task<GenericResponse<bool>> DeleteHotelServiceByAdminAsync(int id);

        Task<GenericResponse<bool>> CreateServiceRequestByGuestAsync(
            CreateServiceRequestByGuestDTO createServiceRequestByGuestDTO
        );

        Task<
            GenericResponse<IEnumerable<ServiceRequestDTO>>
        > GetAllServiceRequestsForCurrentGuestAsync(string guestId);

        Task<GenericResponse<bool>> DeleteServiceRequestByGuestAsync(
            int serviceRequestId,
            string guestId
        );

        Task<
            GenericResponse<IEnumerable<ServiceRequestForAdminDTO>>
        > GetAllServiceRequestsForAdminAsync(ServiceRequestQueryParam? queryParam);

        Task<GenericResponse<ServiceRequestForAdminDTO>> GetServiceRequestByIdForAdminAsync(
            int serviceRequestId
        );

        Task<GenericResponse<bool>> AssignStaffToServiceRequestByAdminAsync(
            int serviceRequestId,
            string staffId
        );

        Task<GenericResponse<bool>> UpdateServiceRequestStatusByStaffAsync(
            int serviceRequestId,
            string staffUserId,
            string status
        );
    }
}
