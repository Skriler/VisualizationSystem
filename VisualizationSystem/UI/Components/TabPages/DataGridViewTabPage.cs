using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.UI.Components.TabPages;
public sealed class ClosableDataGridViewTabPage : ClosableTabPageBase
{
    private readonly NodeTableDataGridView nodeTableDataGridView;

    public ClosableDataGridViewTabPage(string text, NodeTable table)
        : base(text)
    {
        nodeTableDataGridView = new NodeTableDataGridView(table);
        
        InitializeContent();
    }

    protected override void InitializeContent()
    {
        nodeTableDataGridView.Dock = DockStyle.Fill;

        Controls.Add(nodeTableDataGridView);
    }

    public override void UpdateContent(object newData)
    {
        if (newData is not NodeTable newNodeTable)
            throw new ArgumentException("Invalid input data");

        nodeTableDataGridView.UpdateTable(newNodeTable);
    }
}
