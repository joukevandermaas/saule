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

        public static IEnumerable<Person> People(int count)
        {
            return People().Take(count);
        }
        public static IEnumerable<Company> Companies(int count)
        {
            return Companies().Take(count);
        }

        public static Person Person(string id = "123")
        {
            return new Person(prefill: true, id: id)
            {
                Age = random.Next(80),
                FirstName = FirstNames[random.Next(FirstNames.Length)],
                LastName = LastNames[random.Next(LastNames.Length)]
            };
        }
        public static Company Company(string id = "456")
        {
            return new Company(prefill: true, id: id);
        }
    }
}
