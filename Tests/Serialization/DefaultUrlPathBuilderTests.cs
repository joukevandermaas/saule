using Saule.Serialization;
using Tests.Helpers;
using Xunit;

namespace Tests.Serialization
{
    public class DefaultUrlPathBuilderTests
    {
        [Fact(DisplayName = "Collection uses ApiResource.UrlPath")]
        public void UseUrlPath()
        {
            var target = new DefaultUrlPathBuilder();
            var result =target.BuildCanonicalPath(new PersonResource());

            Assert.Equal("/people/", result);
        }
    }
}
