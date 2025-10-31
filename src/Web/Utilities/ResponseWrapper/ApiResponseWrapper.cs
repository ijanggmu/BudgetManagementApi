using System.Collections.Generic;
using System.Linq;

namespace BeemaEdgeApi.Utilities.ResponseWrapper;

public static class VersionConstant
{
    public const string _version = "1.0.0";
}

public class ErrorApiResponse
{
    public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>() {
            { "api",
               new {
                       apiVersion = "1.0",
                       buildVersion = VersionConstant._version
                }
            }
        };
    public ErrorApiResponse()
    {

    }

    public List<ErrorDetail> Errors { get; set; }

    private ErrorApiResponse(string errorMessage, int code)
    {
        Errors = new List<ErrorDetail>
                {
                    new ErrorDetail
                    {
                        Code = code,
                        Title = errorMessage,
                        Detail = errorMessage
                    }
                };
    }

    private ErrorApiResponse(Dictionary<string, string> errorMessages, int code)
    {
        Errors = errorMessages.Select(kv => new ErrorDetail
        {
            Code = code,
            Title = kv.Key,
            Detail = kv.Value
        }).ToList();
    }

    private ErrorApiResponse(List<string> errorMessages, int code)
    {
        Errors = errorMessages.Select(x => new ErrorDetail
        {
            Code = code,
            Title = x,
            Detail = x,
        }).ToList();
    }

    public static ErrorApiResponse WrapError(string errorMessage, int code = 400)
    {
        return new ErrorApiResponse(errorMessage, code);
    }

    public static ErrorApiResponse WrapError(List<string> errorMessages, int code = 400)
    {
        return new ErrorApiResponse(errorMessages, code);
    }

    public static ErrorApiResponse WrapError(Dictionary<string, string> errorMessages, int code = 400)
    {
        return new ErrorApiResponse(errorMessages, code);
    }
}


public class SuccessApiResponse<T>
{
    public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>() {
            { "api",
               new {
                       apiVersion = "1.0",
                       buildversion = VersionConstant._version
                }
            }
        };
    public T Data { get; set; }

    private SuccessApiResponse(T data)
    {
        Data = data;
    }

    public static SuccessApiResponse<T> WrapSuccess(T data)
    {
        return new SuccessApiResponse<T>(data);
    }
}

public class SuccessPaginateApiResponse<T1, T2>
{
    public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>() {
            { "api",
               new {
                       apiVersion = "1.0",
                       buildversion = VersionConstant._version
                }
            } };

    public T1 Data { get; set; }

    private SuccessPaginateApiResponse(T1 responseData, T2 pagination)
    {
        Data = responseData;
        Meta.Add("pagination", pagination);
    }
    public static SuccessPaginateApiResponse<T1, T2> WrapSuccess(T1 responseData, T2 pagination)
    {
        return new SuccessPaginateApiResponse<T1, T2>(responseData, pagination);
    }
}

public class SuccessPaginateApiResponse<T1, T2, T3>
{
    public Dictionary<string, object> Meta { get; set; } = new Dictionary<string, object>() {
           { "api",
               new {
                       apiVersion = "1.0",
                       buildversion = VersionConstant._version
                }
            } };

    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

    private SuccessPaginateApiResponse(T1 responseData, T2 pagination, T3 additionalData, string additionalFieldName)
    {
        Data.Add(additionalFieldName, additionalData);
        Data.Add("data", responseData);
        Meta.Add("pagination", pagination);
    }
    public static SuccessPaginateApiResponse<T1, T2, T3> WrapSuccess(T1 responseData, T2 pagination, T3 additionalData, string additionalFieldName) => new SuccessPaginateApiResponse<T1, T2, T3>(responseData, pagination, additionalData, additionalFieldName);
}

public class ErrorDetail
{
    public string Title { get; set; }
    public string Detail { get; set; }
    public int Code { get; set; }
}

