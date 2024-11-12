using System.Data;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.ViewModels;

namespace VisualizationSystem.UI.Components;

public class NodeTableDataGridView : DataGridView
{
    public NodeTableDataGridView(NodeTable table)
    {
        InitializeWithNodeTable(table);
    }

    private void InitializeWithNodeTable(NodeTable table)
    {
        var headers =
            table
            .ParameterTypes
            .Select(p => p.Name)
            .Where(name => name != null)
            .Cast<string>();

        NodeObjectViewModel.InitializeHeaders(headers);

        var nodeViewModels = table.NodeObjects
            .Select(n => new NodeObjectViewModel(n))
            .ToList();

        DataSource = CreateDataTable(nodeViewModels);

        DisableUserInteractions();
    }

    private DataTable CreateDataTable(List<NodeObjectViewModel> nodeViewModels)
    {
        var dataTable = new DataTable();

        foreach (var header in NodeObjectViewModel.Headers)
        {
            dataTable.Columns.Add(header);
        }

        DataRow row;
        foreach (var node in nodeViewModels)
        {
            row = dataTable.NewRow();

            for (int i = 0; i < node.Parameters.Count; ++i)
            {
                row[i] = node.Parameters[i];
            }

            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    private void DisableUserInteractions()
    {
        ReadOnly = true;
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        AllowUserToResizeRows = false;
        AllowUserToResizeColumns = false;
        AllowUserToOrderColumns = false;
    }
}
