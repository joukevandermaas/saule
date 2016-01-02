using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Saule.Queries.Filtering
{
    internal class TypeCorrectingVisitor : ExpressionVisitor
    {
        private readonly Type _propertyType;

        public TypeCorrectingVisitor(Type propertyType)
        {
            _propertyType = propertyType;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var funcType = typeof(Func<,,>)
                .MakeGenericType(_propertyType, _propertyType, typeof(bool));
            var expressionFactory = CreateExpressionFactory(funcType);

            return expressionFactory.Invoke(null, new object[]
            {
                Visit(node.Body), new[]
                {
                    Expression.Parameter(_propertyType, node.Parameters[0].Name),
                    Expression.Parameter(_propertyType, node.Parameters[1].Name)
                }
            }) as Expression;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return Expression.Parameter(_propertyType, node.Name);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var item = Visit(node.Operand);

            return Expression.MakeUnary(node.NodeType, item, node.Type, node.Method);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var left = Visit(node.Left);
            var right = Visit(node.Right);

            return Expression.MakeBinary(node.NodeType, left, right);
        }

        private static MethodInfo CreateExpressionFactory(Type funcType)
        {
            var expressionFactory = typeof(Expression).GetMethods()
                .Where(m => m.Name == "Lambda")
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == 2 && x.Args.Length == 1)
                .Select(x => x.Method)
                .First()
                .MakeGenericMethod(funcType);
            return expressionFactory;
        }
    }
}
