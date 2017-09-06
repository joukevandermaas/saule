using System;
using System.Collections.Generic;
using Moq;
using Newtonsoft.Json.Linq;
using Saule;
using Saule.Serialization;
using Tests.Helpers;
using Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Serialization
{
    public class MetadataTests
    {
        private readonly ITestOutputHelper _output;

        public MetadataTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Serializes objects correctly")]
        public void SerializesObjectsCorrectly()
        {
            var company = Get.Company();
            var mock = new Mock<ApiResource>();
            var metaObject = Get.Person();

            mock.Setup(r => r.GetMetadata(It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<bool>()))
                .Returns(metaObject);

            var target = new ResourceSerializer(
                company,
                mock.Object,
                new Uri("http://localhost/people/123"),
                new DefaultUrlPathBuilder(),
                null,
                null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var meta = result["meta"];

            Assert.Equal(metaObject.FirstName, meta.Value<string>("first-name"));
        }

        [Fact(DisplayName = "Serializes dictionaries correctly")]
        public void SerializesDictionariesCorrectly()
        {
            var company = Get.Company();
            var mock = new Mock<ApiResource>();
            var metaObject = new Dictionary<string, int>
            {
                ["first"] = 1,
                ["last"] = 12,
                ["sum"] = 42
            };

            mock.Setup(r => r.GetMetadata(It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<bool>()))
                .Returns(metaObject);

            var target = new ResourceSerializer(
                company,
                mock.Object,
                new Uri("http://localhost/people/123"),
                new DefaultUrlPathBuilder(),
                null,
                null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var meta = result["meta"];

            Assert.Equal(metaObject["first"], meta.Value<int>("first"));
            Assert.Equal(metaObject["last"], meta.Value<int>("last"));
            Assert.Equal(metaObject["sum"], meta.Value<int>("sum"));
        }

        [Fact(DisplayName = "Passes through JTokens without modifying them")]
        public void PassesThroughJTokens()
        {
            var company = Get.Company();
            var mock = new Mock<ApiResource>();
            var metaObject = JObject.FromObject(Get.Person());

            mock.Setup(r => r.GetMetadata(It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<bool>()))
                .Returns(metaObject);

            var target = new ResourceSerializer(
                company,
                mock.Object,
                new Uri("http://localhost/people/123"),
                new DefaultUrlPathBuilder(),
                null,
                null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var meta = result["meta"];

            Assert.Equal(metaObject, meta);
        }
        
        [Fact(DisplayName = "Serializes collections correctly")]
        public void SerializesCollectionsCorrectly()
        {
            var company = Get.Company();
            var mock = new Mock<ApiResource>();
            var metaObject = new List<string> { "first", "last", "sum" };

            mock.Setup(r => r.GetMetadata(It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<bool>()))
                .Returns(metaObject);

            var target = new ResourceSerializer(
                company,
                mock.Object,
                new Uri("http://localhost/people/123"),
                new DefaultUrlPathBuilder(),
                null,
                null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            var meta = result["meta"] as JArray;

            Assert.Equal(metaObject[0], meta?[0].Value<string>());
            Assert.Equal(metaObject[1], meta?[1].Value<string>());
            Assert.Equal(metaObject[2], meta?[2].Value<string>());
        }

        [Fact(DisplayName = "Calls 'GetMetaDataFor' with correct arguments")]
        public void CallsGetMetadataForCorrectly()
        {
            var company = Get.Company();
            var mock = new Mock<ApiResource>();
            mock.Setup(r => r.GetMetadata(It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<bool>())).Returns(null);

            var target = new ResourceSerializer(
                company,
                mock.Object,
                new Uri("http://localhost/people/123"),
                new DefaultUrlPathBuilder(),
                null,
                null);

            target.Serialize();

            mock.Verify(r => r.GetMetadata(company, company.GetType(), false));

            var companies = new List<Company> {company};

            target = new ResourceSerializer(
                companies,
                mock.Object,
                new Uri("http://localhost/people/123"),
                new DefaultUrlPathBuilder(),
                null,
                null);

            target.Serialize();

            mock.Verify(r => r.GetMetadata(companies, company.GetType(), true));
        }

        [Fact(DisplayName = "Serializes empty meta hash for null metadata")]
        public void EmptyForNull()
        {
            var company = Get.Company();

            var target = new ResourceSerializer(
                company,
                new CompanyResource(),
                new Uri("http://localhost/people/123"),
                new DefaultUrlPathBuilder(),
                null,
                null);

            var result = target.Serialize();
            _output.WriteLine(result.ToString());

            JToken meta;
            Assert.False(result.TryGetValue("meta", out meta), "Meta hash is null");
        }
    }
}
