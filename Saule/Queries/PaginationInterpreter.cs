using System;
using System.Diagnostics;
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
            var filtered = queryable.ApplyQuery(QueryMethods.Skip, Context.Page * Context.PerPage) as IQueryable;

            filtered = filtered.ApplyQuery(QueryMethods.Take, Context.PerPage) as IQueryable;

            return filtered;
        }

    }
}
