using Saule;
using System;
using Xunit;

namespace Tests
{
    public class StringExtensionsTests
    {
        private static readonly string[] _camel = { "", "someString", "some", "someVeryLongString" };
        private static readonly string[] _pascal = { "", "SomeString", "Some", "SomeVeryLongString" };
        private static readonly string[] _dashed = { "", "some-string", "some", "some-very-long-string" };
        private static readonly string[] _spaced = { "", "some string", "some", "Some very Long string" };
        private static readonly string[] _traps = { "----", "   some---String    ", "_- some ", "_some_very-longString" };

        [Fact(DisplayName = "Camel case works")]
        public void TestCamel()
        {
            TestCasing(_camel, StringExtensions.ToCamelCase);
        }

        [Fact(DisplayName = "Pascal case works")]
        public void TestPascal()
        {
            TestCasing(_pascal, StringExtensions.ToPascalCase);
        }

        [Fact(DisplayName = "Dashed works")]
        public void TestDashed()
        {
            TestCasing(_dashed, StringExtensions.ToDashed);
        }

        private void TestCasing(string[] array, Func<string, string> function)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Assert.Equal(array[i], function(_camel[i]));
                Assert.Equal(array[i], function(_pascal[i]));
                Assert.Equal(array[i], function(_dashed[i]));
                Assert.Equal(array[i], function(_spaced[i]));
                Assert.Equal(array[i], function(_traps[i]));
            }
        }
    }
}