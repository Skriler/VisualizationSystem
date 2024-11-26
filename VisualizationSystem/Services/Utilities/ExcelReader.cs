using System.Data;
using System.Text;
using ExcelDataReader;

namespace VisualizationSystem.Services.Utilities;

public class ExcelReader
{
    public ExcelReader()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public List<DataTable> GetExcelTables(string filePath)
    {
        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataSet = reader.AsDataSet();

        return dataSet.Tables
            .Cast<DataTable>()
            .ToList();
    }
}
