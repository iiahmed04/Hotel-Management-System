using HMS.Core.Entities.IdentityEntities;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.AuthDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace HMS.Services.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<HotelUser> _userManager;

        public AuthenticationService(UserManager<HotelUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<GenericResponse<UserDTO>> RegisterUserAsync(RegisterDTO registerData)
        {
            var genericResponse = new GenericResponse<UserDTO>();

            if (registerData is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "No Data to register new Guest provided";

                return genericResponse;
            }

            var userEmailExist = await _userManager.FindByEmailAsync(registerData.Email);
            if (userEmailExist is not null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "This is already existed email";
                return genericResponse;
            }

            var hotelUser = new HotelUser
            {
                FullName = registerData.FullName,
                Email = registerData.Email,
                PhoneNumber = registerData.PhoneNumber,
                IsActive = true,
                UserName = registerData.Email.Split("@")[0],
                CreatedAt = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(hotelUser, registerData.Password);

            await _userManager.AddToRoleAsync(hotelUser, "Guest");

            if (!result.Succeeded)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = string.Join("|", result.Errors.Select(e => e.Description));

                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success to register new Guest";
            genericResponse.Data = new UserDTO
            {
                Email = registerData.Email,
                FullName = registerData.FullName,
                Token = "Token"
            };

            return genericResponse;

        }

        public async Task<GenericResponse<UserDTO>> LoginAsync(LoginDTO loginData)
        {
            var genericResponse = new GenericResponse<UserDTO>();

            if (loginData is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "No Data to login in system";

                return genericResponse;
            }

            var user = await _userManager.FindByEmailAsync(loginData.Email);
            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status401Unauthorized;
                genericResponse.Message = "Ivalid Email or Password";
                return genericResponse;
            }

            if (!user.IsActive)
            {
                genericResponse.StatusCode = StatusCodes.Status403Forbidden;
                genericResponse.Message = "You are not active in system , to activate send to support";
                return genericResponse;
            }

            var checkPassword = await _userManager.CheckPasswordAsync(user, loginData.Password);
            if (checkPassword == false)
            {
                genericResponse.StatusCode = StatusCodes.Status401Unauthorized;
                genericResponse.Message = "Ivalid Email or Password";
                return genericResponse;
            }

            var loginUser = new UserDTO
            {
                Email = loginData.Email,
                FullName = user.FullName,
                Token = "Token"
            };

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Login successfully";
            genericResponse.Data = loginUser;

            return genericResponse;
        }

    }
}
