using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Helpers;

namespace Tests.Models
{
    public class PersonWithJsonConverters
    {
        public PersonWithJsonConverters()
        {
        }

        public string Id { get; set; } = "1";

        public string Name { get; set; } = "Dustin";

        [JsonConverter(typeof(DateISOConverter))]
        public DateTime Birthday { get; set; } = new DateTime(1992, 9, 28);

        public DateTime WorkAniversary { get; set; } = new DateTime(2019, 4, 27);
    }


}
