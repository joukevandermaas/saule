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

        private class TestApiResource3 : ApiResource
        {
            public TestApiResource3()
            {
                Attribute("Id");
            }
        }

        private class TestApiResource4 : ApiResource
        {
            public TestApiResource4()
            {
                HasMany<TestApiResource>("Id");
            }
        }

        private class TestApiResource5 : ApiResource
        {
            public TestApiResource5()
            {
                BelongsTo<TestApiResource>("Id");
            }
        }

        [Fact(DisplayName = "Model name defaults to class name")]
        public void UsesClassName()
        {
            var model = new TestApiResource();

            Assert.Equal("test-api", model.ResourceType);

            var model2 = new TestApiResource2();

            Assert.Equal("test-api-resource2", model2.ResourceType);
        }

        [Fact(DisplayName = "Can't add attribute or relationship called 'id'")]
        public void CannotAddIdAttributeOrRelationship()
        {
            Assert.Throws<JsonApiException>(() => { new TestApiResource3(); });
            Assert.Throws<JsonApiException>(() => { new TestApiResource4(); });
            Assert.Throws<JsonApiException>(() => { new TestApiResource5(); });
        }
    }
}