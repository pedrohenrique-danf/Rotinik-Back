using System.Net.Http.Json;
using System.Text.Json;
using Rotinik.Features.Users.DTOs;

namespace Rotinik.Tests.Features.Users;

public class UserApiClient
{
    private readonly HttpClient _client;

    public UserApiClient(HttpClient client)
    {
        _client = client;
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
        => await _client.PostAsJsonAsync("/api/user/login", dto);
        
    public async Task<HttpResponseMessage> RefreshTokenAsync(TokenDto dto)
        => await _client.PostAsJsonAsync("/api/user/refresh-token", dto);

    // Novos métodos adicionados para ampliar a cobertura
    public async Task<HttpResponseMessage> GetRankAsync(int limit = 100)
        => await _client.GetAsync($"/api/user/rank?limit={limit}");

    public async Task<HttpResponseMessage> GetPremiumContentAsync()
        => await _client.GetAsync("/api/user/conteudo-vip");

    public async Task<string> LoginAndGetTokenAsync(string email, string password)
    {
        var response = await LoginAsync(new UserLoginDto { Email = email, Password = password });
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("data").GetProperty("accessToken").GetString()!;
    }
}