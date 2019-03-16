using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Saule.Http
{
    /// <summary>
    /// Custom route attribute to support negotiating the same route depending on content type to support the bulk extension.
    /// </summary>
    public class BulkExtRouteAttributeAttribute : RouteFactoryAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkExtRouteAttributeAttribute"/> class.
        /// </summary>
        /// <param name="template">Route name</param>
        /// <param name="multiple">Sets whether this route is standard JSON API or bulk extension enabled</param>
        public BulkExtRouteAttributeAttribute(string template, bool multiple)
            : base(template)
        {
            Multiple = multiple;
        }

        /// <summary>
        /// Gets overriden constraints handling to select route based on the attribute.
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether this route is standard JSON API or bulk extension enabled
        /// </summary>
        public bool Multiple { get; private set; }
    }
}
