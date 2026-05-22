using System.Net.Http.Json;
using Rotinik.Features.Routines.DTO;

namespace Rotinik.Tests.Features.Routines;

public class RoutineApiClient
{
    private readonly HttpClient _client;

    public RoutineApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<HttpResponseMessage> CreateRoutineAsync(RoutineCreateDto dto)
        => await _client.PostAsJsonAsync("/api/routine", dto);

    public async Task<HttpResponseMessage> UpdateRoutineAsync(int id, RoutineUpdateDto dto)
        => await _client.PutAsJsonAsync($"/api/routine/{id}", dto);

    public async Task<HttpResponseMessage> DeleteRoutineAsync(int id)
        => await _client.DeleteAsync($"/api/routine/{id}");

    public async Task<HttpResponseMessage> GetUserRoutinesAsync()
        => await _client.GetAsync("/api/routine");
}