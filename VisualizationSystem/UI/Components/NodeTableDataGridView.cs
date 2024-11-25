using System.Data;
using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.UI.Components;

public class NodeTableDataGridView : DataGridView
{
    public NodeTable NodeTable { get; private set; }

    public NodeTableDataGridView(NodeTable nodeTable)
    {
        NodeTable = nodeTable;
        InitializeTable();

        SetReadOnly();
    }

    public void UpdateTable(NodeTable nodeTable)
    {
        NodeTable = nodeTable;
        InitializeTable();
    }

    private void InitializeTable()
    {
        var dataTable = new DataTable();

        AddColumns(dataTable);
        FillRowsWithData(dataTable);

        DataSource = dataTable;
    }

    private void AddColumns(DataTable dataTable)
    {
        dataTable.Columns.Add("Name");

        var parameterNames = NodeTable.ParameterTypes
            .Where(p => !string.IsNullOrEmpty(p.Name))
            .Select(p => p.Name)
            .ToList();

        foreach (var parameterName in parameterNames)
        {
            dataTable.Columns.Add(parameterName);
        }
    }

    private void FillRowsWithData(DataTable dataTable)
    {
        foreach (var node in NodeTable.NodeObjects)
        {
            var row = dataTable.NewRow();
            row["Name"] = node.Name ?? string.Empty;

            foreach (var parameter in node.Parameters)
            {
                row[parameter.ParameterType.Name] = parameter.Value ?? string.Empty;
            }

            dataTable.Rows.Add(row);
        }
    }

    private void SetReadOnly()
    {
        ReadOnly = true;
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        AllowUserToResizeRows = false;
        AllowUserToResizeColumns = false;
        AllowUserToOrderColumns = false;
    }
}
