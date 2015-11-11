using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saule.Queries;
using Saule.Serialization;

namespace Saule.Http
{
    /// <summary>
    /// Custom media type formatter for Json Api (1.0) responses and requests.
    /// </summary>
    public class JsonApiMediaTypeFormatter : MediaTypeFormatter
    {
        // NOTE: the comments on `overrride` public methods below are copied from the MSDN documentation at
        // https://msdn.microsoft.com/en-us/library/system.net.http.formatting.mediatypeformatter(v=vs.118).aspx
        private readonly JsonConverter[] _converters;
        private readonly IUrlPathBuilder _urlBuilder;
        private readonly ApiResource _resource;
        private readonly Uri _baseUrl;
        private readonly JsonApiSerializer _jsonApiSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiMediaTypeFormatter"/> class.
        /// </summary>
        public JsonApiMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(Constants.MediaType));
            _converters = new JsonConverter[0];
            _urlBuilder = new DefaultUrlPathBuilder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiMediaTypeFormatter"/> class.
        /// </summary>
        /// <param name="urlBuilder">Determines how to generate urls for links.</param>
        public JsonApiMediaTypeFormatter(IUrlPathBuilder urlBuilder)
            : this()
        {
            _urlBuilder = urlBuilder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiMediaTypeFormatter"/> class.
        /// </summary>
        /// <param name="converters">Json converters to manipulate the serialization process.</param>
        public JsonApiMediaTypeFormatter(params JsonConverter[] converters)
            : this(new DefaultUrlPathBuilder(), converters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiMediaTypeFormatter"/> class.
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
                var paginationContext = (PaginationContext)request.Properties[Constants.PaginationContextPropertyName];
                jsonApi.PaginationContext = paginationContext;
            }

            if (request.Properties.ContainsKey(Constants.RequestPropertyName))
            {
                _resource = (ApiResource)request.Properties[Constants.RequestPropertyName];
            }

            _jsonApiSerializer = jsonApi;
        }

        /// <summary>
        /// See base class documentation.
        /// Queries whether this <see cref="MediaTypeFormatter"/> can serialize an
        /// object of the specified type.
        /// </summary>
        /// <param name="type">The type to serialize.</param>
        /// <returns>
        /// true if the <see cref="MediaTypeFormatter"/> can serialize the type; otherwise, false.
        /// </returns>
        public override bool CanReadType(Type type)
        {
            return true;
        }

        /// <summary>
        /// Queries whether this <see cref="MediaTypeFormatter"/> can deserialize an
        /// object of the specified type.
        /// </summary>
        /// <param name="type">The type to deserialize.</param>
        /// <returns>
        /// true if the <see cref="MediaTypeFormatter"/> can deserialize the type; otherwise, false.
        /// </returns>
        public override bool CanWriteType(Type type)
        {
            return true;
        }

        /// <summary>
        /// Asynchronously writes an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the object to write.</param>
        /// <param name="value">The object value to write. It may be null.</param>
        /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
        /// <param name="content">The <see cref="HttpContent"/> if available. It may be null.</param>
        /// <param name="transportContext">The <see cref="TransportContext"/> if available. It may be null.</param>
        /// <returns>A <see cref="Task"/> that will perform the write.</returns>
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

        /// <summary>
        /// Asynchronously deserializes an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="readStream">The <see cref="Stream"/> to read.</param>
        /// <param name="content">The <see cref="HttpContent"/>, if available. It may be null.</param>
        /// <param name="formatterLogger">The <see cref="IFormatterLogger"/> to log events to.</param>
        /// <returns>A <see cref="Task"/> whose result will be an object of the given type.</returns>
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
        /// Returns a specialized instance of the <see cref="MediaTypeFormatter"/> that can
        /// format a response for the given parameters.
        /// </summary>
        /// <param name="type">The type to format.</param>
        /// <param name="request">The request.</param>
        /// <param name="mediaType">The media type.</param>
        /// <returns>Returns <see cref="JsonApiMediaTypeFormatter"/>.</returns>
        public override MediaTypeFormatter GetPerRequestFormatterInstance(
            Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            return new JsonApiMediaTypeFormatter(request, _urlBuilder);
        }

        private async Task WriteJsonToStream(JToken json, Stream stream)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 2048, true))
            {
                await writer.WriteAsync(json.ToString(Formatting.None, _converters));
            }
        }
    }
}