using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Models;

namespace Tests.Helpers
{
    internal static class Get
    {
        public static IEnumerable<Person> People()
        {
            var i = 0;
            var random = new Random();

            while (true)
            {
                yield return new Person(prefill: true, id: i++.ToString())
                {
                    Age = random.Next(80)
                };
            }
        }
        public static IEnumerable<Company> Companies()
        {
            var i = 0;

            while (true)
            {
                yield return new Company(prefill: true, id: i++.ToString());
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
            return new Person(prefill: true, id: id);
        }
        public static Company Company(string id = "456")
        {
            return new Company(prefill: true, id: id);
        }
    }
}
