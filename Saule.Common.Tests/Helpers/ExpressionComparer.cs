using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tests.Helpers
{
    public class ExpressionComparer : IEqualityComparer<Expression>
    {
        public bool Equals(Expression x, Expression y)
        {
            return x.Type == y.Type && x.NodeType == y.NodeType && ChildrenEquals(x, y);
        }

        private bool ChildrenEquals(Expression x, Expression y)
        {
            var binaryX = x as BinaryExpression;
            var binaryY = y as BinaryExpression;
            if (binaryX != null && binaryY != null)
                return Equals(binaryX.Left, binaryY.Left)
                    && Equals(binaryX.Right, binaryY.Right);

            var unaryX = x as UnaryExpression;
            var unaryY = y as UnaryExpression;
            if (unaryX != null && unaryY != null)
                return Equals(unaryX.Operand, unaryY.Operand);

            var funcX = x as LambdaExpression;
            var funcY = y as LambdaExpression;
            if (funcX != null && funcY != null)
                return Equals(funcX.Body, funcY.Body);

            var callX = x as MethodCallExpression;
            var callY = y as MethodCallExpression;
            if (callX != null && callY != null)
                return callX.Arguments.SequenceEqual(callY.Arguments, this)
                       && callX.Method == callY.Method;

            return true;
        }

        public int GetHashCode(Expression obj)
        {
            return obj.GetHashCode();
        }
    }
}