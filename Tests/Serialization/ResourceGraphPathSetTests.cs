using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Saule;
using Saule.Serialization;
using System.Linq;
using Tests.Helpers;
using Tests.Models;
using Xunit;
using Xunit.Abstractions;
using Saule.Queries.Including;
using Saule.Queries.Pagination;

namespace Tests.Serialization
{
    public class ResourceGraphPathSetTests
    {
        private readonly ITestOutputHelper _output;

        public ResourceGraphPathSetTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Path set constructor ignores duplicates")]
        public void PathSetIgnoresDuplicates()
        {
            var pathSet = new ResourceGraphPathSet(new string[] { "1", "2", "2", "3" });

            Assert.Equal(3, pathSet.Paths.Count);
        }

        [Fact(DisplayName = "Path set equality works")]
        public void PathSetEqualityWorks()
        {
            var pathSet1 = new ResourceGraphPathSet(new string[] { "1", "2", "3",});
            var pathSet2 = new ResourceGraphPathSet(new string[] { "2", "3", "1", });

            Assert.True(pathSet1.Equals(pathSet2));
            Assert.True(pathSet1 == pathSet2);
        }

        [Fact(DisplayName = "Path set match appropriately")]
        public void PathSetAllNoneMatches()
        {
            var pathSetAll = new ResourceGraphPathSet.All();
            var pathSetNone = new ResourceGraphPathSet.None();
            var pathSet = new ResourceGraphPathSet(new string[]
            {
                "one",
                "one.two",
                "one.two.three",
                "two",
                "two.three",
                "three"
            });

            Assert.True(pathSetAll.MatchesProperty("random"));
            Assert.False(pathSetNone.MatchesProperty("random"));
            Assert.True(pathSet.MatchesProperty("one"));
            Assert.True(pathSet.MatchesProperty("three"));
        }

        [Fact(DisplayName = "Path set children matches correct properties")]
        public void PathSetChildrenMatchesCorrectProperties()
        {
            var pathSet = new ResourceGraphPathSet(new string[]
            {
                "one",
                "one.two",
                "one.two.three",
                "two",
                "two.three",
                "three"
            });

            var oneChildren = pathSet.PathSetForChildProperty("one");

            Assert.Equal(2, oneChildren.Paths.Count);
        }
    }
}