using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Saule;
using Saule.Queries;
using Saule.Queries.Filtering;
using Saule.Queries.Sorting;
using Tests.Helpers;
using Tests.Models;
using Xunit;

namespace Tests.Queries
{
    public class FilteringInterpreterTests
    {
        private static FilteringContext DefaultContext => new FilteringContext(GetQuery(
            new[] { "id" },
            new[] { "5" }));

        [Fact(DisplayName = "Does not do anything to non-enumerables/queryables")]
        public void IgnoresNonEnumerables()
        {
            var obj = Get.Person();
            var result = Query.ApplyFiltering(obj, DefaultContext, new PersonResource());

            Assert.Same(obj, result);
        }

        [Fact(DisplayName = "Doesn't do anything on empty queryable/enumerable")]
        public void EmptyIsNoop()
        {
            var target = new FilteringInterpreter(DefaultContext, new PersonResource());
            var enumerableResult = target.Apply(Enumerable.Empty<Person>()) as IEnumerable<Person>;
            var queryableResult = target.Apply(Enumerable.Empty<Person>().AsQueryable()) as IQueryable<Person>;

            Assert.Equal(Enumerable.Empty<Person>().ToList(), enumerableResult.ToList());
            Assert.Equal(Enumerable.Empty<Person>().AsQueryable(), queryableResult);
        }

        [Theory(DisplayName = "Parses filtering string value correctly")]
        [InlineData(new[] { "id" }, new[] { "5" }, new[] { "Id" })]
        [InlineData(new[] { "firstName" }, new[] { "John" }, new[] { "FirstName" })]
        [InlineData(new[] { "first-name" }, new[] { "John" }, new[] { "FirstName" })]
        [InlineData(new[] { "last-name", "first-name" }, new[] { "Smith", "John" }, new[] { "LastName", "FirstName" })]
        internal void ParsesCorrectly(string[] names, string[] values, string[] properties)
        {
            var context = new FilteringContext(GetQuery(names, values));
            var expected = properties.Zip(values, (n, v) => new
            {
                Name = n,
                Value = v
            }).ToList();
            var actual = context.Properties.Select(p => new
            {
                p.Name,
                p.Value
            }).ToList();

            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Parses empty query string correctly")]
        public void ParsesEmpty()
        {
            var context = new FilteringContext(Enumerable.Empty<KeyValuePair<string, string>>());

            var actual = context.Properties;

            Assert.Equal(Enumerable.Empty<FilteringProperty>(), actual);
        }

        [Fact(DisplayName = "Applies filtering on enums (int)")]
        public void WorksOnEnumsAsNumbers()
        {
            var companies = Get.Companies(100).ToList().AsQueryable();
            var expected = companies.Where(c => c.Location == LocationType.National).ToList();

            var query = GetQuery(new[] { "Location" }, new[] { "1" });

            var result = Query.ApplyFiltering(companies, new FilteringContext(query), new CompanyResource())
                as IQueryable<Company>;

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Applies filtering on ints")]
        public void WorksOnInts()
        {
            var people = Get.People(100).ToList().AsQueryable();
            var expected = people.Where(c => c.Age == 20).ToList();

            var query = GetQuery(new[] { "Age" }, new[] { "20" });

            var result = Query.ApplyFiltering(people, new FilteringContext(query), new PersonResource())
                as IQueryable<Person>;

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Applies filtering on enums (string)")]
        public void WorksOnEnumsAsStrings()
        {
            var companies = Get.Companies(100).ToList().AsQueryable();
            var expected = companies.Where(c => c.Location == LocationType.National).ToList();

            var query = GetQuery(new[] { "Location" }, new[] { "national" });

            var result = Query.ApplyFiltering(companies, new FilteringContext(query), new CompanyResource())
                as IQueryable<Company>;

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Applies filtering on strings")]
        public void WorksOnStrings()
        {
            var people = Get.People(100).ToList().AsQueryable();
            var expected = people.Where(c => c.LastName == "Russel").ToList();

            var query = GetQuery(new[] { "LastName" }, new[] { "Russel" });

            var result = Query.ApplyFiltering(people, new FilteringContext(query), new PersonResource())
                as IQueryable<Person>;

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Works on enumerables")]
        public void WorksOnEnumerables()
        {
            var people = Get.People(100).ToList();
            var expected = people.Where(c => c.LastName == "Russel").ToList();

            var query = GetQuery(new[] { "LastName" }, new[] { "Russel" });

            var result = Query.ApplyFiltering(people, new FilteringContext(query), new PersonResource())
                as IEnumerable<Person>;

            Assert.Equal(expected, result);
        }

        [Fact(DisplayName = "Throws when property does not exist")]
        public void ThrowsOnMissingProperty()
        {
            var enumerable = Get.People(100).ToList();
            var queryable = enumerable.AsQueryable();

            var query = GetQuery(new[] { "Fake" }, new[] { "no" });

            Assert.Throws<JsonApiException>(() =>
                Query.ApplyFiltering(enumerable, new FilteringContext(query), new PersonResource()));
            Assert.Throws<JsonApiException>(() =>
                Query.ApplyFiltering(queryable, new FilteringContext(query), new PersonResource()));
        }

        private static IEnumerable<KeyValuePair<string, string>> GetQuery(IEnumerable<string> properties, IEnumerable<string> values)
        {
            return properties.Zip(values, (property, value) => new KeyValuePair<string, string>(
                $"{Constants.QueryNames.Filtering}.{property}", value));
        }
    }
}
