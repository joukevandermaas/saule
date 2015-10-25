using System.Linq;
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

            var test = q.ApplyQuery(QueryMethod.Take, 50) as IQueryable;
        }

    }

}