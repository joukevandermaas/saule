using System;
using System.Collections.Generic;
using System.Linq;
using Saule;
using Saule.Serialization;
using Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Serialization
{
    public class DefaultUrlPathBuilderTests
    {
        private readonly ITestOutputHelper _output;

        private static string Id => "123";
        private static ApiResource Resource => new PersonResource();
        private static ResourceRelationship Relationship => new ResourceRelationship<CompanyResource>(
            "job", "/job", RelationshipKind.BelongsTo, new CompanyResource(), LinkType.All);

        public DefaultUrlPathBuilderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetPrefixData()
        {
            var prefixes = new[]
            {
                "/api",
                "/two/parts",
                "/"
            };
            var templates = new[]
            {
                "{controller}/{id}",
                "{controller}/{category}/{id}",
                "people/{id}",
                "people/{id}/employees",
                "people"
            };

            return from prefix in prefixes
                   from template in templates
                   select new object[] { prefix, template };
        }

        [Theory(DisplayName = "Finds correct prefix from template & url")]
        [MemberData("GetPrefixData")]
        public void FindsCorrectPrefix(string prefix, string template)
        {
            var resource = new PersonResource();
            template = $"{prefix}/{template}";

            var builder = new DefaultUrlPathBuilder("/", template);

            var url = builder.BuildCanonicalPath(resource);

            _output.WriteLine($"template: {template}\ngenerated result: {url}");

            Assert.Equal($"{prefix.TrimEnd('/')}/people/", builder.BuildCanonicalPath(resource));
        }

        [Theory(DisplayName = "Finds correct prefix from template, virtual path root & url")]
        [MemberData("GetPrefixData")]
        public void FindsCorrectPrefixWithVirtualPath(string prefix, string template)
        {
            var resource = new PersonResource();
            template = $"test/{template}";

            var builder = new DefaultUrlPathBuilder(prefix, template);

            var url = builder.BuildCanonicalPath(resource);

            _output.WriteLine($"template: {template}\ngenerated result: {url}");

            Assert.Equal($"{prefix.TrimEnd('/')}/test/people/", builder.BuildCanonicalPath(resource));
        }

        [Fact(DisplayName = "Collection uses ApiResource.UrlPath")]
        public void UseUrlPath()
        {
            var target = new DefaultUrlPathBuilder();
            var result = target.BuildCanonicalPath(new PersonResource());
            _output.WriteLine(result);

            Assert.Equal("/people/", result);
        }

        [Fact(DisplayName = "All methods use prefix")]
        public void AddsPrefix()
        {
            var target = new DefaultUrlPathBuilder("my-prefix");

            var result = target.BuildCanonicalPath(Resource);
            _output.WriteLine(result);
            Assert.StartsWith("/my-prefix/", result);

            result = target.BuildCanonicalPath(Resource, Id);
            _output.WriteLine(result);
            Assert.StartsWith("/my-prefix/", result);

            result = target.BuildRelationshipPath(Resource, Id, Relationship);
            _output.WriteLine(result);
            Assert.StartsWith("/my-prefix/", result);

            result = target.BuildRelationshipPath(Resource, Id, Relationship, Id);
            _output.WriteLine(result);
            Assert.StartsWith("/my-prefix/", result);
        }

        [Fact(DisplayName = "Prefix defaults to /")]
        public void HasDefaultPrefix()
        {
            var target = new DefaultUrlPathBuilder();

            var result = target.BuildCanonicalPath(Resource);
            _output.WriteLine(result);
            Assert.StartsWith("/people/", result);

            result = target.BuildCanonicalPath(Resource, Id);
            _output.WriteLine(result);
            Assert.StartsWith("/people/", result);

            result = target.BuildRelationshipPath(Resource, Id, Relationship);
            _output.WriteLine(result);
            Assert.StartsWith("/people/", result);

            result = target.BuildRelationshipPath(Resource, Id, Relationship, Id);
            _output.WriteLine(result);
            Assert.StartsWith("/people/", result);
        }

        [Fact(DisplayName = "Gives correct results")]
        public void GivesCorrectResults()
        {
            var target = new DefaultUrlPathBuilder();

            var result = target.BuildCanonicalPath(Resource);
            _output.WriteLine(result);
            Assert.Equal("/people/", result);

            result = target.BuildCanonicalPath(Resource, Id);
            _output.WriteLine(result);
            Assert.Equal("/people/123/", result);

            result = target.BuildRelationshipPath(Resource, Id, Relationship);
            _output.WriteLine(result);
            Assert.Equal("/people/123/relationships/job/", result);

            result = target.BuildRelationshipPath(Resource, Id, Relationship, Id);
            _output.WriteLine(result);
            Assert.Equal("/people/123/job/", result);
        }
    }
}
