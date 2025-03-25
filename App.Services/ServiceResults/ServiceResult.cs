using System.Net;
using System.Text.Json.Serialization;

namespace App.Services.ServiceResults
{
    public class ServiceResult<T> where T : class
    {
        public T? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        [JsonIgnore] public bool IsSuccess => ErrorMessages.Count == 0 || ErrorMessages is null;
        [JsonIgnore] public bool IsFailure => !IsSuccess;
        [JsonIgnore] public HttpStatusCode StatusCode { get; set; }
        [JsonIgnore] public string? UrlAsCreated { get; set; }

        public static ServiceResult<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ServiceResult<T>
            {
                Data = data,
                StatusCode = statusCode
            };
        }

        public static ServiceResult<T> SuccessAsCreated(T data, string urlAsCreated)
        {
            return new ServiceResult<T>
            {
                Data = data,
                StatusCode = HttpStatusCode.Created,
                UrlAsCreated = urlAsCreated
            };
        }

        public static ServiceResult<T> Failure(List<string> errorMessages, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ServiceResult<T>
            {
                ErrorMessages = errorMessages,
                StatusCode = statusCode
            };
        }

        public static ServiceResult<T> Failure(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ServiceResult<T>
            {
                ErrorMessages = new List<string> { errorMessage },
                StatusCode = statusCode
            };
        }
    }

    public class ServiceResult
    {
        public List<string>? ErrorMessages { get; set; }
        [JsonIgnore] public bool IsSuccess => ErrorMessages.Count == 0 || ErrorMessages is null;
        [JsonIgnore] public bool IsFailure => !IsSuccess;
        [JsonIgnore] public HttpStatusCode StatusCode { get; set; }

        public static ServiceResult Success(HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ServiceResult
            {
                StatusCode = statusCode
            };
        }

        public static ServiceResult Failure(List<string> errorMessages, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ServiceResult
            {
                ErrorMessages = errorMessages,
                StatusCode = statusCode
            };
        }

        public static ServiceResult Failure(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ServiceResult
            {
                ErrorMessages = new List<string> { errorMessage },
                StatusCode = statusCode
            };
        }
    }
}
