using Microsoft.EntityFrameworkCore;
using Rotinik.Models;

namespace Rotinik.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
}
