using System.Collections.Generic;
using System.Linq;
using Saule;
using Saule.Queries.Pagination;
using Xunit;

namespace Tests.Queries
{
    public class PaginationQueryTests
    {
        [Fact(DisplayName = "All properties are null when context is null")]
        public void NoContext()
        {
            var target = new PaginationQuery(null);

            Assert.Null(target.FirstPage);
            Assert.Null(target.NextPage);
            Assert.Null(target.PreviousPage);
        }

        [Fact(DisplayName = "PreviousPage is null when on first page")]
        public void NoPrevious()
        {
            var context = new PaginationContext(GetQuery(Constants.QueryNames.PageNumber, "0"), 10);
            var target = new PaginationQuery(context);

            Assert.Null(target.PreviousPage);
        }

        [Fact(DisplayName = "NextPage is 1 when on first page with no query params")]
        public void NextPageOnFirstPage()
        {
            var context = new PaginationContext(EmptyQuery, 10);
            var target = new PaginationQuery(context);

            Assert.Equal("?page[number]=1", target.NextPage);
        }

        [Fact(DisplayName = "FirstPage is always 0")]
        public void FirstPageWorks()
        {
            var context = new PaginationContext(EmptyQuery, 10);
            var target = new PaginationQuery(context);

            Assert.Equal("?page[number]=0", target.FirstPage);

            context = new PaginationContext(GetQuery(Constants.QueryNames.PageNumber, "0"), 10);
            target = new PaginationQuery(context);

            Assert.Equal("?page[number]=0", target.FirstPage);

            context = new PaginationContext(GetQuery(Constants.QueryNames.PageNumber, "4"), 10);
            target = new PaginationQuery(context);

            Assert.Equal("?page[number]=0", target.FirstPage);
        }

        [Fact(DisplayName = "Does not change other query parameters")]
        public void SupportsOtherQueryParams()
        {
            var context = new PaginationContext(GetQuery(Constants.QueryNames.PageNumber, "3").Concat(GetQuery("something", "hello")), 10);
            var target = new PaginationQuery(context);

            Assert.Equal("?page[number]=4&something=hello", target.NextPage);
            Assert.Equal("?page[number]=2&something=hello", target.PreviousPage);
            Assert.Equal("?page[number]=0&something=hello", target.FirstPage);
        }

        [Fact(DisplayName = "PreviousPage is current - 1 when appropriate")]
        public void PreviousPageWorks()
        {
            var context = new PaginationContext(GetQuery(Constants.QueryNames.PageNumber, "4"), 10);
            var target = new PaginationQuery(context);

            Assert.Equal("?page[number]=3", target.PreviousPage);
        }

        [Fact(DisplayName = "NextPage is current + 1 when appropriate")]
        public void NextPageWorks()
        {
            var context = new PaginationContext(GetQuery(Constants.QueryNames.PageNumber, "4"), 10);
            var target = new PaginationQuery(context);

            Assert.Equal("?page[number]=5", target.NextPage);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetQuery(string key, string value)
        {
            yield return new KeyValuePair<string, string>(key, value);
        }

        private static IEnumerable<KeyValuePair<string, string>> EmptyQuery
            => Enumerable.Empty<KeyValuePair<string, string>>();
    }
}
