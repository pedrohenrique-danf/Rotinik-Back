using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Rotinik.Features.Routines.DTOs;
using Rotinik.Tests.Core;
using Rotinik.Tests.Features.Users;
using Xunit;

namespace Rotinik.Tests.Features.Routines;

public class RoutineTests : IntegrationTestBase
{
    private readonly RoutineApiClient _routineApi;
    private readonly UserApiClient _userApi;

    public RoutineTests(CustomApiFactory factory) : base(factory)
    {
        _routineApi = new RoutineApiClient(Client);
        _userApi = new UserApiClient(Client);
    }

    private async Task AuthenticateAsync()
    {
        var user = UserDataBuilder.CreateValidRegistrationDto();
        await _userApi.RegisterUserAsync(user);
        var token = await _userApi.LoginAndGetTokenAsync(user.Email, user.Password);
        SetToken(token);
    }

    [Fact]
    public async Task RoutineLifecycle_CreateUpdateDelete()
    {
        await AuthenticateAsync();
        var newRoutine = RoutineDataBuilder.CreateValidRoutineDto();

        var createResponse = await _routineApi.CreateRoutineAsync(newRoutine);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var json = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var routineId = json.GetProperty("data").GetProperty("id").GetInt32();

        var updateDto = RoutineDataBuilder.CreateValidUpdateDto();
        var updateResponse = await _routineApi.UpdateRoutineAsync(routineId, updateDto);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var deleteResponse = await _routineApi.DeleteRoutineAsync(routineId);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task AccessProtectedRoutes_WithoutToken_ReturnsUnauthorized()
    {
        ClearToken();
        var dto = RoutineDataBuilder.CreateValidRoutineDto();

        var createResponse = await _routineApi.CreateRoutineAsync(dto);
        var updateResponse = await _routineApi.UpdateRoutineAsync(1, new RoutineUpdateDto());
        var deleteResponse = await _routineApi.DeleteRoutineAsync(1);

        Assert.Equal(HttpStatusCode.Unauthorized, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task CreateRoutine_WithMissingData_ReturnsBadRequest()
    {
        await AuthenticateAsync();
        var invalidRoutine = new RoutineCreateDto { Title = "", Category = "" };

        var response = await _routineApi.CreateRoutineAsync(invalidRoutine);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRoutine_FromAnotherUser_ReturnsForbidden()
    {
        await AuthenticateAsync();
        var createResponse = await _routineApi.CreateRoutineAsync(RoutineDataBuilder.CreateValidRoutineDto());
        var json = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var routineId = json.GetProperty("data").GetProperty("id").GetInt32();

        ClearToken();
        await AuthenticateAsync(); 

        var response = await _routineApi.UpdateRoutineAsync(routineId, RoutineDataBuilder.CreateValidUpdateDto());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteRoutine_FromAnotherUser_ReturnsForbidden()
    {
        await AuthenticateAsync();
        var createResponse = await _routineApi.CreateRoutineAsync(RoutineDataBuilder.CreateValidRoutineDto());
        var json = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var routineId = json.GetProperty("data").GetProperty("id").GetInt32();

        ClearToken();
        await AuthenticateAsync(); 

        var response = await _routineApi.DeleteRoutineAsync(routineId);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
public async Task GetUserRoutines_ReturnsOnlyCurrentUserRoutines()
{
    await AuthenticateAsync();

    await _routineApi.CreateRoutineAsync(new RoutineCreateDto 
    { 
        Title = "Rotina B1", 
        Category = "Test" 
    });

    var response = await _routineApi.GetUserRoutinesAsync();
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var json = await response.Content.ReadFromJsonAsync<JsonElement>();
    var routines = json.GetProperty("data").EnumerateArray().ToList();

    Assert.Equal(2, routines.Count);
    Assert.Contains(routines, r => r.GetProperty("title").GetString() == "Inbox");
    Assert.Contains(routines, r => r.GetProperty("title").GetString() == "Rotina B1");
}
}