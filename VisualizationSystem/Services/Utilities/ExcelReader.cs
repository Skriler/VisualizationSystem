using System.Data;
using System.Text;
using ExcelDataReader;
using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Services.Utilities;

public static class ExcelParser
{
    static ExcelParser()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static List<DataTable> GetExcelTables(string filePath)
    {
        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataSet = reader.AsDataSet();

        return dataSet.Tables
            .Cast<DataTable>()
            .ToList();
    }

    public static List<string> GetColumnHeaders(DataTable table)
    {
        var headersRowIndex = FindHeaderRowIndex(table);

        if (headersRowIndex < 0)
            throw new Exception("Table must contain a header row.");

        return GetRowValues(table.Rows[headersRowIndex]);
    }

    public static NodeTable ParseTable(DataTable table, int nameColumnIndex)
    {
        var headerRowIndex = FindHeaderRowIndex(table);

        if (headerRowIndex < 0)
            throw new Exception("Table must contain header row.");

        return ParseNodeTable(table, headerRowIndex, nameColumnIndex);
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

        var parameterTypes = ParseParameterTypes(table, headerRowIndex, nameColumnIndex);
        var nodes = ParseNodes(table, parameterTypes, headerRowIndex, nameColumnIndex);

        var duplicateNames = nodes
            .GroupBy(node => node.Name)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicateNames.Any())
            throw new InvalidOperationException($"Name duplication: {string.Join(", ", duplicateNames)}");

        var parameterTypesWithoutName = parameterTypes
            .Where((value, index) => index != nameColumnIndex)
            .ToList();

        var nodeTable = new NodeTable
        {
            Name = table.TableName,
            ParameterTypes = parameterTypesWithoutName,
            NodeObjects = nodes,
        };

        return nodeTable;
    }

    private static List<ParameterType> ParseParameterTypes(DataTable table, int headerRowIndex, int nameColumnIndex)
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
        var name = rowData[nameColumnIndex].ToString()?.Trim();

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

            node.Parameters.Add(new NodeParameter
            {
                Value = rowData[col].ToString() ?? string.Empty,
                ParameterType = parameterTypes[col]
            });
        }

        return node;
    }
}
