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
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Saule.Queries;

namespace Saule.Http
{
    /// <summary>
    /// Custom media type formatter for Json Api (1.0) responses and requests.
    /// </summary>
    public class JsonApiMediaTypeFormatter : MediaTypeFormatter
    {
        private readonly ApiResource _resource;
        private readonly PaginationContext _paginationContext;
        private readonly Uri _baseUrl;

        internal JsonSerializer JsonSerializer { get; }
        internal IUrlPathBuilder UrlPathBuilder { get; }

        /// <summary>
        /// Creates a new instance of the JsonApiMediaTypeFormatter class.
        /// </summary>
        public JsonApiMediaTypeFormatter()
        {
            UrlPathBuilder = new DefaultUrlPathBuilder();
            JsonSerializer = new JsonSerializer();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(Constants.MediaType));
        }

        /// <summary>
        /// Creates a new instance of the JstonApiMediaTypeFormatter class.
        /// </summary>
        /// <param name="urlBuilder">Determines how to generate urls for links.</param>
        public JsonApiMediaTypeFormatter(IUrlPathBuilder urlBuilder)
            : this()
        {
            UrlPathBuilder = urlBuilder;
        }

        /// <summary>
        /// Creates a new instance of the JsonApiMediaTypeFormatter class.
        /// </summary>
        /// <param name="converters">Json converters to manipulate the serialization process.</param>
        public JsonApiMediaTypeFormatter(params JsonConverter[] converters)
            : this(new DefaultUrlPathBuilder(), converters)
        {
        }

        /// <summary>
        /// Creates a new instance of the JstonApiMediaTypeFormatter class.
        /// </summary>
        /// <param name="urlBuilder">Determines how to generate urls for links.</param>
        /// <param name="converters">Json converters to manipulate the serialization process.</param>
        public JsonApiMediaTypeFormatter(IUrlPathBuilder urlBuilder, params JsonConverter[] converters)
            : this(urlBuilder)
        {
            foreach (var converter in converters)
            {
                JsonSerializer.Converters.Add(converter);
            }
        }

        internal JsonApiMediaTypeFormatter(HttpRequestMessage request, JsonSerializer serializer, IUrlPathBuilder urlBuilder)
            : this()
        {
            _baseUrl = request.RequestUri;
            UrlPathBuilder = urlBuilder;
            JsonSerializer = serializer;
            if (request.Properties.ContainsKey(Constants.RequestPropertyName))
            {
                _resource = (ApiResource)request.Properties[Constants.RequestPropertyName];
            }

            if (request.Properties.ContainsKey(Constants.PaginationContextPropertyName))
            {
                _paginationContext = (PaginationContext) request.Properties[Constants.PaginationContextPropertyName];
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
                _baseUrl, 
                UrlPathBuilder,
                _paginationContext)

                .Serialize(JsonSerializer);
        }

        private static JToken SerializeError(object value)
        {
            var httpError = value as HttpError;
            var serializer = new ErrorSerializer();
            return httpError != null
                ? serializer.Serialize(new ApiError(httpError))
                : serializer.Serialize(new ApiError(value as Exception));
        }

        private async Task WriteJsonToStream(JToken json, Stream stream)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 2048, true))
            {
                await writer.WriteAsync(json.ToString(Formatting.None, JsonSerializer.Converters.ToArray()));
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
            return new JsonApiMediaTypeFormatter(request, JsonSerializer, UrlPathBuilder);
        }
    }
}