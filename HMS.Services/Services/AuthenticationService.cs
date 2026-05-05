using HMS.Core.Entities.IdentityEntities;
using HMS.Services.Abstraction;
using HMS.Shared.DTOs.AuthDTOs;
using HMS.Shared.Messages;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HMS.Services.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<HotelUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthenticationService(UserManager<HotelUser> userManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
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

            var email = new Email
            {
                To = registerData.Email,
                Subject = $"Welcome {registerData.FullName} To Our HotelSystem APP ",
                Body = "This is A Welcome message from our app please Go and Login to Our app and Enjoy our services"
            };

            await _emailService.SendEmailAsync(email);

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
                Token = await CreateTokenAsync(hotelUser),
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
                Token = await CreateTokenAsync(user),
            };

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Login successfully";
            genericResponse.Data = loginUser;

            return genericResponse;
        }

        private async Task<string> CreateTokenAsync(HotelUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email,user.Email!),
                new Claim(JwtRegisteredClaimNames.NameId,user.Id),
                new Claim("Activity",user.IsActive.ToString()),
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var secretKey = _configuration["JWTOptions:SecretKey"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWTOptions:Issuer"],
                audience: _configuration["JWTOptions:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: cred
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<GenericResponse<bool>> CreateStaffUserAsync(StaffUserDTO staffUser)
        {
            var genericResponse = new GenericResponse<bool>();

            if (staffUser is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid Staff data";
                return genericResponse;
            }

            var staffUserCheck = await _userManager.FindByEmailAsync(staffUser.Email);

            if (staffUserCheck is not null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "This is already existed Email";
                return genericResponse;
            }

            var resultOfParsing = Enum.TryParse(staffUser.Specialities, out StaffSpecialities speciality);

            if (!resultOfParsing)
            {

                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid Staff speciality";
                return genericResponse;
            }
            var newStaffUser = new StaffUser
            {
                FullName = staffUser.FullName,
                Email = staffUser.Email,
                PhoneNumber = staffUser.PhoneNumber,
                Specialities = speciality,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UserName = staffUser.Email.Split("@")[0]
            };

            var result = await _userManager.CreateAsync(newStaffUser, staffUser.Password);

            if (!result.Succeeded)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = string.Join("|", result.Errors.Select(e => e.Description));
                return genericResponse;
            }

            await _userManager.AddToRoleAsync(newStaffUser, "Staff");

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Staff created successfully";
            genericResponse.Data = true;

            return genericResponse;
        }

        public async Task<GenericResponse<IEnumerable<GetUserDTO>>> GettAllUsersForAdminAsync()
        {
            var genericResponse = new GenericResponse<IEnumerable<GetUserDTO>>();

            var users = await _userManager.Users.ToListAsync();

            var listOfUserToReturn = new List<GetUserDTO>();

            if (users is null || users.Count == 0)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No Users founded";
                return genericResponse;
            }

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    continue;

                var roles = await _userManager.GetRolesAsync(user);

                var userToReturnDTO = new GetUserDTO()
                {
                    Email = user.Email!,
                    Id = user.Id,
                    IsActive = user.IsActive,
                    Role = roles.FirstOrDefault()!
                };

                listOfUserToReturn.Add(userToReturnDTO);
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Sucess to retreive all user [Staff-Guest]";
            genericResponse.Data = listOfUserToReturn;

            return genericResponse;
        }

        public async Task<GenericResponse<bool>> ActivateUser(string userId)
        {
            var genericResponse = new GenericResponse<bool>();

            if (userId is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = $"User with id : {userId} not found";
                return genericResponse;
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = $"User with id : {userId} not found";
                return genericResponse;
            }

            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);

            if (result is null)
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = genericResponse.Message = string.Join("|", result.Errors.Select(e => e.Description));
                return genericResponse;
            }

            user.UpdatedAt = DateTime.Now;
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = $"User with id : {userId} Activated Successfully";
            genericResponse.Data = true;

            return genericResponse;

        }

        public async Task<GenericResponse<bool>> DeActivateUser(string userId)
        {
            var genericResponse = new GenericResponse<bool>();

            if (userId is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = $"User with id : {userId} not found";
                return genericResponse;
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = $"User with id : {userId} not found";
                return genericResponse;
            }

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = string.Join("|", result.Errors.Select(e => e.Description));
                return genericResponse;
            }

            user.UpdatedAt = DateTime.Now;
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = $"User with id : {userId} DeActivated Successfully";
            genericResponse.Data = true;

            return genericResponse;
        }

        public async Task<GenericResponse<bool>> EmailExistAsync(string email)
        {
            var genericResponse = new GenericResponse<bool>();

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "This Email not exist";
                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Sucess to check On Email";
            genericResponse.Data = true;

            return genericResponse;
        }

        public async Task<GenericResponse<ProfileDTO>> GetProfileAsync(string userId)
        {
            var genericResponse = new GenericResponse<ProfileDTO>();

            if (userId is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = $"this Id : {userId} not found";
                return genericResponse;
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "User not found";
                return genericResponse;
            }

            var profileUserToReturn = new ProfileDTO()
            {
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                FullName = user.FullName,
                UserName = user.UserName!
            };

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "User Found Seccessfully";
            genericResponse.Data = profileUserToReturn;

            return genericResponse;
        }
    }
}
