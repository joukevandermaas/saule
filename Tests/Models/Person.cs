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
            FamilyMembers = new List<Person>();
            Friends = new List<Person>();
        }

        public string Identifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Age { get; set; }
        public int NumberOfLegs { get; set; }
        public Company Job { get; set; }
        public Address Address { get; set; }
        public Car Car { get; set; }
        public IEnumerable<Person> FamilyMembers { get; set; }
        public IEnumerable<Person> Friends { get; set; }
    }

    public class Address
    {
        public string StreetName { get; set; }
        public string ZipCode { get; set; }
    }
}