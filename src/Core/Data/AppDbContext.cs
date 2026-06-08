using Microsoft.EntityFrameworkCore;
using Rotinik.Features.Payments;
using Rotinik.Features.Routines;
using Rotinik.Features.Users;
using Rotinik.Features.Medals;
using Rotinik.Features.Statistics;
using Rotinik.Features.Shop;
using Rotinik.Features.Wallet;

namespace Rotinik.Core.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public DbSet<WalletTransaction> Payments { get; set; }
    public DbSet<Routine> Routines { get; set; }
    public DbSet<Rotinik.Features.Tasks.TaskItem> Tasks { get; set; }
    public DbSet<Medal> Medals { get; set; }
    public DbSet<UserMedal> UserMedals { get; set; }
    public DbSet<DailyUserSummary> DailyUserSummaries { get; set; }
    public DbSet<ShopItem> ShopItems { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<UserShopItem> UserShopItems { get; set; }
    public DbSet<Rotinik.Features.Tasks.TaskHistory> TaskHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}