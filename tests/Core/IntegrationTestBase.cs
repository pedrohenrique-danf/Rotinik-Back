using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Rotinik.Core.Data;
using Xunit;

namespace Rotinik.Tests.Core;

[Collection("Api Tests")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly HttpClient Client;
    
    protected readonly CustomApiFactory Factory; 

    protected IntegrationTestBase(CustomApiFactory factory)
    {
        Factory = factory; 
        Client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    protected void SetToken(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    
    protected void ClearToken()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }
}