using System;
using System.Configuration;

namespace Saule.Configuration
{
    /// <summary>
    /// Pagination configuration.  Property values can be set programmatically or be read from web.config app settings.
    /// </summary>
    public static class PaginationConfig
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

        static PaginationConfig()
        {
            // the default for the default page size
            int defaultPageSize = 10;
            int.TryParse(ConfigurationManager.AppSettings[PageSizeAppSettingKey], out defaultPageSize);
            DefaultPageSize = defaultPageSize;

            int pageSizeLimit;
            if (int.TryParse(ConfigurationManager.AppSettings[PageSizeLimitKey], out pageSizeLimit))
            {
                DefaultPageSizeLimit = pageSizeLimit;
            }
        }

        /// <summary>
        /// Gets or sets default page size to use for query actions decorated
        /// with a [Paginated] attribute that does not specify a PerPage value.
        /// </summary>
        public static int DefaultPageSize { get; set; }

        /// <summary>
        /// Gets or sets the default value to use for the maximum allowable page
        /// size to accept from the client with a [Paginated] attribute that does
        /// not specify a MaxClientPageSize value.
        /// </summary>
        public static int? DefaultPageSizeLimit { get; set; }
    }
}
