using Saule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Models
{
    public class PersonWithJsonConvertersResource : ApiResource
    {
        public PersonWithJsonConvertersResource()
        {
            OfType("Person");
            WithId(nameof(PersonWithJsonConverters.Id));
            Attribute(nameof(PersonWithJsonConverters.Name));
            Attribute(nameof(PersonWithJsonConverters.Birthday));
            Attribute(nameof(PersonWithJsonConverters.WorkAniversary));
        }
    }
}
