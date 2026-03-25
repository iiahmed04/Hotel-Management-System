using HMS.Core.Contracts;
using HMS.Core.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;

namespace HMS.Infrastructure.Data.DataSeed
{
    public class IdentityDataIntializer : IDataIntializer
    {
        private readonly UserManager<HotelUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityDataIntializer(UserManager<HotelUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task IntializeAdminAndRoleAsync()
        {
            if (!_roleManager.Roles.Any())
            {
                var adminRole = new IdentityRole { Name = "Admin" };
                var staffRole = new IdentityRole { Name = "Staff" };
                var guestRole = new IdentityRole { Name = "Guest" };

                await _roleManager.CreateAsync(adminRole);
                await _roleManager.CreateAsync(staffRole);
                await _roleManager.CreateAsync(guestRole);
            }

            if (!_userManager.Users.Any())
            {
                var admin = new HotelUser
                {
                    FullName = "Admin.Hotel",
                    Email = "Admin.HMS@gmail.com",
                    UserName = "Admin_HMS",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                };

                await _userManager.CreateAsync(admin, "P@ssw0rd");

                await _userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
