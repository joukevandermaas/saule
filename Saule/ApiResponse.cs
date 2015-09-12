using System.Collections.Generic;

namespace Saule
{
    public class ApiResponse<T>
    {
        private ApiModel _model;
        private T _object;

        public ApiResponse(T obj, ApiModel model)
        {
            _object = obj;
            _model = model;
        }
    }
}