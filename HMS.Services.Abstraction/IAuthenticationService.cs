using HMS.Shared.DTOs.AuthDTOs;
using HMS.Shared.Responses;

namespace HMS.Services.Abstraction
{
    public interface IAuthenticationService
    {
        Task<GenericResponse<UserDTO>> RegisterUserAsync(RegisterDTO registerData);
        Task<GenericResponse<UserDTO>> LoginAsync(LoginDTO loginData);
        Task<GenericResponse<bool>> CreateStaffUserAsync(StaffUserDTO staffUser);
        Task<GenericResponse<IEnumerable<GetUserDTO>>> GettAllUsersForAdminAsync();
        Task<GenericResponse<bool>> ActivateUser(string userId);
        Task<GenericResponse<bool>> DeActivateUser(string userId);
        Task<GenericResponse<bool>> EmailExistAsync(string email);
        Task<GenericResponse<ProfileDTO>> GetProfileAsync(string userId);
    }
}
