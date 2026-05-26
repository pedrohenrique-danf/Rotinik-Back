using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rotinik.Core.Data;
using Rotinik.Features.Users;
using Rotinik.Core.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Rotinik.Tests.Features.Users;

public class DbDebugTests
{
    private readonly ITestOutputHelper _output;

    public DbDebugTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void PrintUsersInDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=rotinik_db;Username=postgres;Password=postgres")
            .Options;

        using var context = new AppDbContext(options);
        var users = context.Users.AsNoTracking().ToList();

        _output.WriteLine($"--- TOTAL USERS: {users.Count} ---");
        foreach (var u in users)
        {
            _output.WriteLine($"USER: Id={u.Id}, Name={u.Name}, Email={u.Email}, UserName={u.UserName}");
        }
        _output.WriteLine("---------------------------------");
    }

    [Fact]
    public void PrintRoutinesInDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=rotinik_db;Username=postgres;Password=postgres")
            .Options;

        using var context = new AppDbContext(options);
        var routines = context.Routines.AsNoTracking().ToList();

        _output.WriteLine($"--- TOTAL ROUTINES: {routines.Count} ---");
        foreach (var r in routines)
        {
            _output.WriteLine($"ROUTINE: Id={r.Id}, Title={r.Title}, UserId={r.UserId}");
        }
        _output.WriteLine("---------------------------------");
    }
}
