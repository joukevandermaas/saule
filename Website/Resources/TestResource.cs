using Saule;

namespace Website.Controllers
{
    internal class TestResource : ApiResource
    {
        public TestResource()
        {
            OfType("some very nice type name");

            Attribute("Id");
            Attribute("Name");

            HasMany("Interests", typeof(OtherResource), "/hobbies/");
        }
    }

    internal class OtherResource : ApiResource
    {
        public OtherResource()
        {
        }
    }
}