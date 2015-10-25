using System.Collections;
using System.Linq;

namespace Saule.Queries
{
    internal class PaginationInterpreter
    {
        public PaginationInterpreter(PaginationContext context)
        {
            Context = context;
        }

        public PaginationContext Context { get; }

        public IQueryable Apply(IQueryable queryable)
        {
            var filtered = queryable.ApplyQuery(QueryMethod.Skip, Context.Page * Context.PerPage) as IQueryable;
            filtered = filtered.ApplyQuery(QueryMethod.Take, Context.PerPage) as IQueryable;

            return filtered;
        }
        public IEnumerable Apply(IEnumerable queryable)
        {
            var filtered = queryable.ApplyQuery(QueryMethod.Skip, Context.Page * Context.PerPage) as IEnumerable;
            filtered = filtered.ApplyQuery(QueryMethod.Take, Context.PerPage) as IEnumerable;

            return filtered;
        }

    }
}
