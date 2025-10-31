namespace Models.Common.Policy.UnderwritingAPI.ResponseParameter
{
    public class ApiResponse<T>
    {
        public MetaDetail Metadata { get; set; }
        public T Data { get; set; }
        public ErrorDetail Error { get; set; }
        public static ApiResponse<T> Fail(ErrorDetail error)
        {
            return new ApiResponse<T> {  Error = error };
        }
        public static ApiResponse<T> Success(MetaDetail metaDetail, T data)
        {
            return new ApiResponse<T> { Metadata = metaDetail,  Data = data };
        }

        //public static ApiResponse<MetaDetail> Meta()
        //{
        //    return new ApiResponse<MetaDetail>
        //    {

        //    };
        //}
        //private static MetaDetail GetMetaDetail()
        //{
        //    return new MetaDetail
        //    {
        //        Copyright = "copyright",
        //        Email = "email",
        //        Api = new ApiDetail
        //        {
        //            Version = "version"
        //        }
        //    };
        //}
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


}
