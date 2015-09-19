using Saule;
using System.Collections.Generic;

namespace Tests.Helpers
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
        public Person()
        {
        }

        public Person(bool prefill = false, string id = "123")
        {
            if (prefill)
            {
                Id = id;
                FirstName = "John";
                LastName = "Smith";
                Age = 34;
                NumberOfLegs = 4;
                Job = new Company(true);
                Friends = new List<Person>();
            }
        }

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int NumberOfLegs { get; set; }
        public Company Job { get; set; }
        public IEnumerable<Person> Friends { get; set; }
    }

    public class Company
    {
        public Company(bool prefill = false)
        {
            if (prefill)
            {
                Id = "456";
                Name = "Awesome, Inc.";
                NumberOfEmployees = 24;
            }
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int NumberOfEmployees { get; set; }
    }

    public class PersonResource : ApiResource
    {
        public PersonResource()
        {
            Attribute("FirstName");
            Attribute("LastName");
            Attribute("Age");

            BelongsTo<CompanyResource>("Job", "/employer");
            HasMany<PersonResource>("Friends");
        }
    }

    public class CompanyResource : ApiResource
    {
        public CompanyResource()
        {
            OfType("Coorporation");
            Attribute("Name");
            Attribute("NumberOfEmployees");
        }
    }
}