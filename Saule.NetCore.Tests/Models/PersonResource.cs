using System;
using System.Collections.Generic;
using System.Linq;
using Saule;

namespace Tests.Models
{
    public class PersonResource : ApiResource
    {
        public PersonResource()
        {
            WithId(nameof(Person.Identifier));

            Attribute(nameof(Person.FirstName));
            Attribute(nameof(Person.LastName));
            Attribute(nameof(Person.Age));
            Attribute(nameof(Person.Address));

            BelongsTo<CompanyResource>(nameof(Person.Job), "/employer");
            BelongsTo<CarResource>(nameof(Person.Car));
            HasMany<PersonResource>(nameof(Person.Friends));
            HasMany<PersonResource>("SecretFriends");
            HasMany<PersonResource>(nameof(Person.FamilyMembers));
        }

        public override object GetMetadata(object response, Type resourceType, bool isEnumerable)
        {
            if (isEnumerable || resourceType != typeof(Person))
            {
                return null;
            }

            var person = (Person)response;

            var friends = person.Friends?.Count();
            var family = person.FamilyMembers?.Count();

            if (friends == null || family == null)
            {
                return null;
            }

            return new PersonMetadata
            {
                NumberOfFriends = friends.Value,
                NumberOfFamilyMembers = family.Value
            };
        }

        private class PersonMetadata
        {
            public int NumberOfFriends { get; set; }
            public int NumberOfFamilyMembers { get; set; }
        }
    }
}