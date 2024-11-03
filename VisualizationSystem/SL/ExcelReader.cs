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

    public static List<NodeObject> ReadFile(string filePath)
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

    private static List<NodeObject> ParseDataSet(DataSet dataSet, short tableIndex = 0)
    {
        List<NodeObject> nodes = new List<NodeObject>();

        var table = dataSet.Tables[tableIndex];

        if (table.Rows.Count == 0)
            return nodes;

        var headers = GetHeaders(table, RowHeadersIndex);

        NodeObject node;
        for (int row = RowHeadersIndex + 1; row < table.Rows.Count; ++row)
        {
            node = ParseDataRow(table.Rows[row], headers);
            nodes.Add(node);
        }

        return nodes;
    }

    private static List<string> GetHeaders(DataTable table, short rowHeadersIndex)
    {
        var headers = new List<string>();
        var rowHeaders = table.Rows[rowHeadersIndex];

        for (int col = 1; col < table.Columns.Count; ++col)
        {
            headers.Add(rowHeaders[col]?.ToString() ?? string.Empty);
        }

        return headers;
    }

    private static NodeObject ParseDataRow(DataRow rowData, List<string> headers)
    {
        NodeObject node;

        node = new NodeObject()
        {
            Name = rowData[0].ToString()
        };

        var rowAmount = rowData.Table.Columns.Count;
        for (int col = 1; col < rowAmount; ++col)
        {
            var parameterName = headers[col - 1];
            var parameterValue = rowData[col]?.ToString() ?? string.Empty;

            node.Parameters.Add(new NodeParameter
            {
                Name = parameterName,
                Value = parameterValue
            });
        }

        return node;
    }
}
