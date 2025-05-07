using BookStore.DTO;
using BookStore.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Services
{
    public class UserService : IUserService
    {
        private UserManager<Users> _userManager;

        private readonly JwtTokenService _jwtTokenService;

        public UserService(UserManager<Users> userManager, JwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<bool> AddUser(UserRegisterDTO userDTO, string? creatorRole = null)
        {
            string roleToAssign = "Member";

            // Admin registering a user with specific role
            if (!string.IsNullOrEmpty(creatorRole) && creatorRole == "Admin" &&
                !string.IsNullOrEmpty(userDTO.Role) && (userDTO.Role == "Admin" || userDTO.Role == "Staff"))
            {
                roleToAssign = userDTO.Role;
            }

            var newUser = new Users
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                ContactNumber = userDTO.ContactNumber,
                MembershipId = (roleToAssign == "Member") ? Guid.NewGuid().ToString() : null
            };

            var result = await _userManager.CreateAsync(newUser, userDTO.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, roleToAssign);
                return true;
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Code} - {error.Description}");
                }
                return false;
            }
        }


        public async Task<(string? token, Users? user, IList<string> roles)> UserLoginWithUserData(LoginDTO loginCredential)
        {
            var user = await isUserExist(loginCredential.Email);
            if (user != null)
            {
                var isAuthenticated = await _userManager.CheckPasswordAsync(user, loginCredential.Password);
                if (isAuthenticated)
                {
                    var token = await _jwtTokenService.GenerateUserToken(user);
                    var roles = await _userManager.GetRolesAsync(user);
                    return (token, user, roles);
                }
            }
            return (null, null, new List<string>());
        }


        private async Task<Users> isUserExist(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> FindUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
    }
}