using BookStore.DTO;

namespace BookStore.Services
{
    public interface IUserService
    {
        Task<bool> AddUser(UserRegisterDTO userDTO);
        Task<bool> UserLogin(LoginDTO userDTO);
        Task<bool> FindUser(string email);
    }
}
