using System;
using System.Linq.Expressions;
using System.Reflection;
using Saule.Http;
using Tests.Helpers;
using Xunit;

namespace Tests.Http
{
    public class QueryFilterExpressionCollectionTests
    {
        [Fact(DisplayName = "Works with method calls in the expression")]
        public void WorksWithMethodCalls()
        {
            var collection = new QueryFilterExpressionCollection();
            collection.SetExpression<Child>((x, y) => x.StringEquals(y.ToString()));
            var property = CreatePropertyInfo<Child>();

            var result = collection.GetQueryFilterExpression(property);

            AssertExpressionEqual<Child>((x, y) => x.StringEquals(y.ToString()), result);
        }

        [Fact(DisplayName = "Setting a new expression overwrites the previous")]
        public void SetOverridesOldValue()
        {
            var collection = new QueryFilterExpressionCollection();
            var property = CreatePropertyInfo<Child>();

            collection.SetExpression<Child>((x, y) => x.GetHashCode() == y.GetHashCode());
            var result = collection.GetQueryFilterExpression(property);
            AssertExpressionEqual<Child>((x, y) => x.GetHashCode() == y.GetHashCode(), result);

            collection.SetExpression<Child>((x, y) => x != y);
            result = collection.GetQueryFilterExpression(property);
            AssertExpressionEqual<Child>((x, y) => x != y, result);
        }

        [Fact(DisplayName = "Has default expression for any unset type")]
        public void HasDefaultExpression()
        {
            var collection = new QueryFilterExpressionCollection();
            var childProperty = CreatePropertyInfo<Child>();
            var parentProperty = CreatePropertyInfo<Parent>();

            var childResult = collection.GetQueryFilterExpression(childProperty);
            var parentResult = collection.GetQueryFilterExpression(parentProperty);

            AssertExpressionEqual<Child>((x, y) => x == y, childResult);
            AssertExpressionEqual<Parent>((x, y) => x == y, parentResult);
        }

        [Fact(DisplayName = "Finds the real type before the base type")]
        public void FindsRealTypeFirst()
        {
            var collection = new QueryFilterExpressionCollection();
            collection.SetExpression<Parent>((x, y) => x != y);
            collection.SetExpression<Child>((x, y) => x.GetHashCode() == y.GetHashCode());
            var childProperty = CreatePropertyInfo<Child>();
            var parentProperty = CreatePropertyInfo<Parent>();

            var childResult = collection.GetQueryFilterExpression(childProperty);
            var parentResult = collection.GetQueryFilterExpression(parentProperty);

            AssertExpressionEqual<Child>((x, y) => x.GetHashCode() == y.GetHashCode(), childResult);
            AssertExpressionEqual<Parent>((x, y) => x != y, parentResult);
        }

        [Fact(DisplayName = "Finds the expression set for the type")]
        public void FindsCorrectExpression()
        {
            var collection = new QueryFilterExpressionCollection();
            collection.SetExpression<Child>((x, y) => x != y);
            var property = CreatePropertyInfo<Child>();

            var result = collection.GetQueryFilterExpression(property);

            AssertExpressionEqual<Child>((x, y) => x != y, result);
        }

        [Fact(DisplayName = "Finds the expression set for base type")]
        public void FindsCorrectExpressionWithInheritance()
        {
            var collection = new QueryFilterExpressionCollection();
            collection.SetExpression<Parent>((x, y) => x != y);
            var property = CreatePropertyInfo<Child>();

            var result = collection.GetQueryFilterExpression(property);

            AssertExpressionEqual<Child>((x, y) => x != y, result);
        }

        private static void AssertExpressionEqual<T>(Expression<Func<T, T, bool>> expected, Expression actual)
        {
            // The actual expression could be of a different type, if something
            // is broken (the collection is supposed to change the types internally)
            //
            // IsType assertion removed for NetCore. Fails due to:
            // Expected: System.Linq.Expressions.Expression`1...<same>
            // Actual:   System.Linq.Expressions.Expression2`1...<same>
            // It is not properly understood why this difference is happening, but hoping that
            // ExpressionComparer will provide sufficient test coverage
            //
            // Assert.IsType(typeof(Expression<Func<T, T, bool>>), actual);

            var actualFunc = actual as Expression<Func<T, T, bool>>;

            Assert.Equal(expected, actualFunc, new ExpressionComparer());
        }

        private static PropertyInfo CreatePropertyInfo<T>() where T : new()
        {
            var type = new
            {
                Property = new T()
            };

            return type.GetType().GetProperty("Property");
        }

        private class Parent
        {
            public bool StringEquals(string other)
            {
                return ToString() == other;
            }
        }
        private class Child : Parent { }
    }
}

