using Microsoft.EntityFrameworkCore;
using Rotinik_Backend.Models;

namespace Rotinik_Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
}
