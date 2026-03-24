using HMS.Shared.DTOs.AuthDTOs;
using HMS.Shared.Responses;

namespace HMS.Services.Abstraction
{
    public interface IAuthenticationService
    {
        Task<GenericResponse<UserDTO>> RegisterUserAsync(RegisterDTO registerData);
        Task<GenericResponse<UserDTO>> LoginAsync(LoginDTO loginData);
    }
}
