using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Saule;
using Saule.Queries;
using Tests.Helpers;
using Xunit;

namespace Tests.Queries
{
    public class PaginationInterpreterTests
    {
        private static PaginationContext DefaultContext => new PaginationContext(GetQueryForPage(0), 10);

        [Fact(DisplayName = "Does not do anything to non-enumerables/queryables")]
        public void IgnoresNonEnumerables()
        {
            var obj = new Person(prefill: true);
            PaginationInterpreter.ApplyPaginationIfApplicable(DefaultContext, obj);
        }

        [Fact(DisplayName = "Orders queryables before pagination")]
        public void OrdersQueryables()
        {
            var obj = new NotOrdered<Person>(GetPeopleDescending(99).Take(100).AsQueryable());

            var result = (PaginationInterpreter.ApplyPaginationIfApplicable(DefaultContext, obj)
                as IEnumerable<Person>)?.ToList();

            Assert.Equal("0", result?.FirstOrDefault()?.Id);
            Assert.Equal("1", result?.Skip(1).FirstOrDefault()?.Id);
        }

        [Fact(DisplayName = "Does not order enumerables before pagination")]
        public void DoesNotOrderEnumerables()
        {
            var obj = GetPeopleDescending(100).Take(100);
            var result = (PaginationInterpreter.ApplyPaginationIfApplicable(DefaultContext, obj)
                as IEnumerable<Person>)?.ToList();

            Assert.Equal("100", result?.FirstOrDefault()?.Id);
            Assert.Equal("99", result?.Skip(1).FirstOrDefault()?.Id);
        }

        [Fact(DisplayName = "Takes the correct elements based on the parameters")]
        public void TakesCorrectElements()
        {
            var obj = GetPeople().Take(100);
            var context = new PaginationContext(GetQueryForPage(1), 10);
            var result = (PaginationInterpreter.ApplyPaginationIfApplicable(context, obj)
                as IEnumerable<Person>)?.ToList();

            Assert.Equal(10, result?.Count);
            Assert.Equal("10", result?.FirstOrDefault()?.Id);
        }

        [Fact(DisplayName = "Works on an empty enumerable")]
        public void WorksOnEmptyEnumerable()
        {
            var obj = Enumerable.Empty<Person>();
            var result = PaginationInterpreter.ApplyPaginationIfApplicable(DefaultContext, obj)
                as IEnumerable<Person>;

            Assert.Equal(false, result?.Any());
        }

        [Fact(DisplayName = "Does not order already ordered queryable")]
        public void RespectsOrderedQueryable()
        {
            var obj = new NotOrdered<Person>(GetPeople().Take(100).AsQueryable())
                .OrderByDescending(p => p.Id);
            var result = (PaginationInterpreter.ApplyPaginationIfApplicable(DefaultContext, obj)
                as IEnumerable<Person>)?.ToList();

            Assert.Equal("99", result?.FirstOrDefault()?.Id);
            Assert.Equal("98", result?.Skip(1).FirstOrDefault()?.Id);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetQueryForPage(int number)
        {
            yield return new KeyValuePair<string, string>(Constants.PageNumberQueryName, number.ToString());
        }

        private static IEnumerable<Person> GetPeople()
        {
            var i = 0;
            while (true)
            {
                yield return new Person(prefill: true, id: i++.ToString());
            }
        }

        private static IEnumerable<Person> GetPeopleDescending(int start)
        {
            var i = start;
            while (true)
            {
                yield return new Person(prefill: true, id: i--.ToString());
            }
        }

        /// <summary>
        /// The queryable returned by Enumerable.AsQueryable() is ordered
        /// by default, so it cannot be used in these tests.
        /// </summary>
        private class NotOrdered<T> : IQueryable<T>
        {
            private readonly IQueryable<T> _impl;

            public NotOrdered(IQueryable<T> impl)
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
}
