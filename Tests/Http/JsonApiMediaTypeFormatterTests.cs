using System;
using System.Globalization;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using Newtonsoft.Json;
using Saule.Http;
using Xunit;


namespace Tests.Http
{
    public class JsonApiMediaTypeFormatterTests
    {
        [Fact(DisplayName = "Default constructor must instantiate JsonSerializer")]
        public void Constructor()
        {
            var target = new JsonApiMediaTypeFormatter();
            Assert.NotNull(target.JsonSerializer);
        }

        //[Fact(DisplayName = "Constructor with JsonSerializer must use that instance")]
        //public void ConstructorWithJsonSerializer()
        //{
        //    var culture = CultureInfo.GetCultureInfo("LT");
        //    var serializer = new JsonSerializer() {Culture = culture } ;
        //    var target = new JsonApiMediaTypeFormatter(serializer);
        //    Assert.NotNull(target.JsonSerializer);
        //    Assert.Equal(target.JsonSerializer.Culture, culture );
        //}

        [Fact(DisplayName = "Constructor with JsonConverterArray must instantiate JsonSerializer")]
        public void ConstructorWithJsonConverters()
        {
            JsonConverter[] jsonConverters = {new DummyJsonConverter() {Name = "Dummy1"}, new DummyJsonConverter() {Name = "Dummy2"}};
            var target = new JsonApiMediaTypeFormatter(jsonConverters);
            Assert.NotNull(target.JsonSerializer);
        }


        private class DummyJsonConverter : JsonConverter
        {
            public string Name { get; set; }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return null;
            }

            public override bool CanConvert(Type objectType)
            {
                return true;
            }
        }

    }
}
