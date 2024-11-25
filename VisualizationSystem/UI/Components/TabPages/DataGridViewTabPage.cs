using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.UI.Components.TabPages;
public sealed class ClosableDataGridViewTabPage : ClosableTabPageBase
{
    private NodeTableDataGridView nodeTableDataGridView;

    public NodeTable DisplayedNodeTable { get; set; }

    public ClosableDataGridViewTabPage(string text, NodeTable table)
        : base(text)
    {
        DisplayedNodeTable = table;

        InitializeContent();
    }

    protected override void InitializeContent()
    {
        nodeTableDataGridView = new NodeTableDataGridView(DisplayedNodeTable)
        {
            Dock = DockStyle.Fill,
        };

        Controls.Add(nodeTableDataGridView);
    }

    public override void UpdateContent(object newData)
    {
        if (newData is not NodeTable newNodeTable)
            throw new ArgumentException("Invalid input data");

        DisplayedNodeTable = newNodeTable;
        nodeTableDataGridView.UpdateTable(DisplayedNodeTable);
    }
}
