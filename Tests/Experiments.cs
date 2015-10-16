using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Win32.SafeHandles;
using Saule.Queries;
using Xunit;

namespace Tests
{
    public class Experiments
    {
        [Fact]
        public void Run()
        {
            var q = Enumerable.Repeat("hello", 100).AsQueryable();

            var test = q.ApplyQuery(QueryMethods.Take, 50) as IQueryable;
        }

    }

}