using Saule;

namespace Website.Resources
{
    internal class TestResource : ApiResource
    {
        public TestResource()
        {
            OfType("some very nice type name");

            Attribute("Name");

            HasMany<OtherResource>("Interests", "/hobbies/");
        }
    }

    internal class OtherResource : ApiResource
    {
        public OtherResource()
        {
            HasMany<TestResource>("Tests");
        }
    }
}