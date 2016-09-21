using System;
using System.Collections;
using System.Linq;
using Saule.Queries.Including;

namespace Saule.Queries.Including
{
    internal class IncludingInterpreter
    {
        private readonly IncludingContext _context;
        private readonly ApiResource _resource;

        public IncludingInterpreter(IncludingContext context, ApiResource resource)
        {
            _context = context;
            _resource = resource;
        }

        internal object Apply(IQueryable queryable)
        {
            throw new NotImplementedException();
        }

        internal object Apply(IEnumerable enumerable)
        {
            throw new NotImplementedException();
        }
    }
}