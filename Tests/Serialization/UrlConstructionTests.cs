using System;
using Saule.Serialization;
using Tests.Helpers;
using Xunit;

namespace Tests.Serialization
{
    public class UrlConstructionTests
    {
        [Fact(DisplayName = "Handles query parameters correctly")]
        public void HandlesQueryParams()
        {
            var url = new Uri("http://example.com/api/people/123?a=b&c=d");
            var target = new ResourceSerializer(new Person(prefill: true), new PersonResource(), url);
            var result = target.Serialize();

            var jobLinks = result["data"]?["relationships"]?["job"]?["links"];

            var selfLink = result["links"].Value<Uri>("self")?.PathAndQuery;
            var jobSelfLink = jobLinks?.Value<Uri>("self")?.PathAndQuery;
            var jobRelationLink = jobLinks?.Value<Uri>("related")?.PathAndQuery;

            Assert.Equal("/api/people/123?a=b&c=d", selfLink);
            Assert.Equal("/api/people/123/relationships/employer/", jobSelfLink);
            Assert.Equal("/api/people/123/employer/", jobRelationLink);
        }

        [Fact(DisplayName = "Items have self links in a collection")]
        public void SelfLinksInCollection()
        {
            var people = new[]
            {
                new Person(prefill: true, id: "1"),
                new Person(prefill: true, id: "2"),
                new Person(prefill: true, id: "3"),
                new Person(prefill: true, id: "4")
            };
            var target = new ResourceSerializer(
                people, new PersonResource(), new Uri("http://example.com/people/"));
            var result = target.Serialize();

            foreach (var elem in result["data"])
            {
                var links = elem["links"];
                Assert.NotNull(links);
                Assert.Equal("/people/" + elem.Value<string>("id") + "/", links.Value<Uri>("self").AbsolutePath);
            }
        }

        [Fact(DisplayName = "Item does not have self link in single element")]
        public void NoSelfLinksInObject()
        {
            var target = new ResourceSerializer(
                new Person(prefill: true), new PersonResource(), new Uri("http://example.com/people/1"));
            var result = target.Serialize();

            var links = result["data"]?["links"];

            Assert.Null(links);
        }

        [Fact(DisplayName = "Adds top level self link")]
        public void SelfLink()
        {
            var target = new ResourceSerializer(
                new Person(prefill: true), new PersonResource(), new Uri("http://example.com/people/1"));
            var result = target.Serialize();

            var selfLink = result["links"].Value<Uri>("self").AbsolutePath;

            Assert.Equal("/people/1", selfLink);
        }

        [Fact(DisplayName = "Serializes relationships' links")]
        public void SerializesRelationshipLinks()
        {
            var target = new ResourceSerializer(
                new Person(prefill: true), new PersonResource(), new Uri("http://example.com/people/1"));
            var result = target.Serialize();

            var relationships = result["data"]["relationships"];
            var job = relationships["job"];
            var friends = relationships["friends"];

            Assert.Equal("/people/1/employer/", job["links"].Value<Uri>("related").AbsolutePath);
            Assert.Equal("/people/1/relationships/employer/", job["links"].Value<Uri>("self").AbsolutePath);

            Assert.Equal("/people/1/friends/", friends["links"].Value<Uri>("related").AbsolutePath);
            Assert.Equal("/people/1/relationships/friends/", friends["links"].Value<Uri>("self").AbsolutePath);
        }

        [Fact(DisplayName = "Builds absolute links correctly")]
        public void BuildsRightLinks()
        {
            var target = new ResourceSerializer(
                new Person(prefill: true), new PersonResource(), new Uri("http://example.com/api/people/1"));
            var result = target.Serialize();

            var job = result["data"]["relationships"]["job"];

            Assert.Equal("http://example.com/api/people/1/employer/",
                job["links"].Value<Uri>("related").ToString());
            Assert.Equal("http://example.com/api/people/1/relationships/employer/",
                job["links"].Value<Uri>("self").ToString());
        }

    }
}
