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
        var headerRowIndex = FindHeaderRowIndex(table);

        if (headerRowIndex < 0)
            throw new Exception("Table must contain header row.");

        return ParseNodeTable(table, headerRowIndex, nameColumnIndex);
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
        var headerRowIndex = table.AsEnumerable()
            .Select((row, index) => new { Row = row, Index = index })
            .FirstOrDefault(r => r.Row.ItemArray.All(IsCellFilled));

        return headerRowIndex?.Index ?? -1;
    }

    private static bool IsCellFilled(object? cell)
    {
        return cell != null && !string.IsNullOrWhiteSpace(cell.ToString());
    }

    private static NodeTable ParseNodeTable(DataTable table, int headerRowIndex, int nameColumnIndex)
    {
        if (table.Rows.Count == 0)
            return new NodeTable();

        var parameterTypes = ParseParameterTypes(table, headerRowIndex);
        var nodes = ParseNodes(table, parameterTypes, headerRowIndex, nameColumnIndex);

        var duplicateNames = nodes
            .GroupBy(node => node.Name)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateNames.Any())
            throw new InvalidOperationException($"Name duplication: {string.Join(", ", duplicateNames)}");

        var nodeTable = new NodeTable
        {
            Name = table.TableName,
            ParameterTypes = parameterTypes,
            NodeObjects = nodes,
        };

        return nodeTable;
    }

    private static List<ParameterType> ParseParameterTypes(DataTable table, int headerRowIndex)
    {
        var parameterTypes = new List<ParameterType>();
        var headerRow = table.Rows[headerRowIndex];

        for (int col = 0; col < table.Columns.Count; ++col)
        {
            var parameterType = new ParameterType()
            {
                Name = headerRow[col].ToString() ?? string.Empty,
            };

            parameterTypes.Add(parameterType);
        }

        return parameterTypes;
    }

    private static List<NodeObject> ParseNodes(DataTable table, List<ParameterType> parameterTypes, int headerRowIndex, int nameColumnIndex)
    {
        var nodes = new List<NodeObject>();

        for (int row = headerRowIndex + 1; row < table.Rows.Count; ++row)
        {
            var node = ParseNode(table.Rows[row], parameterTypes, nameColumnIndex);
            nodes.Add(node);
        }

        return nodes;
    }

    private static NodeObject ParseNode(DataRow rowData, List<ParameterType> parameterTypes, int nameColumnIndex)
    {
        var name = rowData[nameColumnIndex]?.ToString()?.Trim();

        if (string.IsNullOrEmpty(name))
            throw new InvalidOperationException("The name column cannot contain an empty string");

        var node = new NodeObject
        {
            Name = name
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
