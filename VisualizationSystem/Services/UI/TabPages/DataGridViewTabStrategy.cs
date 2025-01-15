using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.UI.Components.TabPages;

namespace VisualizationSystem.Services.UI.TabPages;

public class DataGridViewTabStrategy : ITabStrategy
{
    public bool CanHandle(object content) => content is NodeTable;

    public string GetTabId(object content)
    {
        if (content is not NodeTable table)
            throw new ArgumentException("Invalid content type, NodeTable expected");

        return $"Dataset: {table.Name}";
    }

    public ClosableTabPageBase CreateTabContent(object content)
    {
        if (content is not NodeTable table)
            throw new ArgumentException("Invalid content type, NodeTable expected");

        return new ClosableDataGridViewTabPage(GetTabId(table), table);
    }
}
