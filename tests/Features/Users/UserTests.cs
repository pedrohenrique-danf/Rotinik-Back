using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Rotinik.Features.Users.DTOs;
using Rotinik.Tests.Core;
using Xunit;

namespace Rotinik.Tests.Features.Users;

public class UserTests : IntegrationTestBase
{
    private readonly UserApiClient _userApi;

    public UserTests(CustomApiFactory factory) : base(factory) 
    { 
        _userApi = new UserApiClient(Client);
    }

    [Fact]
    public async Task UserLifecycle()
    {
        var newUser = UserDataBuilder.CreateValidRegistrationDto();
        await _userApi.RegisterUserAsync(newUser);

        var token = await _userApi.LoginAndGetTokenAsync(newUser.Email, newUser.Password);
        SetToken(token);

        var meResponse = await _userApi.GetCurrentUserAsync();
        var meJson = await meResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = meJson.GetProperty("id").GetInt32();

        var updateResponse = await _userApi.UpdateUserAsync(userId, UserDataBuilder.CreateValidUpdateDto());
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var deleteResponse = await _userApi.DeleteUserAsync(userId);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var loginResponse = await _userApi.LoginAsync(new() { Email = newUser.Email, Password = newUser.Password });
        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokens()
    {
        var user = UserDataBuilder.CreateValidRegistrationDto();
        await _userApi.RegisterUserAsync(user);

        var loginData = new UserLoginDto { Email = user.Email, Password = user.Password };
        var response = await _userApi.LoginAsync(loginData);
        
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
        await _userApi.RegisterUserAsync(user);
        
        var loginData = new UserLoginDto { Email = user.Email, Password = user.Password };
        var loginResponse = await _userApi.LoginAsync(loginData);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        
        var oldAccessToken = loginResult.GetProperty("data").GetProperty("accessToken").GetString();
        var oldRefreshToken = loginResult.GetProperty("data").GetProperty("refreshToken").GetString();

        var refreshRequest = new RefreshTokenRequestDto
        {
            AccessToken = oldAccessToken!,
            RefreshToken = oldRefreshToken!
        };
        
        var refreshResponse = await _userApi.RefreshTokenAsync(refreshRequest);
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
        await _userApi.RegisterUserAsync(user);

        var loginData = new UserLoginDto { Email = user.Email, Password = "WrongPassword123!" };
        var response = await _userApi.LoginAsync(loginData);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
    {
        var loginData = new UserLoginDto { Email = "ghost@email.com", Password = "pAssword123!" };
        var response = await _userApi.LoginAsync(loginData);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetPublicProfile_WithValidUsername()
    {
        var user = UserDataBuilder.CreateValidRegistrationDto();
        await _userApi.RegisterUserAsync(user);

        var response = await _userApi.GetPublicProfileAsync(user.UserName);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var profileData = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(user.Name, profileData.GetProperty("name").GetString());
    }

    [Fact]
    public async Task CreateUser_WithWeakPassword()
    {
        var weakUser = UserDataBuilder.CreateValidRegistrationDto();
        weakUser.Password = "weak";

        var response = await _userApi.RegisterUserAsync(weakUser);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateEmail()
    {
        var firstUser = UserDataBuilder.CreateValidRegistrationDto();
        await _userApi.RegisterUserAsync(firstUser);

        var duplicateEmailUser = UserDataBuilder.CreateValidRegistrationDto();
        duplicateEmailUser.Email = firstUser.Email; 

        var response = await _userApi.RegisterUserAsync(duplicateEmailUser);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithFutureBirthDate()
    {
        var futureUser = UserDataBuilder.CreateValidRegistrationDto();
        futureUser.BirthDate = DateTime.UtcNow.AddYears(1);

        var response = await _userApi.RegisterUserAsync(futureUser);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_UpdateAnotherUser()
    {
        var userA = UserDataBuilder.CreateValidRegistrationDto();
        await _userApi.RegisterUserAsync(userA);
        
        var token = await _userApi.LoginAndGetTokenAsync(userA.Email, userA.Password);
        SetToken(token);

        int anotherUserId = 99999;
        var response = await _userApi.UpdateUserAsync(anotherUserId, UserDataBuilder.CreateValidUpdateDto());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_DeleteAnotherUser()
    {
        var userA = UserDataBuilder.CreateValidRegistrationDto();
        await _userApi.RegisterUserAsync(userA);
        
        var token = await _userApi.LoginAndGetTokenAsync(userA.Email, userA.Password);
        SetToken(token);

        int anotherUserId = 99999;
        var response = await _userApi.DeleteUserAsync(anotherUserId);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AccessProtectedRoutes_WithoutToken()
    {
        ClearToken();

        var getMeResponse = await _userApi.GetCurrentUserAsync();
        var updateResponse = await _userApi.UpdateUserAsync(1, UserDataBuilder.CreateValidUpdateDto());
        var deleteResponse = await _userApi.DeleteUserAsync(1);

        Assert.Equal(HttpStatusCode.Unauthorized, getMeResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
    }
}