using Saule;
using Xunit;

namespace Tests
{
    public class ApiResourceTests
    {
        private class TestApiResource : ApiResource
        { }

        private class TestApiResource2 : ApiResource
        { }

        [Fact(DisplayName = "Model name defaults to class name")]
        public void UsesClassName()
        {
            var model = new TestApiResource();

            Assert.Equal("test-api", model.ResourceType);

            var model2 = new TestApiResource2();

            Assert.Equal("test-api-resource2", model2.ResourceType);
        }
    }
}