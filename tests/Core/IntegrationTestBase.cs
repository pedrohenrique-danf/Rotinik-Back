using Xunit;

namespace Rotinik.Tests.Core;

[Collection("Api Tests")]
public abstract class IntegrationTestBase
{
    protected readonly RotinikApiClient ApiClient;

    protected IntegrationTestBase(CustomApiFactory factory)
    {
        var httpClient = factory.CreateClient();
        ApiClient = new RotinikApiClient(httpClient);
    }
}