
using System.Net;

namespace SharedKernel.Operation;

public interface IResult<T>
{
    string Error { get; }
    int ErrorCode { get; }
    bool IsSuccess { get; }
    T Data { get; }
    Pagination Pagination { get; }
    object OptionalData { get; }
    IReadOnlyDictionary<string, string> ErrorMessages { get; }
}

public class Pagination
{
    public int PageSize { get; init; }
    public int CurrentPage { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
}

public class Result<T> : IResult<T>
{
    public bool IsSuccess { get; private init; }
    public HttpStatusCode StatusCode { get; set; }
    public string Error { get; private init; } = string.Empty;
    public int ErrorCode { get; private init; }
    public T Data { get; private init; } = default!;
    public Pagination Pagination { get; private init; } = default!;
    public object OptionalData { get; private init; }
    public IReadOnlyDictionary<string, string> ErrorMessages { get; private init; }

    // Factory methods
    public static Result<T> Success(
        T result,
        Pagination pagination = null,
        object optionalData = null,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new Result<T>
        {
            IsSuccess = true,
            StatusCode = statusCode,
            Data = result,
            Pagination = pagination,
            OptionalData = optionalData
        };
    }

    public static Result<T> Failed(
        string error,
        int errorCode = 0,
        IReadOnlyDictionary<string, string> errorMessages = null,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Error = error,
            ErrorCode = errorCode,
            ErrorMessages = errorMessages,
            StatusCode = statusCode
        };
    }

    // Implicit operator for Success with Result only
    public static implicit operator Result<T>(T result)
    {
        return Success(result);
    }

    // Implicit operator for Success with Pagination and OptionalData
    public static implicit operator Result<T>((T Result, Pagination Pagination, object OptionalData) tuple)
    {
        return Success(tuple.Result, tuple.Pagination, tuple.OptionalData);
    }

    // Implicit operator for Failed
    public static implicit operator Result<T>(string error)
    {
        return Failed(error);
    }
}



// Example Usage:
// Success with a single result
//var singleResult = OperationResult<string>.Success("Operation completed");

// Success with a paginated result
//var paginatedResult = OperationResult<List<string>>.Success(
//    new List<string> { "Item1", "Item2" },
//    new Pagination { PageSize = 10, CurrentPage = 1, TotalItems = 50 });

// Failed result
//var failedResult = OperationResult<string>.Failed("Something went wrong", 500);
