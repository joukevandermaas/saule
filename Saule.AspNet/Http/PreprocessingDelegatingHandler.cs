namespace Saule.Http
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Saule.Serialization;

    /// <summary>
    /// Processes JSON API responses to enable filtering, pagination and sorting.
    /// </summary>
    public class PreprocessingDelegatingHandler : DelegatingHandler
    {
        private readonly FrameworkIndependentPreprocesser _processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreprocessingDelegatingHandler"/> class.
        /// </summary>
        /// <param name="config">The configuration parameters for JSON API serialization.</param>
        public PreprocessingDelegatingHandler(JsonApiConfiguration config)
        {
            _processor = new FrameworkIndependentPreprocesser(config);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var actionMethodResult = await base.SendAsync(request, cancellationToken);
            var input = new AspNetPreprocessInput(request, actionMethodResult);
            try
            {
                var result = _processor.ProcessAfterActionMethod(input);

                if (result.ResourceSerializer == null && result.ErrorContent == null)
                {
                    return actionMethodResult;
                }

                request.Properties.Add(Constants.PropertyNames.PreprocessResult, result);

                actionMethodResult.StatusCode = (HttpStatusCode)result.StatusCode;

                return actionMethodResult;
            }
            catch (Exception ex)
            {
                input.AddError(ex);

                var result = _processor.ProcessAfterActionMethod(input);

                request.Properties.Add(Constants.PropertyNames.PreprocessResult, result);
                actionMethodResult.StatusCode = (HttpStatusCode)result.StatusCode;

                return actionMethodResult;
            }
        }
    }
}
