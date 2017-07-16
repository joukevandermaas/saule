using System.Configuration;

namespace Saule.Http
{
    /// <summary>
    /// Pagination configuration.  Property values can be set programmatically or be read from web.config app settings.
    /// </summary>
    public class PaginationConfig
    {
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
    }
}
