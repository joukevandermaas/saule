using System.Collections.Generic;
using Tests.Helpers;

namespace Tests.Models
{
    public class PersonWithDifferentId
    {
        public PersonWithDifferentId(bool prefill = false, string id = "123")
        {
            PersonId = id;
            if (!prefill) return;

            FirstName = "John";
            LastName = "Smith";
            Age = 34;
            NumberOfLegs = 4;
            Job = new CompanyWithDifferentId(prefill: true);
            Friends = new List<PersonWithDifferentId>();
        }

        public string PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int NumberOfLegs { get; set; }
        public CompanyWithDifferentId Job { get; set; }
        public IEnumerable<PersonWithDifferentId> Friends { get; set; }
    }
}