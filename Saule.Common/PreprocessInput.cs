using System;
using Saule.Queries;

namespace Saule
{
    internal abstract class PreprocessInput
    {
        public abstract object Content { get; }

        public abstract ApiResource ResourceDescriptor { get; }

        public abstract int StatusCode { get; }

        public abstract bool IsErrorContent { get; }

        public abstract Uri RequestUri { get; }

        public abstract string RouteTemplate { get; }

        public abstract string VirtualPathRoot { get; }

        public abstract QueryContext QueryContext { get; }
    }
}