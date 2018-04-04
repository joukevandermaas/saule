using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
        /// <param name="actionExecutedContext">The action context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
            {
                return;
            }

            var content = actionExecutedContext.Response.Content as ObjectContent;
            if (content == null)
            {
                return;
            }

            var formatter = new JsonApiMediaTypeFormatter(new JsonApiConfiguration()).GetPerRequestFormatterInstance(
                typeof(string),
                actionExecutedContext.Request,
                new MediaTypeHeaderValue("application/vnd.api+json"));

            actionExecutedContext.Response.Content = new ObjectContent(content.ObjectType, content.Value, formatter);
        }
    }
}
