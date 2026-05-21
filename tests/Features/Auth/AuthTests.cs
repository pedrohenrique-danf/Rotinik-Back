using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Rotinik.Features.Users.DTOs;
using Rotinik.Tests.Core;
using Rotinik.Tests.Features.Users;
using Xunit;

namespace Rotinik.Tests.Features.Auth;

public class AuthTests : IntegrationTestBase
{
    public AuthTests(CustomApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokens()
    {
        var user = UserDataBuilder.CreateValidRegistrationDto();
        await ApiClient.RegisterUserAsync(user);

        var loginData = new UserLoginDto { Email = user.Email, Password = user.Password };
        var response = await ApiClient.LoginAsync(loginData);
        
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
        var user = UserDataBuilder.CreateValidRegistrationDto();
        await ApiClient.RegisterUserAsync(user);
        
        var loginData = new UserLoginDto { Email = user.Email, Password = user.Password };
        var loginResponse = await ApiClient.LoginAsync(loginData);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        
        var oldAccessToken = loginResult.GetProperty("data").GetProperty("accessToken").GetString();
        var oldRefreshToken = loginResult.GetProperty("data").GetProperty("refreshToken").GetString();

        var refreshRequest = new RefreshTokenRequestDto
        {
            AccessToken = oldAccessToken!,
            RefreshToken = oldRefreshToken!
        };
        
        var refreshResponse = await ApiClient.RefreshTokenAsync(refreshRequest);
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
        var user = UserDataBuilder.CreateValidRegistrationDto();
        await ApiClient.RegisterUserAsync(user);

        var loginData = new UserLoginDto { Email = user.Email, Password = "WrongPassword123!" };
        var response = await ApiClient.LoginAsync(loginData);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
    {
        var loginData = new UserLoginDto { Email = "ghost@email.com", Password = "pAssword123!" };
        var response = await ApiClient.LoginAsync(loginData);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}