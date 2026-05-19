using Rotinik.Tests.Clients;
using Rotinik.Tests.Setup;
using Xunit;

namespace Rotinik.Tests.Base;

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