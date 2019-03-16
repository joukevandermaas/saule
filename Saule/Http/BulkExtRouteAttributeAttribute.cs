using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Saule.Http
{
    public class BulkExtRouteAttributeAttribute : RouteFactoryAttribute
    {
        public BulkExtRouteAttributeAttribute(string template, bool multiple)
            : base(template)
        {
            Multiple = multiple;
        }

        public override IDictionary<string, object> Constraints
        {
            get
            {
                var constraints = new HttpRouteValueDictionary();
                if (Multiple)
                {
                    constraints.Add("Content-Type", new ContentTypeConstraint(Constants.MediaTypeBulkExtension));
                }
                else
                {
                    constraints.Add("Content-Type", new ContentTypeConstraint(Constants.MediaType));
                }

                return constraints;
            }
        }

        public bool Multiple { get; private set; }
    }

    internal class ContentTypeConstraint : IHttpRouteConstraint
    {
        public ContentTypeConstraint(string allowedMediaType)
        {
            AllowedMediaType = allowedMediaType;
        }

        public string AllowedMediaType { get; private set; }

        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (routeDirection == HttpRouteDirection.UriResolution)
            {
                return GetMediaHeader(request) == AllowedMediaType;
            }
            else
            {
                return true;
            }
        }

        private string GetMediaHeader(HttpRequestMessage request)
        {
            IEnumerable<string> headerValues;
            if (request.Content.Headers.TryGetValues("Content-Type", out headerValues) && headerValues.Count() == 1)
            {
                return headerValues.First();
            }
            else
            {
                return "application/vnd.api+json";
            }
        }
    }
}
