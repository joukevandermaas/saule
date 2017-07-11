using System.Configuration;

namespace Saule.Http
{
    /// <summary>
    /// Pagination configuration.  Property values can be set programmatically or be read from web.config app settings.
    /// </summary>
    public class PaginationConfig
    {
        /// <summary>
        /// Web.Config appSettings key for the default page size.
        /// </summary>
        public const string PageSizeAppSettingKey = "JsonApi.PageSizeDefault";

        /// <summary>
        /// Web.Config appSettings key for the default limit for page[size] query parameter values.
        /// </summary>
        /// <remarks>
        /// Omitting this value from configuration will result in the page[size] query parameter being ignored.
        /// </remarks>
        public const string PageSizeLimitKey = "JsonApi.PageSizeLimit";

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationConfig"/> class.
        /// </summary>
        public PaginationConfig()
        {
            // TODO: for compatibility with legacy code only.  Recommend doing away with this in favor of app level configuration
            DefaultPageSize = 10;
        }

        /// <summary>
        /// Gets or sets default page size to use for query actions decorated
        /// with a [Paginated] attribute that does not specify a PerPage value.
        /// </summary>
        public int DefaultPageSize { get; set; }

        /// <summary>
        /// Gets or sets the default value to use for the maximum allowable page
        /// size to accept from the client with a [Paginated] attribute that does
        /// not specify a MaxClientPageSize value.
        /// </summary>
        public int? DefaultPageSizeLimit { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not page[size] query string parameters will be honored.
        /// </summary>
        public bool AllowQueryPageSize
        {
            get { return DefaultPageSizeLimit.HasValue; }
        }

        /// <summary>
        /// Create pagination configuration from web.config app settings.
        /// </summary>
        /// <returns>PaginationConfig instance with values from web.config.</returns>
        public static PaginationConfig FromWebConfig()
        {
            // defaulting to 10 for compatibility with existing Saule conventions
            var paginationConfig = new PaginationConfig();

            int configDefaultPageSize;
            if (int.TryParse(ConfigurationManager.AppSettings[PageSizeAppSettingKey], out configDefaultPageSize))
            {
                paginationConfig.DefaultPageSize = configDefaultPageSize;
            }

            int configPageSizeLimit;
            if (int.TryParse(ConfigurationManager.AppSettings[PageSizeLimitKey], out configPageSizeLimit))
            {
                paginationConfig.DefaultPageSizeLimit = configPageSizeLimit;
            }

            return paginationConfig;
        }
    }
}
