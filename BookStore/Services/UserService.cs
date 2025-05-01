using BookStore.DTO;
using BookStore.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Services
{
    public class UserService:IUserService
    {
        private UserManager<Users> _userManager;

        public UserService(UserManager<Users> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> AddUser(UserRegisterDTO userDTO)
        {
            var newUser = new Users
            {
                UserName = userDTO.UserName, 
                Email = userDTO.Email,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                ContactNumber = userDTO.ContactNumber, 
                MembershipId = userDTO.MembershipId
            };

            var result = await _userManager.CreateAsync(newUser, userDTO.Password);

            if (result.Succeeded)
            {
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

        public async Task<Users?> UserLogin(LoginDTO loginCredential)
        {
            var user = await isUserExist(loginCredential.Email);

            if (user != null)
            {
                var isAuthenticated = await _userManager.CheckPasswordAsync(user, loginCredential.Password);
                if (isAuthenticated)
                {
                    return user;
                }
            }

            return null;
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
