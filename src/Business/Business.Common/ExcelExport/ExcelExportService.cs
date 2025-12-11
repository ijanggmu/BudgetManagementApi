using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Business.AdminPortalApi.ExcelExport;

public class ExcelExportService : IExcelExportService
{
    public async Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, Dictionary<string, string>? columnMappings = null) where T : class
    {
        return await Task.Run(() =>
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet(sheetName);

            var dataList = data.ToList();
            if (!dataList.Any())
            {
                // Return empty workbook if no data
                using var ms = new MemoryStream();
                workbook.Write(ms);
                return ms.ToArray();
            }

            // Get properties of the type
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToList();

            // Create header row
            var headerRow = sheet.CreateRow(0);
            var headerStyle = workbook.CreateCellStyle();
            var headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            headerFont.FontHeightInPoints = 11;
            headerStyle.SetFont(headerFont);
            headerStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            headerStyle.BorderBottom = BorderStyle.Thin;
            headerStyle.BorderTop = BorderStyle.Thin;
            headerStyle.BorderLeft = BorderStyle.Thin;
            headerStyle.BorderRight = BorderStyle.Thin;

            // Create data style
            var dataStyle = workbook.CreateCellStyle();
            dataStyle.BorderBottom = BorderStyle.Thin;
            dataStyle.BorderTop = BorderStyle.Thin;
            dataStyle.BorderLeft = BorderStyle.Thin;
            dataStyle.BorderRight = BorderStyle.Thin;

            // Write headers
            for (int i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                var headerName = columnMappings?.ContainsKey(property.Name) == true
                    ? columnMappings[property.Name]
                    : property.Name;

                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headerName);
                cell.CellStyle = headerStyle;
            }

            // Auto-size columns
            for (int i = 0; i < properties.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            // Write data rows
            for (int rowIndex = 0; rowIndex < dataList.Count; rowIndex++)
            {
                var row = sheet.CreateRow(rowIndex + 1);
                var item = dataList[rowIndex];

                for (int colIndex = 0; colIndex < properties.Count; colIndex++)
                {
                    var property = properties[colIndex];
                    var cell = row.CreateCell(colIndex);
                    cell.CellStyle = dataStyle;

                    var value = property.GetValue(item);
                    if (value != null)
                    {
                        switch (value)
                        {
                            case string str:
                                cell.SetCellValue(str);
                                break;
                            case int intVal:
                                cell.SetCellValue(intVal);
                                break;
                            case long longVal:
                                cell.SetCellValue(longVal);
                                break;
                            case decimal decimalVal:
                                cell.SetCellValue((double)decimalVal);
                                break;
                            case double doubleVal:
                                cell.SetCellValue(doubleVal);
                                break;
                            case float floatVal:
                                cell.SetCellValue(floatVal);
                                break;
                            case bool boolVal:
                                cell.SetCellValue(boolVal);
                                break;
                            case DateTime dateTimeVal:
                                cell.SetCellValue(dateTimeVal);
                                var dateStyle = workbook.CreateCellStyle();
                                dateStyle.CloneStyleFrom(dataStyle);
                                var dateFormat = workbook.CreateDataFormat();
                                dateStyle.DataFormat = dateFormat.GetFormat("yyyy-mm-dd hh:mm:ss");
                                cell.CellStyle = dateStyle;
                                break;
                            case DateOnly dateOnlyVal:
                                cell.SetCellValue(dateOnlyVal.ToString("yyyy-MM-dd"));
                                break;
                            case IEnumerable<string> stringList:
                                cell.SetCellValue(string.Join(", ", stringList));
                                break;
                            default:
                                cell.SetCellValue(value.ToString() ?? string.Empty);
                                break;
                        }
                    }
                    else
                    {
                        cell.SetCellValue(string.Empty);
                    }
                }
            }

            // Auto-size columns again after adding data
            for (int i = 0; i < properties.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            using var memoryStream = new MemoryStream();
            workbook.Write(memoryStream);
            return memoryStream.ToArray();
        });
    }
}

