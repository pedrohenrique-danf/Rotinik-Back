using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Rotinik.DTOs.User;
using Rotinik.Tests.Base;
using Rotinik.Tests.Setup;
using Xunit;

namespace Rotinik.Tests.Tests;

public class UserTests : IntegrationTestBase
{
    private const string BaseRoute = "/api/user";
    private const string LoginRoute = "/api/user/login";
    private const string MeRoute = "/api/user/me";

    public UserTests(CustomApiFactory factory) : base(factory) { }


    // ===================================================================
    // HAPPY PATHS
    // ===================================================================

    [Fact]
    public async Task UserLifecycle()
    {
        var newUser = CreateUserDto();
        await RegisterUser(newUser);

        var token = await LoginUser(newUser.Email, newUser.Password);
        SetAuthorizationHeader(token);

        var userId = await GetCurrentUserId();
        await UpdateUser(userId);
        await DeleteUser(userId);
        await VerifyLoginFails(newUser.Email, newUser.Password);
    }

    [Fact]
    public async Task GetPublicProfile_WithValidUsername()
    {
        var user = CreateUserDto();
        await RegisterUser(user);

        var response = await Client.GetAsync($"{BaseRoute}/profile/{user.UserName}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profileData = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(user.Name, profileData.GetProperty("name").GetString());
        Assert.Equal(user.UserName, profileData.GetProperty("userName").GetString());
    }


    // ===================================================================
    // SAD PATHS
    // ===================================================================

    [Fact]
    public async Task CreateUser_WithWeakPassword()
    {
        var weakUser = new UserRegistrationDto
        {
            Name = "Weak User",
            BirthDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UserName = $"weak_{Guid.NewGuid():N}",
            Email = $"weak_{Guid.NewGuid():N}@email.com",
            Password = "weak"
        };

        var response = await Client.PostAsJsonAsync(BaseRoute, weakUser);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var errorResult = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        var errorMessage = errorResult.GetProperty("errors").GetProperty("Password")[0].GetString();
        Assert.Contains("Password requirements", errorMessage);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateEmail()
    {
        var firstUser = CreateUserDto();
        await RegisterUser(firstUser);

        var duplicateEmailUser = CreateUserDto();
        duplicateEmailUser.Email = firstUser.Email; 

        var response = await Client.PostAsJsonAsync(BaseRoute, duplicateEmailUser);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var errorResult = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains("Email in use.", errorResult.GetProperty("message").GetString());
    }

    [Fact]
    public async Task CreateUser_WithFutureBirthDate()
    {
        var futureUser = CreateUserDto();
        futureUser.BirthDate = DateTime.UtcNow.AddYears(1);

        var response = await Client.PostAsJsonAsync(BaseRoute, futureUser);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var errorResult = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        var errorMessage = errorResult.GetProperty("errors").GetProperty("BirthDate")[0].GetString();
        Assert.Contains("Birth date cannot be in the future.", errorMessage);
    }

    [Fact]
    public async Task GetPublicProfile_WithInvalidUsername()
    {
        var response = await Client.GetAsync($"{BaseRoute}/profile/this_user_does_not_exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateUserName()
    {
        var firstUser = CreateUserDto();
        await RegisterUser(firstUser);

        var duplicateUserNameUser = CreateUserDto();
        duplicateUserNameUser.UserName = firstUser.UserName;

        var response = await Client.PostAsJsonAsync(BaseRoute, duplicateUserNameUser);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var errorResult = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains("UserName in use.", errorResult.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Login_WithWrongPassword()
    {
        var user = CreateUserDto();
        await RegisterUser(user);

        var loginData = new UserLoginDto { Email = user.Email, Password = "WrongPassword123!" };
        var response = await Client.PostAsJsonAsync(LoginRoute, loginData);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail()
    {
        var loginData = new UserLoginDto { Email = "ghost@email.com", Password = "pAssword123!" };
        var response = await Client.PostAsJsonAsync(LoginRoute, loginData);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }


    // ===================================================================
    // SECURITY PATHS
    // ===================================================================

    [Fact]
    public async Task UpdateUser_UpdateAnotherUser()
    {
        var userA = CreateUserDto();
        await RegisterUser(userA);
        
        var token = await LoginUser(userA.Email, userA.Password);
        SetAuthorizationHeader(token);

        var updateData = new UserUpdateDto
        {
            Name = "Hacked Name",
            BirthDate = new DateTime(1995, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };
        
        int anotherUserId = 99999;
        var response = await Client.PutAsJsonAsync($"{BaseRoute}/{anotherUserId}", updateData);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_DeleteAnotherUser()
    {
        var userA = CreateUserDto();
        await RegisterUser(userA);
        
        var token = await LoginUser(userA.Email, userA.Password);
        SetAuthorizationHeader(token);

        int anotherUserId = 99999;
        var response = await Client.DeleteAsync($"{BaseRoute}/{anotherUserId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AccessProtectedRoutes_WithoutToken()
    {
        Client.DefaultRequestHeaders.Authorization = null;

        var getMeResponse = await Client.GetAsync(MeRoute);
        var updateResponse = await Client.PutAsJsonAsync($"{BaseRoute}/1", new UserUpdateDto());
        var deleteResponse = await Client.DeleteAsync($"{BaseRoute}/1");

        Assert.Equal(HttpStatusCode.Unauthorized, getMeResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
    }


    // ===================================================================
    // DOMAIN-SPECIFIC HELPER METHODS
    // ===================================================================
    
    private static UserRegistrationDto CreateUserDto() => new()
    {
        Name = "Test User",
        BirthDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        UserName = $"testuser_{Guid.NewGuid():N}",
        Email = $"tester_{Guid.NewGuid():N}@email.com",
        Password = "pAssword123!"
    };

    private async Task RegisterUser(UserRegistrationDto user)
    {
        var response = await Client.PostAsJsonAsync(BaseRoute, user);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private async Task<string> LoginUser(string email, string password)
    {
        var loginData = new UserLoginDto { Email = email, Password = password };
        var response = await Client.PostAsJsonAsync(LoginRoute, loginData);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var loginResult = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginResult.GetProperty("token").GetString();
        
        Assert.False(string.IsNullOrEmpty(token));
        return token!;
    }

    private async Task<int> GetCurrentUserId()
    {
        var response = await Client.GetAsync(MeRoute);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var userData = await response.Content.ReadFromJsonAsync<JsonElement>();
        var userId = userData.GetProperty("id").GetInt32();
        
        Assert.True(userId > 0);
        return userId;
    }

    private async Task UpdateUser(int userId)
    {
        var updateData = new UserUpdateDto
        {
            Name = "Updated Name",
            BirthDate = new DateTime(1995, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };

        var response = await Client.PutAsJsonAsync($"{BaseRoute}/{userId}", updateData);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task DeleteUser(int userId)
    {
        var response = await Client.DeleteAsync($"{BaseRoute}/{userId}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task VerifyLoginFails(string email, string password)
    {
        var loginData = new UserLoginDto { Email = email, Password = password };
        var response = await Client.PostAsJsonAsync(LoginRoute, loginData);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}