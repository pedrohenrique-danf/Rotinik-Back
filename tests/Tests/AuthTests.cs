using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Rotinik.DTOs.User;
using Rotinik.Tests.Base;
using Rotinik.Tests.Setup;
using Xunit;

namespace Rotinik.Tests.Tests;

public class AuthTests : IntegrationTestBase
{
    private const string AuthRoute = "/api/auth";
    private const string UserRoute = "/api/user"; 

    public AuthTests(CustomApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokens()
    {
        var user = CreateUserDto();
        await RegisterUser(user);

        var loginData = new UserLoginDto { Email = user.Email, Password = user.Password };
        var response = await Client.PostAsJsonAsync($"{AuthRoute}/login", loginData);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var loginResult = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        var accessToken = loginResult.GetProperty("data").GetProperty("accessToken").GetString();
        var refreshToken = loginResult.GetProperty("data").GetProperty("refreshToken").GetString();
        
        Assert.False(string.IsNullOrEmpty(accessToken));
        Assert.False(string.IsNullOrEmpty(refreshToken));
    }

    [Fact]
    public async Task RefreshToken_WithValidData_ReturnsNewTokens()
    {
        var user = CreateUserDto();
        await RegisterUser(user);
        var loginData = new UserLoginDto { Email = user.Email, Password = user.Password };
        var loginResponse = await Client.PostAsJsonAsync($"{AuthRoute}/login", loginData);
        
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var oldAccessToken = loginResult.GetProperty("data").GetProperty("accessToken").GetString();
        var oldRefreshToken = loginResult.GetProperty("data").GetProperty("refreshToken").GetString();

        var refreshRequest = new RefreshTokenRequestDto
        {
            AccessToken = oldAccessToken!,
            RefreshToken = oldRefreshToken!
        };
        
        var refreshResponse = await Client.PostAsJsonAsync($"{AuthRoute}/refresh-token", refreshRequest);
        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

        var refreshResult = await refreshResponse.Content.ReadFromJsonAsync<JsonElement>();
        var newAccessToken = refreshResult.GetProperty("data").GetProperty("accessToken").GetString();
        var newRefreshToken = refreshResult.GetProperty("data").GetProperty("refreshToken").GetString();

        Assert.False(string.IsNullOrEmpty(newAccessToken));
        Assert.False(string.IsNullOrEmpty(newRefreshToken));
        Assert.NotEqual(oldAccessToken, newAccessToken);
        Assert.NotEqual(oldRefreshToken, newRefreshToken);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var user = CreateUserDto();
        await RegisterUser(user);

        var loginData = new UserLoginDto { Email = user.Email, Password = "WrongPassword123!" };
        var response = await Client.PostAsJsonAsync($"{AuthRoute}/login", loginData);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
    {
        var loginData = new UserLoginDto { Email = "ghost@email.com", Password = "pAssword123!" };
        var response = await Client.PostAsJsonAsync($"{AuthRoute}/login", loginData);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ===================================================================
    // DOMAIN-SPECIFIC HELPER METHODS
    // ===================================================================
    
    private static UserRegistrationDto CreateUserDto() => new()
    {
        Name = "Test User Auth",
        BirthDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        UserName = $"authuser_{Guid.NewGuid():N}",
        Email = $"auth_{Guid.NewGuid():N}@email.com",
        Password = "pAssword123!"
    };

    private async Task RegisterUser(UserRegistrationDto user)
    {
        var response = await Client.PostAsJsonAsync(UserRoute, user);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}