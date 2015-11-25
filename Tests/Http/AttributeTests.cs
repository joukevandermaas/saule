using System;
using System.Collections.Generic;
using Saule.Http;
using Tests.Models;
using Xunit;

namespace Tests.Http
{
    public class AttributeTests
    {
        [Theory(DisplayName = "PaginatedAttribute does not allow PerPage < 1")]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(-512)]
        public void PerPageLargerThanOne(int count)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new PaginatedAttribute { PerPage = count });
        }

        [Theory(DisplayName = "ReturnsResourceAttribute only allows types that extend ApiResource")]
        [InlineData(typeof(string))]
        [InlineData(typeof(IDictionary<,>))]
        [InlineData(typeof(int))]
        [InlineData(typeof(LocationType))]
        [InlineData(typeof(List<>))]
        public void OnlyAllowApiResource(Type type)
        {
            Assert.Throws<ArgumentException>(() =>
                new ReturnsResourceAttribute(type));
        }
    }
}
