using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Saule.Queries;
using Saule.Queries.Filtering;
using Saule.Queries.Including;
using Saule.Queries.Sorting;

namespace Saule.Http
{
    /// <summary>
    /// Indicates that the action should not include related data by default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NoDefaultIncludedAttribute : ActionFilterAttribute
    {
    }
}
