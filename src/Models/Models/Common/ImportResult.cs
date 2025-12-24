namespace Models.Common;

public class ImportResult
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<ImportError> Errors { get; set; } = new();
}

public class ImportError
{
    public int RowNumber { get; set; }
    public string Field { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

