namespace Shared.Model.Base
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public string? ApiName { get; set; }

        public ApiResponse()
        {

        }
        public ApiResponse(T? data, string message = "", string apiName = "")
        {

            Data = data;
            Message = message;
            ApiName = apiName;
        }
    }
}
