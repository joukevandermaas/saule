using Saule;
using Saule.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class JsonApiSerializerTests
    {
        [Fact(DisplayName = "Serializes all found attributes")]
        public void TestAttributesComplete()
        {
            var person = new Person();
            var target = new JsonApiSerializer();
            var result = target.Serialize(person.ToApiResponse(typeof(PersonModel)));

            var attributes = result["data"]["attributes"];
            Assert.Equal(person.FirstName, attributes.Value<string>("firstName"));
            Assert.Equal(person.LastName, attributes.Value<string>("lastName"));
            Assert.Equal(person.Age, attributes.Value<int>("age"));
        }

        [Fact(DisplayName = "Serializes no extra properties")]
        public void TestAttributesSufficient()
        {
            var person = new Person();
            var target = new JsonApiSerializer();
            var result = target.Serialize(person.ToApiResponse(typeof(PersonModel)));

            var attributes = result["data"]["attributes"];
            Assert.True(attributes["numberOfLegs"] == null);
            Assert.Equal(3, attributes.Count());
        }
        
        [Fact(DisplayName = "Uses type name from model definition")]
        public void TestUsesTitle()
        {
            var company = new Company();
            var target = new JsonApiSerializer();
            var result = target.Serialize(company.ToApiResponse(typeof(CompanyResource)));

            Assert.Equal("coorporation", result["data"]["type"]);
        }

        public class Person
        {
            public string FirstName => "John";
            public string LastName => "Smith";
            public int Age => 34;
            public int NumberOfLegs => 2;
            public Company Job => new Company();
            public IEnumerable<Person> Friends => new List<Person>();
        }
        public class Company
        {
            public string Name => "Name";
            public int NumberOfEmployees => 24;
        }
        private class PersonResource : ApiResource
        {
            public PersonResource()
            {
                WithAttribute("FirstName");
                WithAttribute("LastName");
                WithAttribute("Age");

                BelongsTo("Job", typeof(CompanyResource));
                HasMany("Friends", typeof(PersonResource));
            }
        }
        private class CompanyResource : ApiResource
        {
            public CompanyResource()
            {
                WithType("Coorporation");
                WithAttribute("Name");
                WithAttribute("NumberOfEmployees");
            }
        }
    }
}
