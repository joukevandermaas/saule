﻿namespace Saule
{
    internal static class Constants
    {
        public const string MediaType = "application/vnd.api+json";

        public static class PropertyNames
        {
            public const string ResourceDescriptor = "Saule_ResourceDescriptor";
            public const string QueryContext = "Saule_QueryContext";
            public const string PreprocessResult = "Saule_PreprocessedResult";
            public const string WebApiRequestContext = "MS_RequestContext";
        }

        public static class QueryNames
        {
            public const string PageNumber = "page.number";
            public const string Sorting = "sort";
            public const string Filtering = "filter";
            public const string Including = "include";
        }
    }
}
