using System.Net.Http.Headers;
using Xunit;

namespace Rotinik.Tests.Core;

[Collection("Api Tests")]
public abstract class IntegrationTestBase
{
    protected readonly HttpClient Client;

    protected IntegrationTestBase(CustomApiFactory factory)
    {
        Client = factory.CreateClient();
    }

    protected void SetToken(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    
    protected void ClearToken()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }
}