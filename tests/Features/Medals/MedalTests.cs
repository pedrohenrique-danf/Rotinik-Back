using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Rotinik.Core.Data;
using Rotinik.Features.Medals;
using Rotinik.Tests.Core;
using Rotinik.Tests.Features.Users;
using Xunit;

namespace Rotinik.Tests.Features.Medals;

public class MedalTests : IntegrationTestBase
{
    private readonly UserApiClient _userApi;
    private readonly MedalApiClient _medalApi;

    public MedalTests(CustomApiFactory factory) : base(factory)
    {
        _userApi = new UserApiClient(Client);
        _medalApi = new MedalApiClient(Client);
    }

    private async Task<int> SetupUserAsync()
    {
        var newUser = UserDataBuilder.CreateValidRegistrationDto();
        await _userApi.RegisterUserAsync(newUser);

        var token = await _userApi.LoginAndGetTokenAsync(newUser.Email, newUser.Password);
        SetToken(token);

        var meResponse = await _userApi.GetCurrentUserAsync();
        var meJson = await meResponse.Content.ReadFromJsonAsync<JsonElement>();
        return meJson.GetProperty("id").GetInt32();
    }

    [Fact]
    public async Task GetMyMedals_WithoutToken_ReturnsUnauthorized()
    {
        ClearToken();
        var response = await _medalApi.GetMyMedalsAsync();
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyMedals_NewUser_ReturnsEmptyList()
    {
        await SetupUserAsync();

        var response = await _medalApi.GetMyMedalsAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var medals = json.GetProperty("data").EnumerateArray().ToList();

        Assert.Empty(medals);
    }

    [Fact]
    public async Task GetMyMedals_AfterEarningPoints_ReturnsAwardedMedals()
    {
        var userId = await SetupUserAsync();

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var medalService = scope.ServiceProvider.GetRequiredService<MedalService>();

        var medal = new Medal
        {
            Name = "Iniciante",
            Description = "Atingiu 100 pontos!",
            TargetValue = 100,
            TriggerType = MedalTriggerType.TotalPoints,
            IconUrl = "http://icon.com/iniciante.png"
        };
        db.Medals.Add(medal);

        var user = await db.Users.FindAsync(userId);
        user!.Xp = 150;
        await db.SaveChangesAsync();

        await medalService.EvaluateMedalsAsync(userId, MedalTriggerType.TotalPoints);

        var response = await _medalApi.GetMyMedalsAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var medals = json.GetProperty("data").EnumerateArray().ToList();

        Assert.Single(medals);
        Assert.Equal("Iniciante", medals[0].GetProperty("medal").GetProperty("name").GetString());
    }

    [Fact]
    public async Task CheckAndAwardMedals_CalledTwice_DoesNotDuplicateMedal()
    {
        var userId = await SetupUserAsync();

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var medalService = scope.ServiceProvider.GetRequiredService<MedalService>();

        // Replaced PointsThreshold and added TriggerType
        db.Medals.Add(new Medal { Name = "Prata", TargetValue = 200, TriggerType = MedalTriggerType.TotalPoints, Description = "Desc", IconUrl = "Url" });

        var user = await db.Users.FindAsync(userId);
        user!.Xp = 250;
        await db.SaveChangesAsync();

        await medalService.EvaluateMedalsAsync(userId, MedalTriggerType.TotalPoints);
        await medalService.EvaluateMedalsAsync(userId, MedalTriggerType.TotalPoints);

        var response = await _medalApi.GetMyMedalsAsync();
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var medals = json.GetProperty("data").EnumerateArray().ToList();

        Assert.Single(medals);
    }
}