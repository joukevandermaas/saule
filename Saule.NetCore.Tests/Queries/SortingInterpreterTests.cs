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
            var result = Query.ApplySorting(obj, DefaultContext, new PersonResource());

            Assert.Same(obj, result);
        }

        [Fact(DisplayName = "Doesn't do anything on empty queryable/enumerable")]
        public void EmptyIsNoop()
        {
            var target = new SortingInterpreter(DefaultContext, new PersonResource());
            var enumerableResult = target.Apply(Enumerable.Empty<Person>()) as IEnumerable<Person>;
            var queryableResult = target.Apply(Enumerable.Empty<Person>().AsQueryable()) as IQueryable<Person>;

            Assert.Equal(Enumerable.Empty<Person>().ToList(), enumerableResult.ToList());
            Assert.Equal(Enumerable.Empty<Person>().AsQueryable(), queryableResult);
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

        [Fact(DisplayName = "Parses empty query string correctly")]
        public void ParsesEmpty()
        {
            var context = new SortingContext(Enumerable.Empty<KeyValuePair<string, string>>());

            var actual = context.Properties;

            Assert.Equal(Enumerable.Empty<SortingProperty>(), actual);
        }

        [Fact(DisplayName = "Applies sorting order (asc,desc)")]
        public void AppliesSortingAscDesc()
        {
            var people = Get.People(100).ToList().AsQueryable();
            var expected = people.OrderBy(p => p.Age).ThenByDescending(p => p.Identifier);

            var result = Query.ApplySorting(people, new SortingContext(GetQuery("age,-id")), new PersonResource())
                as IQueryable<Person>;

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Applies sorting order (asc,asc)")]
        public void AppliesSortingAscAsc()
        {
            var people = Get.People(100).ToList().AsQueryable();
            var expected = people.OrderBy(p => p.Age).ThenBy(p => p.Identifier);

            var result = Query.ApplySorting(people, new SortingContext(GetQuery("age,id")), new PersonResource())
                as IQueryable<Person>;

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Applies expected sorting (asc, asc) to enumerables")]
        public void AppliesSortingAscAscToEnumerables()
        {
            var people = Get.People(100).ToList();
            var expected = people.OrderBy(p => p.Age).ThenBy(p => p.Identifier);

            var result = Query.ApplySorting(people, new SortingContext(GetQuery("age,id")), new PersonResource())
                as IEnumerable<Person>;

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Applies expected sorting (asc, desc) to enumerables")]
        public void AppliesSortingAscDescToEnumerables()
        {
            var people = Get.People(100).ToList();
            var expected = people.OrderBy(p => p.Age).ThenByDescending(p => p.Identifier);

            var result = Query.ApplySorting(people, new SortingContext(GetQuery("age,-id")), new PersonResource())
                as IEnumerable<Person>;

            Assert.Equal(expected, result);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetQuery(string query)
        {
            yield return new KeyValuePair<string, string>(
                Constants.QueryNames.Sorting, query);
        }
    }
}
