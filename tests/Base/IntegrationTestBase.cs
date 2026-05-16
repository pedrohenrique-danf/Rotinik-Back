using System.Net.Http.Headers;
using Rotinik.Tests.Setup;
using Xunit;

namespace Rotinik.Tests.Base;

[Collection("Api Tests")]
public abstract class IntegrationTestBase
{
    protected readonly HttpClient Client;

    protected IntegrationTestBase(Rotinik.Tests.Setup.CustomApiFactory factory)
    {
        Client = factory.CreateClient();
    }

    protected void SetAuthorizationHeader(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}