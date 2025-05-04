using BookStore.DTO;
using BookStore.Entities;

namespace BookStore.Services
{
    public interface IUserService
    {
        Task<bool> AddUser(UserRegisterDTO userDTO);
        Task<(string? token, Users? user, IList<string> roles)> UserLoginWithUserData(LoginDTO userDTO);
        Task<bool> FindUser(string email);
    }
}