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
        public void AttributesComplete()
        {
            var person = new Person();
            var target = new JsonApiSerializer();
            var result = target.Serialize(person.ToApiResponse(typeof(PersonResource)), "/api/people/1");

            var attributes = result["data"]["attributes"];
            Assert.Equal(person.FirstName, attributes.Value<string>("firstName"));
            Assert.Equal(person.LastName, attributes.Value<string>("lastName"));
            Assert.Equal(person.Age, attributes.Value<int>("age"));
        }

        [Fact(DisplayName = "Serializes no extra properties")]
        public void AttributesSufficient()
        {
            var person = new Person();
            var target = new JsonApiSerializer();
            var result = target.Serialize(person.ToApiResponse(typeof(PersonResource)), "/api/people/1");

            var attributes = result["data"]["attributes"];
            Assert.True(attributes["numberOfLegs"] == null);
            Assert.Equal(3, attributes.Count());
        }
        
        [Fact(DisplayName = "Uses type name from model definition")]
        public void UsesTitle()
        {
            var company = new Company();
            var target = new JsonApiSerializer();
            var result = target.Serialize(company.ToApiResponse(typeof(CompanyResource)), "/api/companies/1");

            Assert.Equal("coorporation", result["data"]["type"]);
        }

        [Fact(DisplayName ="Serializes relationships' links")]
        public void SerializesRelationshipLinks()
        {
            var person = new Person();
            var target = new JsonApiSerializer();
            var result = target.Serialize(person.ToApiResponse(typeof(PersonResource)), "/api/people/1");

            var relationships = result["data"]["relationships"];
            var job = relationships["job"];
            var friends = relationships["friends"];

            Assert.Equal("/api/people/1/employer", job["links"]["related"]);
            Assert.Equal("/api/people/1/relationships/employer", job["links"]["self"]);

            Assert.Equal("/api/people/1/friends", friends["links"]["related"]);
            Assert.Equal("/api/people/1/relationships/friends", friends["links"]["self"]);
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
                Attribute("FirstName");
                Attribute("LastName");
                Attribute("Age");

                BelongsTo("Job", typeof(CompanyResource), "/employer");
                HasMany("Friends", typeof(PersonResource));
            }
        }
        private class CompanyResource : ApiResource
        {
            public CompanyResource()
            {
                WithType("Coorporation");
                Attribute("Name");
                Attribute("NumberOfEmployees");
            }
        }
    }
}
