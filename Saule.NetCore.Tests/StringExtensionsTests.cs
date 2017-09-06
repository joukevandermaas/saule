using Saule;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public class StringExtensionsTests
    {
        private static readonly string[] Camel = { "", "someString", "some", "someVeryLongString" };
        private static readonly string[] Pascal = { "", "SomeString", "Some", "SomeVeryLongString" };
        private static readonly string[] Dashed = { "", "some-string", "some", "some-very-long-string" };
        private static readonly string[] Spaced = { "", "some string", "some", "Some very Long string" };
        private static readonly string[] Traps = { "----", "   some---String    ", "_- some ", "_some_very-longString" };

        [Fact(DisplayName = "Camel case works")]
        public void TestCamel()
        {
            TestCasing(Camel, StringExtensions.ToCamelCase);
        }

        [Fact(DisplayName = "Pascal case works")]
        public void TestPascal()
        {
            TestCasing(Pascal, StringExtensions.ToPascalCase);
        }

        [Fact(DisplayName = "Dashed works")]
        public void TestDashed()
        {
            TestCasing(Dashed, StringExtensions.ToDashed);
        }

        private static void TestCasing(IReadOnlyList<string> array, Func<string, string> function)
        {
            for (var i = 0; i < array.Count; i++)
            {
                Assert.Equal(array[i], function(Camel[i]));
                Assert.Equal(array[i], function(Pascal[i]));
                Assert.Equal(array[i], function(Dashed[i]));
                Assert.Equal(array[i], function(Spaced[i]));
                Assert.Equal(array[i], function(Traps[i]));
            }
        }
    }
}