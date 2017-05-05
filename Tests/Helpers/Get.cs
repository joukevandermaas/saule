using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Models;

namespace Tests.Helpers
{
    internal static class Get
    {
        private static readonly Random random = new Random();
        private static readonly string[] FirstNames =
        {
            "Ola", "Lisabeth", "Latoya", "Sabina", "Eugenie", "Francisco",
            "Kenneth", "Teofila", "Honey", "Larissa", "Dannie", "Nery", "Oswaldo",
            "Romona", "Ollie", "Vergie", "Lolita", "Jung", "Sheba", "Fonda",
        };

        private static readonly string[] LastNames =
        {
            "Summers", "Bockman", "Duque", "Cline", "Neufeld", "Mcray", "Hix",
            "Daniel", "Baumbach", "Forry", "Bozek", "Chichester", "Petri", "Folk",
            "Yadon", "Holliday", "Paniagua", "Hofstetter", "Vasques", "Russel"
        };

        private static readonly string[] StreetNames =
        {
            "Buttonwood Drive", "Cottage Street", "12th Street", "Dogwood Lane",
            "Atlantic Avenue", "Lincoln Avenue", "Route 10", "Water Street",
            "Brookside Drive", "Hillcrest Drive", "Madison Avenue", "Union Street",
            "Lake Avenue", "6th Street", "Broad Street West", "Market Street",
            "North Street", "Heritage Drive", "Cooper Street", "Route 44"
        };

        public static IEnumerable<Person> People()
        {
            var i = 0;
            while (true)
            {
                yield return Person(i++.ToString());
            }
        }

        public static IEnumerable<Company> Companies()
        {
            var i = 0;

            while (true)
            {
                yield return Company(i++.ToString());
            }
        }

        public static IEnumerable<Customer> Customers()
        {
            var i = 0;

            while (true)
            {
                yield return Customer(i++.ToString());
            }
        }

        public static IEnumerable<Person> People(int count)
        {
            return People().Take(count);
        }
        public static IEnumerable<Company> Companies(int count)
        {
            return Companies().Take(count);
        }

        public static IEnumerable<Customer> Customers(int count)
        {
            return Customers().Take(count);
        }

        public static Person Person(string id = "123")
        {
            return new Person(prefill: true, id: id)
            {
                Age = random.Next(maxValue: 80),
                FirstName = FirstNames[random.Next(FirstNames.Length)],
                LastName = LastNames[random.Next(LastNames.Length)],
                Address = new Address
                {
                    StreetName = StreetNames[random.Next(StreetNames.Length)],
                    ZipCode = random.Next(minValue: 10000, maxValue: 99999).ToString()
                },
                Job = Company(),
                Car = Car()
            };
        }
        public static Company Company(string id = "456")
        {
            return new Company(prefill: true, id: id)
            {
                Location = (LocationType)random.Next(Enum.GetNames(typeof(LocationType)).Length)
            };
        }

        public static Customer Customer(string id = "789")
        {
            return new Customer(prefill: true, id: id);
        }
        
        public static Car Car(string id = "12")
        {
            return new Car(prefill: true, id: id);
        }
    }
}