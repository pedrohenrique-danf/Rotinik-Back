using Microsoft.EntityFrameworkCore;
using RotinikApi.Models;

namespace RotinikApi.Data
{
    public class RotinikContext : DbContext
    {
        
        public RotinikContext(DbContextOptions<RotinikContext> opts) : base(opts) { }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}