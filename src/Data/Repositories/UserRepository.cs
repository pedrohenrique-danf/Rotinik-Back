using Microsoft.EntityFrameworkCore;
using Rotinik.Models;

namespace Rotinik.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id) =>
        await _context.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByUserNameAsync(string username) =>
        await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user);

    public void Remove(User user) =>
        _context.Users.Remove(user);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}