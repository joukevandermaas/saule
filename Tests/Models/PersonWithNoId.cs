using System.Collections.Generic;

namespace Tests.Models
{
    public class PersonWithNoId
    {
        public string FirstName => "John";
        public string LastName => "Smith";
        public int Age => 34;
        public int NumberOfLegs => 2;
        public Company Job => new Company();
        public IEnumerable<Person> Friends => new List<Person>();
    }
}