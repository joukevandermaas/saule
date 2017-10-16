using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Saule.Queries;

namespace Saule.Http
{
    internal class CorePreprocessInput : PreprocessInput
    {
        private readonly ActionExecutedContext _context;

        public CorePreprocessInput(ActionExecutedContext context)
        {
            _context = context;
        }

        public override object Content => throw new NotImplementedException();

        public override ApiResource ResourceDescriptor => throw new NotImplementedException();

        public override int StatusCode => throw new NotImplementedException();

        public override bool IsErrorContent => throw new NotImplementedException();

        public override Uri RequestUri => throw new NotImplementedException();

        public override string RouteTemplate => throw new NotImplementedException();

        public override string VirtualPathRoot => throw new NotImplementedException();

        public override QueryContext QueryContext => throw new NotImplementedException();
    }
}