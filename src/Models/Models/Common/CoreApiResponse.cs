using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common;
public class CoreApiResponse<T>
{
    public MetaDetail Metadata { get; set; }
    public T Data { get; set; }
    public ErrorDetail Error { get; set; }
    public static CoreApiResponse<T> Fail(ErrorDetail error)
    {
        return new CoreApiResponse<T> { Error = error };
    }
    public static CoreApiResponse<T> Success(MetaDetail metaDetail, T data)
    {
        return new CoreApiResponse<T> { Metadata = metaDetail, Data = data };
    }

}

public class MetaDetail
{
    public string Copyright { get; set; }
    public string Email { get; set; }
    public ApiDetail Api { get; set; }
    public PaginationDetail pagination { get; set; }
}
public class ApiDetail
{
    public string Version { get; set; }
}
public class PaginationDetail
{
    public int Total { get; set; }
    public int Count { get; set; }
    public int PerPage { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPage { get; set; }
}

public class ErrorDetail
{
    public int Code { get; set; }
    public string Message { get; set; }
}
