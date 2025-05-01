using BookStore.DTO;
using BookStore.Entities;

namespace BookStore.Services
{
    public interface IUserService
    {
        Task<bool> AddUser(UserRegisterDTO userDTO);
        Task<Users?> UserLogin(LoginDTO userDTO);
        Task<bool> FindUser(string email);
    }
}
