using System.Data;
using System.Text;
using ExcelDataReader;
using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.SL;

public static class ExcelReader
{
    private static readonly short RowHeadersIndex = 2;

    static ExcelReader()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static NodeTable ReadFile(string filePath)
    {
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var dataSet = reader.AsDataSet();
                return ParseDataSet(dataSet);
            }
        }
    }

    private static NodeTable ParseDataSet(DataSet dataSet, short tableIndex = 0)
    {
        var table = dataSet.Tables[tableIndex];

        if (table.Rows.Count == 0)
            return new NodeTable();

        var parameterTypes = InitializeParameterTypes(table, RowHeadersIndex);

        NodeTable nodeTable = new NodeTable
        {
            Name = table.TableName,
            ParameterTypes = parameterTypes
        };

        List<NodeObject> nodes = new List<NodeObject>();

        NodeObject node;
        for (int row = RowHeadersIndex + 1; row < table.Rows.Count; ++row)
        {
            node = ParseDataRow(table.Rows[row], parameterTypes);
            nodes.Add(node);
        }

        nodeTable.NodeObjects = nodes;

        return nodeTable;
    }

    private static List<ParameterType> InitializeParameterTypes(DataTable table, short rowHeadersIndex)
    {
        var parameterTypes = new List<ParameterType>();
        var rowHeaders = table.Rows[rowHeadersIndex];

        for (int col = 1; col < table.Columns.Count; ++col)
        {
            var parameterName = rowHeaders[col]?.ToString() ?? string.Empty;

            var parameterType = new ParameterType
            {
                Name = parameterName
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
