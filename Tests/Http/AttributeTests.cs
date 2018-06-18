using System;
using System.Collections.Generic;
using Saule.Http;
using Tests.Models;
using Xunit;
using System.Threading.Tasks;
using Tests.Helpers;

namespace Tests.Http
{
    public class AttributeTests
    {
        [Theory(DisplayName = "PaginatedAttribute does not allow PerPage < 1")]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(-512)]
        public void PerPageLargerThanOne(int count)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new PaginatedAttribute { PerPage = count });
        }

        [Theory(DisplayName = "ReturnsResourceAttribute only allows types that extend ApiResource")]
        [InlineData(typeof(string))]
        [InlineData(typeof(IDictionary<,>))]
        [InlineData(typeof(int))]
        [InlineData(typeof(LocationType))]
        [InlineData(typeof(List<>))]
        public void OnlyAllowApiResource(Type type)
        {
            Assert.Throws<ArgumentException>(() =>
                new ReturnsResourceAttribute(type));
        }

        [Fact(DisplayName = "JsonApiAttribute responds to HttpGets with Json Api even when accept header is not 'application/vnd.api+json'")]
        public async Task JsonApiAttributeRespondsWithJsonApi()
        {
            using (var server = new NewSetupJsonApiServer())
            {
                var client = server.GetClient(addDefaultHeaders: false);

                var result = await client.GetJsonResponseAsync("api/people/123/usingJsonApiAttributeFilter");

                Assert.NotNull(result["data"]["attributes"]["first-name"]);
            }
        }
    }
}
