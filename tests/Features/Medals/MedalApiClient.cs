using System.Net.Http.Json;

namespace Rotinik.Tests.Features.Medals;

public class MedalApiClient
{
    private readonly HttpClient _client;

    public MedalApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<HttpResponseMessage> GetMyMedalsAsync()
        => await _client.GetAsync("/api/medal/me");
}