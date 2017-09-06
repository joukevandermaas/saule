using Saule;
using Xunit;

namespace Tests
{
    public class ApiResourceTests
    {
        private class TestNamingResource : ApiResource
        { }

        private class TestNamingResource2 : ApiResource
        { }

        private class DisallowIdAttr : ApiResource
        {
            public DisallowIdAttr()
            {
                Attribute("Id");
            }
        }

        private class DisallowIdHasMany : ApiResource
        {
            public DisallowIdHasMany()
            {
                HasMany<TestNamingResource>("Id");
            }
        }

        private class DisallowIdBelongsTo : ApiResource
        {
            public DisallowIdBelongsTo()
            {
                BelongsTo<TestNamingResource>("Id");
            }
        }

        private class TestCycles1 : ApiResource
        {
            public TestCycles1()
            {
                BelongsTo<TestCycles2>("Test");
            }
        }

        private class TestCycles2 : ApiResource
        {
            public TestCycles2()
            {
                BelongsTo<TestCycles1>("test12");
                BelongsTo<TestCycles3>("Testtest");
            }
        }

        private class TestCycles3 : ApiResource
        {
            public TestCycles3()
            {
                BelongsTo<TestCycles1>("abc");
            } 
        }

        private class DisallowLinksAttr : ApiResource
        {
            public DisallowLinksAttr()
            {
                Attribute("links");
            }
        }

        private class DisallowRelationshipsAttr : ApiResource
        {
            public DisallowRelationshipsAttr()
            {
                Attribute("relationships");
            }
        }

        [Fact(DisplayName = "Model name defaults to class name")]
        public void UsesClassName()
        {
            var model = new TestNamingResource();

            Assert.Equal("test-naming", model.ResourceType);

            var model2 = new TestNamingResource2();

            Assert.Equal("test-naming-resource2", model2.ResourceType);
        }

        [Fact(DisplayName = "Can't add attribute or relationship called 'id'")]
        public void CannotAddIdAttributeOrRelationship()
        {
            Assert.Throws<JsonApiException>(() => new DisallowIdAttr());
            Assert.Throws<JsonApiException>(() => new DisallowIdHasMany());
            Assert.Throws<JsonApiException>(() => new DisallowIdBelongsTo());
        }

        [Fact(DisplayName = "Can't add attribute named 'relationships'")]
        public void DisallowAttributeNamedRelationships()
        {
            Assert.Throws<JsonApiException>(() => new DisallowRelationshipsAttr());
        }

        [Fact(DisplayName = "Can't add attribute named 'links'")]
        public void DisallowAttributeNamedLinks()
        {
            Assert.Throws<JsonApiException>(() => new DisallowLinksAttr());
        }

        [Fact(DisplayName = "Handles cyclic relationships properly")]
        public void DoesNotBlowUp()
        {
            typeof (TestCycles1).CreateInstance();
        }
    }
}