using BookStore.DTO;
using BookStore.Entities;

namespace BookStore.Services
{
    public interface IUserService
    {
        Task<bool> AddUser(UserRegisterDTO userDTO);
        Task<string?> UserLogin(LoginDTO userDTO);
        Task<bool> FindUser(string email);
    }
}
