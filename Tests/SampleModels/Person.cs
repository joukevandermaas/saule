using Saule;
using System.Collections.Generic;

namespace Tests.SampleModels
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
    public class PersonWithNoJob
    {
        public string Id => "123";
        public string FirstName => "John";
        public string LastName => "Smith";
        public int Age => 34;
        public int NumberOfLegs => 2;
        public IEnumerable<Person> Friends => new List<Person>();
    }
    public class Person
    {
        public string Id => "123";
        public string FirstName => "John";
        public string LastName => "Smith";
        public int Age => 34;
        public int NumberOfLegs => 2;
        public Company Job => new Company();
        public IEnumerable<Person> Friends => new List<Person>();
    }
    public class Company
    {
        public string Id => "456";
        public string Name => "Name";
        public int NumberOfEmployees => 24;
    }
    public class PersonResource : ApiResource
    {
        public PersonResource()
        {
            Attribute("FirstName");
            Attribute("LastName");
            Attribute("Age");

            BelongsTo("Job", typeof(CompanyResource), "/employer");
            HasMany("Friends", typeof(PersonResource));
        }
    }
    public class CompanyResource : ApiResource
    {
        public CompanyResource()
        {
            WithType("Coorporation");
            Attribute("Name");
            Attribute("NumberOfEmployees");
        }
    }
}
