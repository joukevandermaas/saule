using System.Collections.Generic;
using Tests.Helpers;

namespace Tests.Models
{
    public class Person
    {
        public Person(bool prefill = false, string id = "123")
        {
            Identifier = id;
            if (!prefill) return;

            FirstName = "John";
            LastName = "Smith";
            Age = 34;
            NumberOfLegs = 4;
            Job = Get.Company();
            Friends = new List<Person>();
        }

        public string Identifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Age { get; set; }
        public int NumberOfLegs { get; set; }
        public Company Job { get; set; }
        public IEnumerable<Person> Friends { get; set; }
    }
}