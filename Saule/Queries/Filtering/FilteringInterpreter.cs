using System;
using System.Collections;
using System.Linq;
using Saule.Http;

namespace Saule.Queries.Filtering
{
    internal class FilteringInterpreter
    {
        private readonly FilteringContext _context;

        public FilteringInterpreter(FilteringContext context)
        {
            _context = context;
        }

        public IQueryable Apply(IQueryable queryable)
        {
            return _context.Properties.Any()
                ? _context.Properties.Aggregate(queryable, ApplyProperty)
                : queryable;
        }

        public IEnumerable Apply(IEnumerable enumerable)
        {
            return _context.Properties.Any()
                ? _context.Properties.Aggregate(enumerable, ApplyProperty)
                : enumerable;
        }

        private static JsonApiException MissingProperty(string property, Exception ex)
        {
            return new JsonApiException($"Attribute '{property.ToDashed()}' not found.", ex);
        }

        private IEnumerable ApplyProperty(IEnumerable enumerable, FilteringProperty property)
        {
            try
            {
                var elementType = enumerable
                    .GetType()
                    .GetInterface("IEnumerable`1")
                    .GetGenericArguments()
                    .First();

                enumerable = enumerable.ApplyQuery(
                    QueryMethod.Where,
                    Lambda.SelectPropertyValue(elementType, property.Name, property.Value, _context.QueryFilters))
                    as IEnumerable;

                return enumerable;
            }
            catch (ArgumentException ex)
            {
                throw MissingProperty(property.Name, ex);
            }
        }

        private IQueryable ApplyProperty(IQueryable queryable, FilteringProperty property)
        {
            try
            {
                queryable = queryable.ApplyQuery(
                    QueryMethod.Where,
                    Lambda.SelectPropertyValue(queryable.ElementType, property.Name, property.Value, _context.QueryFilters))
                    as IQueryable;
                return queryable;
            }
            catch (ArgumentException ex)
            {
                throw MissingProperty(property.Name, ex);
            }
        }
    }
}