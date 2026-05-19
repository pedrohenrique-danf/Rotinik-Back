using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Rotinik.DTOs.User;

namespace Rotinik.Tests.Clients;

public class RotinikApiClient
{
    private readonly HttpClient _client;

    public RotinikApiClient(HttpClient client)
    {
        _client = client;
    }

    public void SetToken(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    
    public void ClearToken()
    {
        _client.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<HttpResponseMessage> RegisterUserAsync(UserRegistrationDto dto) 
        => await _client.PostAsJsonAsync("/api/user", dto);

    public async Task<HttpResponseMessage> GetPublicProfileAsync(string username) 
        => await _client.GetAsync($"/api/user/profile/{username}");

    public async Task<HttpResponseMessage> GetCurrentUserAsync() 
        => await _client.GetAsync("/api/user/me");

    public async Task<HttpResponseMessage> UpdateUserAsync(int id, UserUpdateDto dto) 
        => await _client.PutAsJsonAsync($"/api/user/{id}", dto);

    public async Task<HttpResponseMessage> DeleteUserAsync(int id) 
        => await _client.DeleteAsync($"/api/user/{id}");

    public async Task<HttpResponseMessage> LoginAsync(UserLoginDto dto) 
        => await _client.PostAsJsonAsync("/api/auth/login", dto);
        
    public async Task<HttpResponseMessage> RefreshTokenAsync(RefreshTokenRequestDto dto)
        => await _client.PostAsJsonAsync("/api/auth/refresh-token", dto);

    public async Task<string> LoginAndGetTokenAsync(string email, string password)
    {
        var response = await LoginAsync(new UserLoginDto { Email = email, Password = password });
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("data").GetProperty("accessToken").GetString()!;
    }
}