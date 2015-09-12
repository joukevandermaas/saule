using System;

namespace Saule
{
    public static class JsonApi
    {
        public static ApiResponse<T> ToApiResponse<T>(this T obj, Type modelType)
        {
            var model = GetModelFor(modelType);

            return new ApiResponse<T>(obj, model);
        }

        private static ApiResource GetModelFor(Type modelType)
        {
            if (!modelType.IsSubclassOf(typeof(ApiResource)))
                throw new ArgumentException("Model must be a subclass of Saule.ApiModel", "modelType");

            try
            {
                var model = Activator.CreateInstance(modelType);

                return (ApiResource)model;
            }
            catch (MissingMethodException ex)
            {
                throw new ArgumentException("Model must have a parameterless constructor", "modelType", ex);
            }
        }
    }
}
