using HMS.Shared.DTOs.ServiceDTOs;
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
    }
}
