using System;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Saule.Http
{
    /// <summary>
    /// An optional attribute that can be used to opt an api into returning a JsonApi response.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class JsonApiAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="context">The action context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            var config = new JsonApiConfiguration();
            JsonApiProcessor.ProcessRequest(context.Request, context.Response, config, requiresMediaType: false);

            if (context.Exception != null)
            {
                return;
            }

            var responseContent = context.Response.Content as ObjectContent;
            if (responseContent == null)
            {
                return;
            }

            var formatter = new JsonApiMediaTypeFormatter(context.Request, config);

            context.Response.Content = new ObjectContent(responseContent.ObjectType, responseContent.Value, formatter);
        }
    }
}
