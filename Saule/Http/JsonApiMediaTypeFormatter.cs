using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saule.Serialization;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Saule.Http
{
    /// <summary>
    /// Custom media type formatter for Json Api (1.0) responses and requests.
    /// </summary>
    public class JsonApiMediaTypeFormatter : MediaTypeFormatter
    {
        private readonly ApiResource _resource;
        private readonly string _baseUrl;

        /// <summary>
        /// Creates a new instance of the JsonApiMediaTypeFormatter class.
        /// </summary>
        public JsonApiMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.api+json"));
        }

        /// <summary>
        /// Creates a new instance of the JsonApiMediaTypeFormatter class for a particular request.
        /// </summary>
        /// <param name="request"></param>
        public JsonApiMediaTypeFormatter(HttpRequestMessage request) : this()
        {
            _baseUrl = request.RequestUri.ToString();
            if (request.Properties.ContainsKey(Constants.RequestPropertyName))
            {
                _resource = (ApiResource) request.Properties[Constants.RequestPropertyName];
            }
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
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        public override async Task WriteToStreamAsync(
            Type type,
            object value,
            Stream writeStream,
            HttpContent content,
            TransportContext transportContext)
        {
            var json = IsException(type)
                ? SerializeError(value)
                : SerializeOther(value);

            await WriteJsonToStream(json, writeStream);
        }

        private static bool IsException(Type type)
        {
            return type == typeof(HttpError) || type.IsSubclassOf(typeof(Exception));
        }

        private JToken SerializeOther(object value)
        {
            return new ResourceSerializer(
                value,
                _resource,
                _baseUrl)

                .Serialize();
        }

        private static JToken SerializeError(object value)
        {
            var httpError = value as HttpError;
            var serializer = new ErrorSerializer();
            return httpError != null
                ? serializer.Serialize(new ApiError(httpError))
                : serializer.Serialize(new ApiError(value as Exception));
        }

        private static async Task WriteJsonToStream(JToken json, Stream stream)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 2048, true))
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
                return new ResourceDeserializer(json, type).Deserialize();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override MediaTypeFormatter GetPerRequestFormatterInstance(
            Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            return new JsonApiMediaTypeFormatter(request);
        }
    }
}