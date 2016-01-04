using System;
using System.Collections;
using System.Linq;
using Saule.Http;

namespace Saule.Queries.Filtering
{
    internal class FilteringInterpreter
    {
        private readonly FilteringContext _context;
        private readonly ApiResource _resource;

        public FilteringInterpreter(FilteringContext context, ApiResource resource)
        {
            _context = context;
            _resource = resource;
        }

        public IQueryable Apply(IQueryable queryable)
        {
            if (_context.Properties.Any())
            {
                 return _context.Properties
                    .Select(p => p.Name == "Id" ? new FilteringProperty(_resource.IdProperty, p.Value) : p)
                    .Aggregate(queryable, ApplyProperty);
            }

            return queryable;
        }

        public IEnumerable Apply(IEnumerable enumerable)
        {
            if (_context.Properties.Any())
            {
                 return _context.Properties
                    .Select(p => p.Name == "Id" ? new FilteringProperty(_resource.IdProperty, p.Value) : p)
                    .Aggregate(enumerable, ApplyProperty);
            }

            return enumerable;
        }

        private static JsonApiException MissingProperty(string property, Exception ex)
        {
            return new JsonApiException(ErrorType.Client, $"Attribute '{property.ToDashed()}' not found.", ex);
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