using System.Data;
using System.Text;
using ExcelDataReader;
using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Services.Utilities;

public static class ExcelReader
{
    static ExcelReader()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static List<string> GetColumnHeaders(string filePath, short tableIndex = 0)
    {
        var table = GetExcelTable(filePath, tableIndex);
        var headersRowIndex = FindHeaderRowIndex(table);

        if (headersRowIndex < 0)
            throw new Exception("Table must contain a header row.");

        return GetRowValues(table.Rows[headersRowIndex]);
    }

    public static NodeTable ReadFile(string filePath, int nameColumnIndex, short tableIndex = 0)
    {
        var table = GetExcelTable(filePath, tableIndex);
        var headersRowIndex = FindHeaderRowIndex(table);

        if (headersRowIndex < 0)
            throw new Exception("Table must contain header row.");

        return ParseNodeTable(table, headersRowIndex, nameColumnIndex);
    }

    private static DataTable GetExcelTable(string filePath, short tableIndex)
    {
        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataSet = reader.AsDataSet();

        return dataSet.Tables[tableIndex];
    }

    private static List<string> GetRowValues(DataRow row)
    {
        return row.ItemArray
            .Select(cell => cell?.ToString() ?? string.Empty)
            .ToList();
    }

    private static int FindHeaderRowIndex(DataTable table)
    {
        var headersRowIndex = table.AsEnumerable()
            .Select((row, index) => new { Row = row, Index = index })
            .FirstOrDefault(r => r.Row.ItemArray.All(cell => IsCellFilled(cell)));

        return headersRowIndex != null ? headersRowIndex.Index : -1;
    }

    private static bool IsCellFilled(object? cell)
    {
        return cell != null && !string.IsNullOrWhiteSpace(cell.ToString());
    }

    private static NodeTable ParseNodeTable(DataTable table, int headersRowIndex, int nameColumnIndex)
    {
        if (table.Rows.Count == 0)
            return new NodeTable();

        var parameterTypes = InitializeParameterTypes(table, headersRowIndex);

        var nodeTable = new NodeTable
        {
            Name = table.TableName,
            ParameterTypes = parameterTypes
        };

        var nodes = new List<NodeObject>();

        for (int row = headersRowIndex + 1; row < table.Rows.Count; ++row)
        {
            var node = ParseDataRow(table.Rows[row], parameterTypes, nameColumnIndex);
            nodes.Add(node);
        }

        nodeTable.NodeObjects = nodes;

        return nodeTable;
    }

    private static List<ParameterType> InitializeParameterTypes(DataTable table, int headersRowIndex)
    {
        var parameterTypes = new List<ParameterType>();
        var rowHeaders = table.Rows[headersRowIndex];

        for (int col = 0; col < table.Columns.Count; ++col)
        {
            var parameterType = new ParameterType()
            {
                Name = rowHeaders[col].ToString() ?? string.Empty,
            };

            parameterTypes.Add(parameterType);
        }

        return parameterTypes;
    }

    private static NodeObject ParseDataRow(DataRow rowData, List<ParameterType> parameterTypes, int nameColumnIndex)
    {
        var node = new NodeObject()
        {
            Name = rowData[nameColumnIndex].ToString() ?? string.Empty
        };

        var colAmount = rowData.Table.Columns.Count;
        for (int col = 0; col < colAmount; ++col)
        {
            if (col == nameColumnIndex)
                continue;

            var parameterValue = rowData[col]?.ToString() ?? string.Empty;

            node.Parameters.Add(new NodeParameter
            {
                Value = parameterValue,
                ParameterType = parameterTypes[col]
            });
        }

        return node;
    }
}
