using System.Net.Http.Json;
using Rotinik.Features.Tasks.DTO;

namespace Rotinik.Tests.Features.Tasks;

public class TaskApiClient
{
    private readonly HttpClient _client;

    public TaskApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<HttpResponseMessage> CreateTaskAsync(int routineId, TaskCreateDto dto) 
        => await _client.PostAsJsonAsync($"/api/routine/{routineId}/task", dto);

    public async Task<HttpResponseMessage> UpdateTaskAsync(int routineId, int taskId, TaskUpdateDto dto) 
        => await _client.PutAsJsonAsync($"/api/routine/{routineId}/task/{taskId}", dto);

    public async Task<HttpResponseMessage> DeleteTaskAsync(int routineId, int taskId) 
        => await _client.DeleteAsync($"/api/routine/{routineId}/task/{taskId}");

    public async Task<HttpResponseMessage> ToggleCompletionAsync(int routineId, int taskId) 
        => await _client.PatchAsync($"/api/routine/{routineId}/task/{taskId}/toggle", null);
}