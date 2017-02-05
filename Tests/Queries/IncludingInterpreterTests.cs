using System.Collections.Generic;
using System.Linq;
using Saule;
using Saule.Queries;
using Saule.Queries.Including;
using Tests.Helpers;
using Tests.Models;
using Xunit;

namespace Tests.Queries
{
    public class IncludingInterpreterTests
    {
        private static IncludingContext DefaultContext => new IncludingContext(GetQuery("id"));

        [Theory(DisplayName = "Parses includes correctly")]
        [InlineData("shortProfileDetail", new[] { "ShortProfileDetail" })]
        [InlineData("shortProfileDetail,longProfileDetail,security,owner,versionCreator", new[] { "ShortProfileDetail", "LongProfileDetail", "Security", "Owner", "VersionCreator" })]
        internal void ParsesCorrectly(string query, string[] includes)
        {
            var context = new IncludingContext(GetQuery(query));
            var expected = includes.Select((s) => new
            {
                Name = s
            }).ToList();
            var actual = context.Includes.Select(p => new
            {
                p.Name
            }).ToList();

            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Parses empty query string correctly")]
        public void ParsesEmpty()
        {
            var context = new IncludingContext(Enumerable.Empty<KeyValuePair<string, string>>());

            var actual = context.Includes;

            Assert.Equal(Enumerable.Empty<IncludingProperty>(), actual);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetQuery(string query)
        {
            yield return new KeyValuePair<string, string>(
                Constants.QueryNames.Including, query);
        }
    }
}
