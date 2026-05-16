using Xunit;

namespace Rotinik.Tests.Setup;

[CollectionDefinition("Api Tests")]
public class SharedTestCollection : ICollectionFixture<CustomApiFactory>
{
    
}