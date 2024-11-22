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
        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataSet = reader.AsDataSet();
        var table = dataSet.Tables[tableIndex];

        var headersRowIndex = FindHeaderRowIndex(table);

        if (headersRowIndex < 0)
            throw new Exception("Table must contain a header row.");

        return table.Rows[headersRowIndex]
            .ItemArray
            .Select(cell => cell?.ToString() ?? string.Empty)
            .ToList();
    }

    public static NodeTable ReadFile(string filePath, short tableIndex = 0)
    {
        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataSet = reader.AsDataSet();
        var table = dataSet.Tables[tableIndex];

        int headersRowIndex = FindHeaderRowIndex(table);

        if (headersRowIndex < 0)
            throw new Exception("Table must contain header row.");

        return ParseDataTable(table, headersRowIndex);
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

    private static NodeTable ParseDataTable(DataTable table, int headersRowIndex)
    {
        if (table.Rows.Count == 0)
            return new NodeTable();

        var parameterTypes = InitializeParameterTypes(table, headersRowIndex);

        NodeTable nodeTable = new NodeTable
        {
            Name = table.TableName,
            ParameterTypes = parameterTypes
        };

        List<NodeObject> nodes = new List<NodeObject>();

        NodeObject node;
        for (int row = headersRowIndex + 1; row < table.Rows.Count; ++row)
        {
            node = ParseDataRow(table.Rows[row], parameterTypes);
            nodes.Add(node);
        }

        nodeTable.NodeObjects = nodes;

        return nodeTable;
    }

    private static List<ParameterType> InitializeParameterTypes(DataTable table, int headersRowIndex)
    {
        var parameterTypes = new List<ParameterType>();
        var rowHeaders = table.Rows[headersRowIndex];

        for (int col = 1; col < table.Columns.Count; ++col)
        {
            var parameterName = rowHeaders[col]?.ToString() ?? string.Empty;
            var parameterType = new ParameterType()
            {
                Name = parameterName,
            };

            parameterTypes.Add(parameterType);
        }

        return parameterTypes;
    }

    private static NodeObject ParseDataRow(DataRow rowData, List<ParameterType> parameterTypes)
    {
        NodeObject node = new NodeObject()
        {
            Name = rowData[0].ToString() ?? string.Empty
        };

        var rowAmount = rowData.Table.Columns.Count;
        for (int col = 1; col < rowAmount; ++col)
        {
            var parameterValue = rowData[col]?.ToString() ?? string.Empty;

            node.Parameters.Add(new NodeParameter
            {
                Value = parameterValue,
                ParameterType = parameterTypes[col - 1]
            });
        }

        return node;
    }
}
