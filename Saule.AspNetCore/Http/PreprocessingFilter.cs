using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Saule.Queries;
using Saule.Serialization;

namespace Saule.Http
{
    /// <summary>
    /// Processes JSON API responses to enable filtering, pagination and sorting.
    /// </summary>
    public class PreprocessingFilter : IActionFilter
    {
        private readonly FrameworkIndependentPreprocesser _processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreprocessingFilter"/> class.
        /// </summary>
        /// <param name="config">The configuration parameters for JSON API serialization.</param>
        public PreprocessingFilter(JsonApiConfiguration config)
        {
            _processor = new FrameworkIndependentPreprocesser(config);
        }

        /// <inheritdoc />
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        /// <inheritdoc />
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var result = _processor.ProcessAfterActionMethod(new CorePreprocessInput(context));

            if (result.ResourceSerializer == null && result.ErrorContent == null)
            {
                return;
            }

            if (result.ErrorContent != null)
            {
                context.Result = new ObjectResult(result.ErrorContent);
                context.ExceptionHandled = true;
            }

            context.HttpContext.Response.StatusCode = result.StatusCode;
            context.HttpContext.Items.Add(Constants.PropertyNames.PreprocessResult, result);
        }
    }
}
