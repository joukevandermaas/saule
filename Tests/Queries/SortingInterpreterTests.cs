using System;
using System.Collections.Generic;
using System.Linq;
using Saule;
using Saule.Queries;
using Saule.Queries.Sorting;
using Tests.Helpers;
using Tests.Models;
using Xunit;

namespace Tests.Queries
{
    public class SortingInterpreterTests
    {
        private static SortingContext DefaultContext => new SortingContext(GetQuery("id"));

        [Fact(DisplayName = "Does not do anything to non-enumerables/queryables")]
        public void IgnoresNonEnumerables()
        {
            var obj = Get.Person();
            Query.ApplySorting(obj, DefaultContext);
        }

        [Theory(DisplayName = "Parses sorting string value correctly")]
        [InlineData("+id", new[] { "Id" }, new[] { SortingDirection.Ascending })]
        [InlineData("-id", new[] { "Id" }, new[] { SortingDirection.Descending })]
        [InlineData("id", new[] { "Id" }, new[] { SortingDirection.Ascending })]
        [InlineData("id,-age", new[] { "Id", "Age" }, new[] { SortingDirection.Ascending, SortingDirection.Descending })]
        [InlineData("+id,age", new[] { "Id", "Age" }, new[] { SortingDirection.Ascending, SortingDirection.Ascending })]
        [InlineData("+id,-id", new[] { "Id", "Id" }, new[] { SortingDirection.Ascending, SortingDirection.Descending })]
        internal void ParsesCorrectly(string query, string[] properties, SortingDirection[] directions)
        {
            var context = new SortingContext(GetQuery(query));
            var expected = properties.Zip(directions, (s, d) => new
            {
                Name = s,
                Direction = d
            }).ToList();
            var actual = context.Properties.Select(p => new
            {
                p.Name,
                p.Direction
            }).ToList();

            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Applies expected sorting to queryables")]
        public void AppliesSorting()
        {
            var people = Get.People(100).ToList().AsQueryable();
            var expected = people.OrderBy(p => p.Age).ThenByDescending(p => p.Id);

            var result = Query.ApplySorting(people, new SortingContext(GetQuery("age,-id")))
                as IQueryable<Person>;

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Applies expected sorting to enumerables")]
        public void AppliesSortingToEnumerables()
        {
            var people = Get.People(100).ToList();
            var expected = people.OrderBy(p => p.Age).ThenByDescending(p => p.Id);

            var result = Query.ApplySorting(people, new SortingContext(GetQuery("age,-id")))
                as IEnumerable<Person>;

            Assert.Equal(expected, result);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetQuery(string query)
        {
            yield return new KeyValuePair<string, string>(
                Constants.SortingQueryName, query);
        }
    }
}
