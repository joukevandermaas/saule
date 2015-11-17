using Saule.Serialization;
using Tests.Helpers;
using Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Serialization
{
    public class DefaultUrlPathBuilderTests
    {
        // todo: unit test DefaultUrlPathBuilder
        // All four methods use prefix if it exists, default to '/'
        // All four methods use ApiResource for url gen where possible
        // Given person resource, all four give the correct response
        private readonly ITestOutputHelper _output;

        public DefaultUrlPathBuilderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Collection uses ApiResource.UrlPath")]
        public void UseUrlPath()
        {
            var target = new DefaultUrlPathBuilder();
            var result = target.BuildCanonicalPath(new PersonResource());
            _output.WriteLine(result);

            Assert.Equal("/people/", result);
        }
    }
}
