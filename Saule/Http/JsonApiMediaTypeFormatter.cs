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
using Saule.Queries;

namespace Saule.Http
{
    /// <summary>
    /// Custom media type formatter for Json Api (1.0) responses and requests.
    /// </summary>
    public class JsonApiMediaTypeFormatter : MediaTypeFormatter
    {
        private readonly JsonConverter[] _converters;
        private readonly IUrlPathBuilder _urlBuilder;
        private readonly ApiResource _resource;
        private readonly Uri _baseUrl;
        private readonly JsonApiSerializer _jsonApiSerializer;

        /// <summary>
        /// Creates a new instance of the JsonApiMediaTypeFormatter class.
        /// </summary>
        public JsonApiMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(Constants.MediaType));
            _converters = new JsonConverter[0];
            _urlBuilder = new DefaultUrlPathBuilder();
        }

        /// <summary>
        /// Creates a new instance of the JstonApiMediaTypeFormatter class.
        /// </summary>
        /// <param name="urlBuilder">Determines how to generate urls for links.</param>
        public JsonApiMediaTypeFormatter(IUrlPathBuilder urlBuilder)
            : this()
        {
            _urlBuilder = urlBuilder;
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
            _converters = converters;
        }

        internal JsonApiMediaTypeFormatter(HttpRequestMessage request, IUrlPathBuilder urlBuilder)
            : this()
        {
            var jsonApi = new JsonApiSerializer { UrlPathBuilder = urlBuilder };
            jsonApi.JsonConverters.AddRange(_converters);

            _baseUrl = request.RequestUri;

            if (request.Properties.ContainsKey(Constants.PaginationContextPropertyName))
            {
                var paginationContext = (PaginationContext) request.Properties[Constants.PaginationContextPropertyName];
                jsonApi.PaginationContext = paginationContext;
            }

            if (request.Properties.ContainsKey(Constants.RequestPropertyName))
            {
                _resource = (ApiResource) request.Properties[Constants.RequestPropertyName];
            }

            _jsonApiSerializer = jsonApi;
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
            var json = _jsonApiSerializer.Serialize(value, _resource, _baseUrl);
            await WriteJsonToStream(json, writeStream);
        }

        private async Task WriteJsonToStream(JToken json, Stream stream)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 2048, true))
            {
                await writer.WriteAsync(json.ToString(Formatting.None, _converters));
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override async Task<object> ReadFromStreamAsync(
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
            return new JsonApiMediaTypeFormatter(request, _urlBuilder);
        }

    }
}