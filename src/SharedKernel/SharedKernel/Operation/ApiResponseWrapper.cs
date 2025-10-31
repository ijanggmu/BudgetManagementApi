using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace SharedKernel.Operation
{
    public static class VersionConstant
    {
        public const string ApiVersion = "1";
        public const string BuildVersion = "1.0.0";
    }

    public static class MetadataHelper
    {
        public static Dictionary<string, object> GetDefaultMeta()
        {

            return new Dictionary<string, object>
        {
            {
                "api",
                new
                {
                    ApiVersion = VersionConstant.ApiVersion,
                    BuildVersion = VersionConstant.BuildVersion,
                }
            }
        };
        }
    }


    public class ErrorApiResponse
    {
        public Dictionary<string, object> Meta { get; init; } = MetadataHelper.GetDefaultMeta();
        public List<ErrorDetail> Errors { get; init; }

        private ErrorApiResponse(IEnumerable<ErrorDetail> errors)
        {
            Errors = errors.ToList();
        }

        public static ErrorApiResponse WrapError(string errorMessage, int code)
        {
            return new ErrorApiResponse(new List<ErrorDetail>
            {
                new()
                {
                    Code = code == 0?(int)HttpStatusCode.BadRequest :code,
                    Title = errorMessage,
                    Detail = errorMessage
                }
            });
        }

        public static ErrorApiResponse WrapError(Dictionary<string, string> errorMessages, int code = 400)
        {
            return new ErrorApiResponse(errorMessages.Select(kv => new ErrorDetail
            {
                Code = code,
                Title = kv.Key,
                Detail = kv.Value
            }));
        }

        public static ErrorApiResponse WrapError(IEnumerable<string> errorMessages, int code = 400)
        {
            return new ErrorApiResponse(errorMessages.Select(message => new ErrorDetail
            {
                Code = code,
                Title = message,
                Detail = message
            }));
        }
    }

    public class SuccessApiResponse<T>
    {
        public Dictionary<string, object> Meta { get; init; } = MetadataHelper.GetDefaultMeta();
        public T Data { get; init; }

        private SuccessApiResponse(T data)
        {
            Data = data;
        }

        public static SuccessApiResponse<T> WrapSuccess(T data)
        {
            return new SuccessApiResponse<T>(data);
        }
    }

    public class SuccessPaginateApiResponse<TData, TPagination>
    {
        public Dictionary<string, object> Meta { get; init; } = MetadataHelper.GetDefaultMeta();
        public TData Data { get; init; }

        private SuccessPaginateApiResponse(TData data, TPagination pagination)
        {
            Data = data;
            Meta["pagination"] = pagination;
        }

        public static SuccessPaginateApiResponse<TData, TPagination> WrapSuccess(TData data, TPagination pagination)
        {
            return new SuccessPaginateApiResponse<TData, TPagination>(data, pagination);
        }
    }

    public class SuccessPaginateApiResponse<TData, TPagination, TAdditional>
    {
        public Dictionary<string, object> Meta { get; init; } = MetadataHelper.GetDefaultMeta();
        public Dictionary<string, object> Data { get; init; }

        private SuccessPaginateApiResponse(TData data, TPagination pagination, TAdditional additionalData, string additionalFieldName)
        {
            Data = new Dictionary<string, object>
            {
                { "data", data },
                { additionalFieldName, additionalData }
            };
            Meta["pagination"] = pagination;
        }

        public static SuccessPaginateApiResponse<TData, TPagination, TAdditional> WrapSuccess(
            TData data,
            TPagination pagination,
            TAdditional additionalData,
            string additionalFieldName)
        {
            return new SuccessPaginateApiResponse<TData, TPagination, TAdditional>(data, pagination, additionalData, additionalFieldName);
        }
    }

    public class ErrorDetail
    {
        public string Title { get; init; }
        public string Detail { get; init; }
        public int Code { get; init; }
    }
}
