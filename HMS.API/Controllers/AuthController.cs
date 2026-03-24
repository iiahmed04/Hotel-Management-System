using HMS.Services.Abstraction;
using HMS.Shared.DTOs.AuthDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    public class AuthController : ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        //POST : BaseUrl/api/Auth/register
        [HttpPost("register")]
        public async Task<ActionResult<GenericResponse<UserDTO>>> RegisterUser(RegisterDTO registerDTO)
        {
            var result = await _authenticationService.RegisterUserAsync(registerDTO);
            return HandleResponse(result);
        }

        //POST : BaseUrl/api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<GenericResponse<UserDTO>>> Login(LoginDTO loginDTO)
        {
            var result = await _authenticationService.LoginAsync(loginDTO);
            return HandleResponse(result);
        }
    }
}
