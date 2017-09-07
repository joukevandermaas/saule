using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Saule;
using Saule.Queries;
using Saule.Queries.Pagination;
using Tests.Helpers;
using Tests.Models;
using Xunit;

namespace Tests.Queries
{
    public class PaginationInterpreterTests
    {
        private static PaginationContext DefaultContext => new PaginationContext(GetQueryForPage(0), 10);

        [Fact(DisplayName = "Throws exception when resource doesn't have an Id property")]
        public void ThrowsWhenNoId()
        {
            var people = new NotOrderedQueryable<PersonWithNoId>(new[]
            {
                new PersonWithNoId(),
                new PersonWithNoId(),
                new PersonWithNoId(),
                new PersonWithNoId()
            }.AsQueryable());
            var target = new PaginationInterpreter(DefaultContext, new PersonResource());

            Assert.Throws<JsonApiException>(() => target.Apply(people));
        }

        [Fact(DisplayName = "Does not do anything to non-enumerables/queryables")]
        public void IgnoresNonEnumerables()
        {
            var obj = Get.Person();
            Query.ApplyPagination(obj, DefaultContext, new PersonResource());
        }

        [Fact(DisplayName = "Orders queryables before pagination")]
        public void OrdersQueryables()
        {
            var obj = new NotOrderedQueryable<Person>(Get.People(100)
                .OrderByDescending(p => p.Identifier)
                .AsQueryable());

            var result = (Query.ApplyPagination(obj, DefaultContext, new PersonResource())
                as IEnumerable<Person>)?.ToList();

            Assert.Equal("0", result?.FirstOrDefault()?.Identifier);
            Assert.Equal("1", result?.Skip(1).FirstOrDefault()?.Identifier);
        }

        [Fact(DisplayName = "Does not order enumerables before pagination")]
        public void DoesNotOrderEnumerables()
        {
            var obj = new NotOrderedQueryable<Person>(Get.People(100)
                .OrderByDescending(p => p.Identifier)
                .AsQueryable()).ToList();

            var result = (Query.ApplyPagination(obj, DefaultContext, new PersonResource())
                as IEnumerable<Person>)?.ToList();

            Assert.Equal("99", result?.FirstOrDefault()?.Identifier);
            Assert.Equal("98", result?.Skip(1).FirstOrDefault()?.Identifier);
        }

        [Fact(DisplayName = "Takes the correct elements based on the parameters")]
        public void TakesCorrectElements()
        {
            var obj = Get.People(100);
            var context = new PaginationContext(GetQueryForPage(1), 10);
            var result = (Query.ApplyPagination(obj, context, new PersonResource())
                as IEnumerable<Person>)?.ToList();

            Assert.Equal(10, result?.Count);
            Assert.Equal("10", result?.FirstOrDefault()?.Identifier);
        }

        [Fact(DisplayName = "Works on an empty enumerable")]
        public void WorksOnEmptyEnumerable()
        {
            var obj = Enumerable.Empty<Person>();
            var result = Query.ApplyPagination(obj, DefaultContext, new PersonResource())
                as IEnumerable<Person>;

            Assert.Equal(false, result?.Any());
        }

        [Fact(DisplayName = "Does not order already ordered queryable")]
        public void RespectsOrderedQueryable()
        {
            var obj = new NotOrderedQueryable<Person>(Get.People(100).AsQueryable())
                .OrderByDescending(p => p.Identifier);
            var result = (Query.ApplyPagination(obj, DefaultContext, new PersonResource())
                as IEnumerable<Person>)?.ToList();

            Assert.Equal("99", result?.FirstOrDefault()?.Identifier);
            Assert.Equal("98", result?.Skip(1).FirstOrDefault()?.Identifier);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetQueryForPage(int number)
        {
            yield return new KeyValuePair<string, string>(Constants.QueryNames.PageNumber, number.ToString());
        }
    }
}
