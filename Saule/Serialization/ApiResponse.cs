namespace Saule.Serialization
{
    internal class ApiResponse
    {
        public ApiResponse(object obj, ApiResource resource)
        {
            Object = obj;
            Resource = resource;
        }

        public object Object { get; }
        public ApiResource Resource { get; }
    }
}