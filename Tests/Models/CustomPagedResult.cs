using Saule.Queries.Pagination;

namespace Tests.Models
{
    public class CustomPagedResult<T>: PagedResult<T>
    {
        public int IgnoredValue { get; set; }
    }
}
