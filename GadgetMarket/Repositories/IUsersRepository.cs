using GadgetMarket.Model;

namespace GadgetMarket.Repositories
{
    public interface IUsersRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> UpdateUserAsync(User user);
        Task<bool> RemoveUserByIdAsync(int id);
        Task<User?> SignInAsync(User user);
    }
}