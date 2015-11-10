using System.Collections.Generic;

namespace Tests.Models
{
    public class PersonWithNoJob
    {
        public string Id => "123";
        public string FirstName => "John";
        public string LastName => "Smith";
        public int Age => 34;
        public int NumberOfLegs => 2;
        public IEnumerable<Person> Friends => new List<Person>();
    }
}