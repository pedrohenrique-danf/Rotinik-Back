using Xunit;

namespace Rotinik.Tests.Core;

[CollectionDefinition("Api Tests")]
public class SharedTestCollection : ICollectionFixture<CustomApiFactory>
{
    
}