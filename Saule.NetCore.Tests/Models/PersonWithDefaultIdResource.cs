using Saule;

namespace Tests.Models
{
    public class PersonWithDefaultIdResource : PersonResource
    {
        public PersonWithDefaultIdResource()
        {
            WithId("Id");
        }
    }
}