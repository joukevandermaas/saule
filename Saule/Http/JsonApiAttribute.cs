using System;
using System.Web.Http.Filters;

namespace Saule.Http
{
    /// <summary>
    /// An optional attribute that can be used to opt an api into returning a JsonApi response.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class JsonApiAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// See base class documentation.
        /// </summary>
        /// <param name="context">The action context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            JsonApiProcessor.ProcessRequest(context);
        }
    }
}
