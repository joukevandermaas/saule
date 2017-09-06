using Saule;
using Saule.Serialization;
using Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Serialization
{
    public class CanonicalUrlPathBuilderTests
    {
        private readonly ITestOutputHelper _output;

        private static string Id => "123";
        private static ApiResource Resource => new PersonResource();
        private static ResourceRelationship Relationship => new ResourceRelationship<CompanyResource>(
            "job", "/job", RelationshipKind.BelongsTo, new CompanyResource(), LinkType.All);

        public CanonicalUrlPathBuilderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Collection uses ApiResource.UrlPath")]
        public void UseUrlPath()
        {
            var target = new CanonicalUrlPathBuilder();
            var result = target.BuildCanonicalPath(new PersonResource());
            _output.WriteLine(result);

            Assert.Equal("/people/", result);
        }

        [Fact(DisplayName = "All methods use prefix")]
        public void AddsPrefix()
        {
            var target = new CanonicalUrlPathBuilder("my-prefix");

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
            var target = new CanonicalUrlPathBuilder();

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
            Assert.StartsWith("/corporations/", result);
        }

        [Fact(DisplayName = "Gives correct results")]
        public void GivesCorrectResults()
        {
            var target = new CanonicalUrlPathBuilder();

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
            Assert.Equal("/corporations/123/", result);
        }
    }
}
