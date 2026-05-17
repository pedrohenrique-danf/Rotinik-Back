using Rotinik.Models;

namespace Rotinik.Data.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUserNameAsync(string username);
    Task AddAsync(User user);
    void Remove(User user);
    Task SaveChangesAsync();
}