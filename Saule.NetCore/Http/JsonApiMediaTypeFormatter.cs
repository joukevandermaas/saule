using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Saule.Serialization;

namespace Saule.Http
{
    /// <summary>
    /// Custom media type formatter for Json Api (1.0) responses and requests.
    /// </summary>
    public class JsonApiMediaTypeFormatter : MediaTypeFormatter
    {
        private readonly JsonApiConfiguration _config = new JsonApiConfiguration();
        private HttpRequestMessage _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiMediaTypeFormatter"/> class.
        /// </summary>
        [Obsolete("Please use the extension method 'ConfigureJsonApi' on HttpConfiguration instead.")]
        public JsonApiMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(Constants.MediaType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiMediaTypeFormatter"/> class.
        /// </summary>
        /// <param name="urlBuilder">Determines how to generate urls for links.</param>
        [Obsolete("Please use the extension method 'ConfigureJsonApi' on HttpConfiguration instead.")]
        public JsonApiMediaTypeFormatter(IUrlPathBuilder urlBuilder)
            : this()
        {
            _config.UrlPathBuilder = urlBuilder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiMediaTypeFormatter"/> class.
        /// </summary>
        /// <param name="converters">Json converters to manipulate the serialization process.</param>
        [Obsolete("Please use the extension method 'ConfigureJsonApi' on HttpConfiguration instead.")]
        public JsonApiMediaTypeFormatter(params JsonConverter[] converters)
            : this()
        {
            _config.JsonConverters.AddRange(converters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiMediaTypeFormatter"/> class.
        /// </summary>
        /// <param name="urlBuilder">Determines how to generate urls for links.</param>
        /// <param name="converters">Json converters to manipulate the serialization process.</param>
        [Obsolete("Please use the extension method 'ConfigureJsonApi' on HttpConfiguration instead.")]
        public JsonApiMediaTypeFormatter(IUrlPathBuilder urlBuilder, params JsonConverter[] converters)
            : this()
        {
            _config.UrlPathBuilder = urlBuilder;
            _config.JsonConverters.AddRange(converters);
        }

        internal JsonApiMediaTypeFormatter(JsonApiConfiguration config)
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(Constants.MediaType));
            _config = config;
        }

        internal JsonApiMediaTypeFormatter(
            HttpRequestMessage request,
            JsonApiConfiguration config)
            : this(config)
        {
            _request = request;
        }

        /// <inheritdoc/>
        public override bool CanReadType(Type type)
        {
            return true;
        }

        /// <inheritdoc/>
        public override bool CanWriteType(Type type)
        {
            return true;
        }

        /// <inheritdoc/>
        public override async Task WriteToStreamAsync(
            Type type,
            object value,
            Stream writeStream,
            HttpContent content,
            TransportContext transportContext)
        {
            PreprocessResult preprocessed;
            if (_request.Properties.ContainsKey(Constants.PropertyNames.PreprocessResult))
            {
                preprocessed = _request.Properties[Constants.PropertyNames.PreprocessResult]
                    as PreprocessResult;
            }
            else
            {
                // backwards compatibility with old way to do Saule setup
                preprocessed = PreprocessingDelegatingHandler.PreprocessRequest(value, _request, _config);
            }

            var json = JsonApiSerializer.Serialize(preprocessed);
            await WriteJsonToStream(json, writeStream);
        }

        /// <inheritdoc/>
        public override async Task<object> ReadFromStreamAsync(
            Type type,
            Stream readStream,
            HttpContent content,
            IFormatterLogger formatterLogger)
        {
            using (var reader = new StreamReader(readStream))
            {
                try
                {
                    var json = JToken.Parse(await reader.ReadToEndAsync());
                    return new ResourceDeserializer(json, type).Deserialize();
                }
                catch (JsonApiException ex)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        ReasonPhrase = ex.Message
                    });
                }
                catch (JsonReaderException)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        ReasonPhrase = "Request content is not valid JSON."
                    });
                }
            }
        }

        /// <inheritdoc/>
        public override MediaTypeFormatter GetPerRequestFormatterInstance(
            Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            return new JsonApiMediaTypeFormatter(request, _config);
        }

        private async Task WriteJsonToStream(JToken json, Stream stream)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 2048, true))
            {
                await writer.WriteAsync(json.ToString(Formatting.None, _config.JsonConverters.ToArray()));
            }
        }
    }
}