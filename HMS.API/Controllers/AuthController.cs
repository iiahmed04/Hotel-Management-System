using HMS.Services.Abstraction;
using HMS.Shared.DTOs.AuthDTOs;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        //POST : BaseUrl/api/Auth/Create-Staff
        [Authorize(Roles = "Admin")]
        [HttpPost("Create-Staff")]
        public async Task<ActionResult<GenericResponse<bool>>> CreateStaffUser(StaffUserDTO staffUser)
        {
            var result = await _authenticationService.CreateStaffUserAsync(staffUser);
            return HandleResponse(result);
        }

        //GET : BaseUrl/api/Auth/Users
        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<ActionResult<GenericResponse<IEnumerable<GetUserDTO>>>> GetUsers()
        {
            var result = await _authenticationService.GettAllUsersForAdminAsync();
            return HandleResponse(result);
        }

        //PUT : BaseUrl/api/Auth/activate
        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id}/activate")]
        public async Task<ActionResult<GenericResponse<bool>>> ActivateUser([FromRoute] string id)
        {
            var result = await _authenticationService.ActivateUser(id);
            return HandleResponse(result);
        }
        //PUT : BaseUrl/api/Auth/activate
        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id}/deActivate")]
        public async Task<ActionResult<GenericResponse<bool>>> DeActivateUser([FromRoute] string id)
        {
            var result = await _authenticationService.DeActivateUser(id);
            return HandleResponse(result);
        }

        //GET : BaseUrl/api/Auth/emailExist
        [HttpGet("emailExists")]
        public async Task<ActionResult<GenericResponse<bool>>> EmailExist(string email)
        {
            var result = await _authenticationService.EmailExistAsync(email);
            return HandleResponse(result);
        }

        //GET : BaseUrl/api/Auth/profile
        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<GenericResponse<ProfileDTO>>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _authenticationService.GetProfileAsync(userId);
            return HandleResponse(result);
        }
    }
}
