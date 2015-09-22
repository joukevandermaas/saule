using Saule.Serialization;
using Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Serialization
{
    public class DefaultUrlPathBuilderTests
    {
        private readonly ITestOutputHelper _output;

        public DefaultUrlPathBuilderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Collection uses ApiResource.UrlPath")]
        public void UseUrlPath()
        {
            var target = new DefaultUrlPathBuilder();
            var result =target.BuildCanonicalPath(new PersonResource());
            _output.WriteLine(result);

            Assert.Equal("/people/", result);
        }
    }
}
