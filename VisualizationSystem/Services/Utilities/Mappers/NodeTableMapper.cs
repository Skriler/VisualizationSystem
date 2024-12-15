using System.Data;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Services.Utilities.Mappers;

public class NodeTableMapper
{
    public List<string> GetColumnHeaders(DataTable table)
    {
        var headersRowIndex = FindHeaderRowIndex(table);

        if (headersRowIndex < 0)
            throw new Exception("Table must contain a header row.");

        return GetRowValues(table.Rows[headersRowIndex]);
    }

    public NodeTable MapToNodeTable(DataTable table, int nameColumnIndex)
    {
        var headerRowIndex = FindHeaderRowIndex(table);

        if (headerRowIndex < 0)
            throw new Exception("Table must contain header row.");

        if (table.Rows.Count == 0)
            return new NodeTable();

        var parameterTypes = ParseParameterTypes(table, headerRowIndex);
        var nodes = ParseNodes(table, parameterTypes, headerRowIndex, nameColumnIndex);

        ValidateNodeNames(nodes);

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

    private List<string> GetRowValues(DataRow row)
    {
        return row.ItemArray
            .Select(cell => cell?.ToString() ?? string.Empty)
            .ToList();
    }

    private int FindHeaderRowIndex(DataTable table)
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

    private List<ParameterType> ParseParameterTypes(DataTable table, int headerRowIndex)
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

    private List<NodeObject> ParseNodes(DataTable table, List<ParameterType> parameterTypes, int headerRowIndex, int nameColumnIndex)
    {
        var nodes = new List<NodeObject>();

        for (int row = headerRowIndex + 1; row < table.Rows.Count; ++row)
        {
            var node = ParseNode(table.Rows[row], parameterTypes, nameColumnIndex);
            nodes.Add(node);
        }

        return nodes;
    }

    private NodeObject ParseNode(DataRow rowData, List<ParameterType> parameterTypes, int nameColumnIndex)
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

    private void ValidateNodeNames(List<NodeObject> nodes)
    {
        var duplicateNames = nodes
            .GroupBy(node => node.Name)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);

        if (duplicateNames.Any())
            throw new InvalidOperationException($"Duplicate names detected: {string.Join(", ", duplicateNames)}");
    }
}
