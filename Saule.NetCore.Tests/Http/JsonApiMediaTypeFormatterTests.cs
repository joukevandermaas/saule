using System.Linq;
using Saule;
using Saule.Http;
using Saule.Http.Formatters;
using Xunit;

namespace Tests.Http
{
    public class JsonApiMediaTypeFormatterTests
    {
        [Fact(DisplayName = "Input formatter must support Json Api media type")]
        public void TestMethod1()
        {
            var formatter = new JsonApiInputFormatter(new JsonApiConfiguration());
            Assert.Equal(1, formatter.SupportedMediaTypes.Count);
            Assert.Equal(Constants.MediaType, formatter.SupportedMediaTypes.First());
        }

        [Fact(DisplayName = "Output formatter must support Json Api media type")]
        public void TestMethod2()
        {
            var formatter = new JsonApiOutputFormatter(new JsonApiConfiguration());
            Assert.Equal(1, formatter.SupportedMediaTypes.Count);
            Assert.Equal(Constants.MediaType, formatter.SupportedMediaTypes.First());
        }
    }
}