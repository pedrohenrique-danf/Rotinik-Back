using Rotinik.Features.Users.DTOs;

namespace Rotinik.Tests.Features.Users;

public static class UserDataBuilder
{
    public static UserRegistrationDto CreateValidRegistrationDto() => new()
    {
        Name = "Test User",
        BirthDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        UserName = $"user_{Guid.NewGuid():N}",
        Email = $"test_{Guid.NewGuid():N}@email.com",
        Password = "pAssword123!"
    };

    public static UserUpdateDto CreateValidUpdateDto() => new()
    {
        Name = "Updated Name",
        BirthDate = new DateTime(1995, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        Password = "NewpAssword123!"
    };
}