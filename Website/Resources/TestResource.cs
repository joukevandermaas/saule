using Saule;

namespace Website.Controllers
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
        }
    }
}