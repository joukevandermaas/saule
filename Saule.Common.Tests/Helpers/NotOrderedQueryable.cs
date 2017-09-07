using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tests.Helpers
{
    /// <summary>
    /// The queryable returned by Enumerable.AsQueryable() is ordered
    /// by default, so it cannot be used in these tests.
    /// </summary>
    public class NotOrderedQueryable<T> : IQueryable<T>
    {
        private readonly IQueryable<T> _impl;

        public NotOrderedQueryable(IQueryable<T> impl)
        {
            _impl = impl;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return _impl.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression => _impl.Expression;
        public Type ElementType => _impl.ElementType;
        public IQueryProvider Provider => _impl.Provider;
    }
}