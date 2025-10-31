//namespace Models.Common;

//public static class VersionConstants
//{
//    public const string ApiVersion = "2.0";
//    public const string BuildVersion = "1.2.9";
//}

//public abstract class ApiResponse
//{
//    public Dictionary<string, object> Meta { get; } = InitializeMeta();

//    private static Dictionary<string, object> InitializeMeta()
//    {
//        return new Dictionary<string, object>
//            {
//                {
//                    "api", new
//                    {
//                        apiVersion = VersionConstants.ApiVersion,
//                        buildVersion = VersionConstants.BuildVersion
//                    }
//                }
//            };
//    }
//}

//public class Result<T> : ApiResponse
//{
//    public T Data { get; }
//    public List<ErrorDetail> Errors { get; }
//    public bool IsSuccess => Errors == null || Errors.Count == 0;

//    private Result(T data, List<ErrorDetail> errors)
//    {
//        Data = data;
//        Errors = errors;
//    }

//    // Factory methods for success and failure
//    public static Result<T> Success(T data)
//    {
//        return new Result<T>(data, null);
//    }

//    public static Result<T> Failure(string errorMessage, int code = 400)
//    {
//        return new Result<T>(default, new List<ErrorDetail>
//            {
//                new ErrorDetail
//                {
//                    Code = code,
//                    Title = errorMessage,
//                    Detail = errorMessage
//                }
//            });
//    }

//    public static Result<T> Failure(Dictionary<string, string> errorMessages, int code = 400)
//    {
//        return new Result<T>(default, errorMessages.Select(kv => new ErrorDetail
//        {
//            Code = code,
//            Title = kv.Key,
//            Detail = kv.Value
//        }).ToList());
//    }

//    public static Result<T> Failure(List<string> errorMessages, int code = 400)
//    {
//        return new Result<T>(default, errorMessages.Select(message => new ErrorDetail
//        {
//            Code = code,
//            Title = message,
//            Detail = message
//        }).ToList());
//    }
//}

//public class PaginatedResult<TData, TPagination> : Result<TData>
//{
//    public TPagination Pagination { get; }

//    private PaginatedResult(TData data, TPagination pagination, List<ErrorDetail> errors)
//        : base(data, errors)
//    {
//        Pagination = pagination;
//        if (pagination != null)
//            Meta.Add("pagination", pagination);
//    }

//    public static PaginatedResult<TData, TPagination> Success(TData data, TPagination pagination)
//    {
//        return new PaginatedResult<TData, TPagination>(data, pagination, null);
//    }

//    public static PaginatedResult<TData, TPagination> Failure(List<ErrorDetail> errors)
//    {
//        return new PaginatedResult<TData, TPagination>(default, default, errors);
//    }
//}

//public class ExtendedResult<TData, TPagination, TAdditional> : PaginatedResult<TData, TPagination>
//{
//    public Dictionary<string, object> AdditionalData { get; } = new Dictionary<string, object>();

//    private ExtendedResult(TData data, TPagination pagination, TAdditional additionalData, string additionalFieldName, List<ErrorDetail> errors)
//        : base(data, pagination, errors)
//    {
//        if (additionalData != null && !string.IsNullOrEmpty(additionalFieldName))
//            AdditionalData.Add(additionalFieldName, additionalData);
//    }

//    public static ExtendedResult<TData, TPagination, TAdditional> Success(TData data, TPagination pagination, TAdditional additionalData, string additionalFieldName)
//    {
//        return new ExtendedResult<TData, TPagination, TAdditional>(data, pagination, additionalData, additionalFieldName, null);
//    }

//    public static ExtendedResult<TData, TPagination, TAdditional> Failure(List<ErrorDetail> errors)
//    {
//        return new ExtendedResult<TData, TPagination, TAdditional>(default, default, default, null, errors);
//    }
//}

//public class ErrorDetail
//{
//    public string Title { get; set; }
//    public string Detail { get; set; }
//    public int Code { get; set; }
//}
