using System.Collections.Generic;
using System.Linq;
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
            var target = new PaginationInterpreter(DefaultContext);

            Assert.Throws<JsonApiException>(() => target.Apply(people));
        }

        [Fact(DisplayName = "Does not do anything to non-enumerables/queryables")]
        public void IgnoresNonEnumerables()
        {
            var obj = Get.Person();
            Query.ApplyPagination(obj, DefaultContext);
        }

        [Fact(DisplayName = "Orders queryables before pagination")]
        public void OrdersQueryables()
        {
            var obj = new NotOrderedQueryable<Person>(Get.People(100)
                .OrderByDescending(p => p.Id)
                .AsQueryable());

            var result = (Query.ApplyPagination(obj, DefaultContext)
                as IEnumerable<Person>)?.ToList();

            Assert.Equal("0", result?.FirstOrDefault()?.Id);
            Assert.Equal("1", result?.Skip(1).FirstOrDefault()?.Id);
        }

        [Fact(DisplayName = "Does not order enumerables before pagination")]
        public void DoesNotOrderEnumerables()
        {
            var obj = new NotOrderedQueryable<Person>(Get.People(100)
                .OrderByDescending(p => p.Id)
                .AsQueryable()).ToList();

            var result = (Query.ApplyPagination(obj, DefaultContext)
                as IEnumerable<Person>)?.ToList();

            Assert.Equal("99", result?.FirstOrDefault()?.Id);
            Assert.Equal("98", result?.Skip(1).FirstOrDefault()?.Id);
        }

        [Fact(DisplayName = "Takes the correct elements based on the parameters")]
        public void TakesCorrectElements()
        {
            var obj = Get.People(100);
            var context = new PaginationContext(GetQueryForPage(1), 10);
            var result = (Query.ApplyPagination(obj, context)
                as IEnumerable<Person>)?.ToList();

            Assert.Equal(10, result?.Count);
            Assert.Equal("10", result?.FirstOrDefault()?.Id);
        }

        [Fact(DisplayName = "Works on an empty enumerable")]
        public void WorksOnEmptyEnumerable()
        {
            var obj = Enumerable.Empty<Person>();
            var result = Query.ApplyPagination(obj, DefaultContext)
                as IEnumerable<Person>;

            Assert.Equal(false, result?.Any());
        }

        [Fact(DisplayName = "Does not order already ordered queryable")]
        public void RespectsOrderedQueryable()
        {
            var obj = new NotOrderedQueryable<Person>(Get.People(100).AsQueryable())
                .OrderByDescending(p => p.Id);
            var result = (Query.ApplyPagination(obj, DefaultContext)
                as IEnumerable<Person>)?.ToList();

            Assert.Equal("99", result?.FirstOrDefault()?.Id);
            Assert.Equal("98", result?.Skip(1).FirstOrDefault()?.Id);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetQueryForPage(int number)
        {
            yield return new KeyValuePair<string, string>(Constants.PageNumberQueryName, number.ToString());
        }
    }
}
