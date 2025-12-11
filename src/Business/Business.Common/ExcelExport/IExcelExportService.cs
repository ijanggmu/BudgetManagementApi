using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.AdminPortalApi.ExcelExport;

public interface IExcelExportService
{
    /// <summary>
    /// Exports a list of data to Excel format
    /// </summary>
    /// <typeparam name="T">The type of data to export</typeparam>
    /// <param name="data">List of data to export</param>
    /// <param name="sheetName">Name of the Excel sheet</param>
    /// <param name="columnMappings">Dictionary mapping property names to column headers</param>
    /// <returns>Excel file as byte array</returns>
    Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, Dictionary<string, string>? columnMappings = null) where T : class;
}

