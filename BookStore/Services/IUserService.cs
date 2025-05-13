using BookStore.DTO;
using BookStore.Entities;

namespace BookStore.Services
{
    public interface IUserService
    {
        Task<bool> AddUser(UserRegisterDTO userDTO, string? creatorRole = null);
        Task<(string? token, Users? user, IList<string> roles)> UserLoginWithUserData(LoginDTO userDTO);
        Task<List<Users>> GetAllUsersAsync();

        Task<bool> FindUser(string email);
        Task<bool> IsContactNumberTaken(string contactNumber);
    }
}