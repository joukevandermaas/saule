using Saule.Common.Tests.Models;
using Saule.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Saule.Common.Tests.Serialization
{
    public class ResourceGraphNodeKeyTests
    {
        private readonly ITestOutputHelper _output;

        public ResourceGraphNodeKeyTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Node key with no id throws server exception")]
        public void NoIdThrowsServerException()
        {
            var model = new
            {
                description= "Has no id property"
            };

            var resource = new PersonResource();

            Assert.Throws<JsonApiException>(() => new ResourceGraphNodeKey(model, resource));
        }

        [Fact(DisplayName = "Node key equality works")]
        public void EqualityWorks()
        {
            var key1 = new ResourceGraphNodeKey("model", "1");
            var key2 = new ResourceGraphNodeKey("model", "1");
            var key3 = new ResourceGraphNodeKey("model", "2");

            Assert.True(key1 == key2);
            Assert.False(key2 == key3);
            Assert.True(key1.Equals(key2));
            Assert.False(key2.Equals(key3));
        }
    }
}