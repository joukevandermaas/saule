using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saule.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Saule.Http
{
    /// <summary>
    /// Custom media type formatter for Json Api (1.0) responses and requests.
    /// </summary>
    public class JsonApiMediaTypeFormatter : MediaTypeFormatter
    {
        private Type _resourceType;
        private string _baseUrl;
        private JsonApiSerializer _serializer;
        private JsonApiDeserializer _deserializer;

        private bool CanWrite { get; set; } = true;

        /// <summary>
        ///
        /// </summary>
        public JsonApiMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.api+json"));
            _serializer = new JsonApiSerializer();
            _deserializer = new JsonApiDeserializer();
        }

        private JsonApiMediaTypeFormatter(Type resourceType, string baseUrl) : this()
        {
            _resourceType = resourceType;
            _baseUrl = baseUrl;
        }

        /// <summary>
        ///
        /// </summary>
        public override bool CanReadType(Type type)
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        public override bool CanWriteType(Type type)
        {
            return CanWrite || type == typeof(HttpError);
        }

        /// <summary>
        ///
        /// </summary>
        public async override Task WriteToStreamAsync(
            Type type,
            object value,
            Stream writeStream,
            HttpContent content,
            TransportContext transportContext)
        {
            var json = _serializer.Serialize(
                new ApiResponse(value, _resourceType.CreateInstance<ApiResource>()),
                _baseUrl);
            using (var writer = new StreamWriter(writeStream))
            {
                await writer.WriteAsync(json.ToString(Formatting.None));
            }
        }

        /// <summary>
        ///
        /// </summary>
        public async override Task<object> ReadFromStreamAsync(
            Type type,
            Stream readStream,
            HttpContent content,
            IFormatterLogger formatterLogger)
        {
            using (var reader = new StreamReader(readStream))
            {
                var json = JToken.Parse(await reader.ReadToEndAsync());
                return _deserializer.Deserialize(json, type);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            var actionDescriptor = (ReflectedHttpActionDescriptor)request.Properties["MS_HttpActionDescriptor"];
            var attribute =
                actionDescriptor.GetCustomAttributes<ApiResourceAttribute>().SingleOrDefault()
                ?? actionDescriptor.ControllerDescriptor.GetCustomAttributes<ApiResourceAttribute>().SingleOrDefault();

            return attribute == null
                ? new JsonApiMediaTypeFormatter { CanWrite = false }
                : new JsonApiMediaTypeFormatter(attribute.ResourceType, request.RequestUri.ToString());
        }
    }
}