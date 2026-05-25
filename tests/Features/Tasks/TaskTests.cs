using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Rotinik.Tests.Core;
using Rotinik.Tests.Features.Users;
using Rotinik.Tests.Features.Routines;
using Xunit;

namespace Rotinik.Tests.Features.Tasks;

public class TaskTests : IntegrationTestBase
{
    private readonly UserApiClient _userApi;
    private readonly RoutineApiClient _routineApi;
    private readonly TaskApiClient _taskApi;

    public TaskTests(CustomApiFactory factory) : base(factory) 
    { 
        _userApi = new UserApiClient(Client);
        _routineApi = new RoutineApiClient(Client);
        _taskApi = new TaskApiClient(Client);
    }

    private async Task<(int UserId, int RoutineId)> SetupUserAndRoutineAsync()
    {
        var newUser = UserDataBuilder.CreateValidRegistrationDto();
        await _userApi.RegisterUserAsync(newUser);

        var token = await _userApi.LoginAndGetTokenAsync(newUser.Email, newUser.Password);
        SetToken(token);

        var routineDto = RoutineDataBuilder.CreateValidRoutineDto();
        var routineResponse = await _routineApi.CreateRoutineAsync(routineDto);
        var routineJson = await routineResponse.Content.ReadFromJsonAsync<JsonElement>();
        var routineId = routineJson.GetProperty("data").GetProperty("id").GetInt32();

        return (0, routineId);
    }

    [Fact]
    public async Task TaskLifecycle_Create_Update_Toggle_Delete()
    {
        var setup = await SetupUserAndRoutineAsync();
        var routineId = setup.RoutineId;
        var newTask = TaskDataBuilder.CreateValidTaskDto();

        var createResponse = await _taskApi.CreateTaskAsync(routineId, newTask);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        
        var createJson = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var taskId = createJson.GetProperty("data").GetProperty("id").GetInt32();
        var initialIsCompleted = createJson.GetProperty("data").GetProperty("isCompleted").GetBoolean();
        
        Assert.False(initialIsCompleted);

        var updateTaskDto = TaskDataBuilder.CreateValidUpdateDto();
        var updateResponse = await _taskApi.UpdateTaskAsync(routineId, taskId, updateTaskDto);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var toggleResponse = await _taskApi.ToggleCompletionAsync(routineId, taskId);
        Assert.Equal(HttpStatusCode.OK, toggleResponse.StatusCode);

        var deleteResponse = await _taskApi.DeleteTaskAsync(routineId, taskId);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task CreateTask_InAnotherUsersRoutine_ReturnsForbidden()
    {
        var setupA = await SetupUserAndRoutineAsync();
        
        var userB = UserDataBuilder.CreateValidRegistrationDto();
        await _userApi.RegisterUserAsync(userB);
        var tokenB = await _userApi.LoginAndGetTokenAsync(userB.Email, userB.Password);
        SetToken(tokenB);

        var newTask = TaskDataBuilder.CreateValidTaskDto();
        var response = await _taskApi.CreateTaskAsync(setupA.RoutineId, newTask);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTask_InNonExistentRoutine_ReturnsNotFound()
    {
        await SetupUserAndRoutineAsync();
        var updateDto = TaskDataBuilder.CreateValidUpdateDto();

        int fakeRoutineId = 99999;
        int fakeTaskId = 99999;
        var response = await _taskApi.UpdateTaskAsync(fakeRoutineId, fakeTaskId, updateDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AccessProtectedTaskRoutes_WithoutToken()
    {
        ClearToken();

        var createResponse = await _taskApi.CreateTaskAsync(1, TaskDataBuilder.CreateValidTaskDto());
        var updateResponse = await _taskApi.UpdateTaskAsync(1, 1, TaskDataBuilder.CreateValidUpdateDto());
        var deleteResponse = await _taskApi.DeleteTaskAsync(1, 1);
        var toggleResponse = await _taskApi.ToggleCompletionAsync(1, 1);

        Assert.Equal(HttpStatusCode.Unauthorized, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, toggleResponse.StatusCode);
    }
}