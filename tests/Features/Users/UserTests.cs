using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Rotinik.Features.Users.DTOs;
using Rotinik.Tests.Core;
using Xunit;

namespace Rotinik.Tests.Features.Users;

public class UserTests : IntegrationTestBase
{
    public UserTests(CustomApiFactory factory) : base(factory) { }

    [Fact]
    public async Task UserLifecycle()
    {
        var newUser = UserDataBuilder.CreateValidRegistrationDto();
        await ApiClient.RegisterUserAsync(newUser);

        var token = await ApiClient.LoginAndGetTokenAsync(newUser.Email, newUser.Password);
        ApiClient.SetToken(token);

        var meResponse = await ApiClient.GetCurrentUserAsync();
        var meJson = await meResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = meJson.GetProperty("id").GetInt32();

        var updateResponse = await ApiClient.UpdateUserAsync(userId, UserDataBuilder.CreateValidUpdateDto());
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var deleteResponse = await ApiClient.DeleteUserAsync(userId);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var loginResponse = await ApiClient.LoginAsync(new() { Email = newUser.Email, Password = newUser.Password });
        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }

    [Fact]
    public async Task GetPublicProfile_WithValidUsername()
    {
        var user = UserDataBuilder.CreateValidRegistrationDto();
        await ApiClient.RegisterUserAsync(user);

        var response = await ApiClient.GetPublicProfileAsync(user.UserName);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var profileData = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(user.Name, profileData.GetProperty("name").GetString());
    }

    [Fact]
    public async Task CreateUser_WithWeakPassword()
    {
        var weakUser = UserDataBuilder.CreateValidRegistrationDto();
        weakUser.Password = "weak";

        var response = await ApiClient.RegisterUserAsync(weakUser);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateEmail()
    {
        var firstUser = UserDataBuilder.CreateValidRegistrationDto();
        await ApiClient.RegisterUserAsync(firstUser);

        var duplicateEmailUser = UserDataBuilder.CreateValidRegistrationDto();
        duplicateEmailUser.Email = firstUser.Email; 

        var response = await ApiClient.RegisterUserAsync(duplicateEmailUser);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithFutureBirthDate()
    {
        var futureUser = UserDataBuilder.CreateValidRegistrationDto();
        futureUser.BirthDate = DateTime.UtcNow.AddYears(1);

        var response = await ApiClient.RegisterUserAsync(futureUser);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_UpdateAnotherUser()
    {
        var userA = UserDataBuilder.CreateValidRegistrationDto();
        await ApiClient.RegisterUserAsync(userA);
        
        var token = await ApiClient.LoginAndGetTokenAsync(userA.Email, userA.Password);
        ApiClient.SetToken(token);

        int anotherUserId = 99999;
        var response = await ApiClient.UpdateUserAsync(anotherUserId, UserDataBuilder.CreateValidUpdateDto());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_DeleteAnotherUser()
    {
        var userA = UserDataBuilder.CreateValidRegistrationDto();
        await ApiClient.RegisterUserAsync(userA);
        
        var token = await ApiClient.LoginAndGetTokenAsync(userA.Email, userA.Password);
        ApiClient.SetToken(token);

        int anotherUserId = 99999;
        var response = await ApiClient.DeleteUserAsync(anotherUserId);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AccessProtectedRoutes_WithoutToken()
    {
        ApiClient.ClearToken();

        var getMeResponse = await ApiClient.GetCurrentUserAsync();
        var updateResponse = await ApiClient.UpdateUserAsync(1, UserDataBuilder.CreateValidUpdateDto());
        var deleteResponse = await ApiClient.DeleteUserAsync(1);

        Assert.Equal(HttpStatusCode.Unauthorized, getMeResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
    }
}