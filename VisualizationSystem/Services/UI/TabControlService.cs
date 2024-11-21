using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.UI.Components;

namespace VisualizationSystem.Services.UI;

public class TabControlService
{
    private readonly TabControl tabControl;
    private readonly Dictionary<string, TabPage> gViewerTabPageMap;

    public TabControlService(TabControl tabControl)
    {
        this.tabControl = tabControl;
        gViewerTabPageMap = new Dictionary<string, TabPage>();
    }

    public void AddDataGridViewTabPage(NodeTable table)
    {
        var tabPage = new ClosableTabPage($"Table: {table.Name}");
        var dataGridView = new NodeTableDataGridView(table)
        {
            Dock = DockStyle.Fill,
        };

        tabPage.Controls.Add(dataGridView);
        AddTabPage(tabPage);
    }

    public void AddOrUpdateGViewerTabPage(Graph graph, string tabName, Action<string> onNodeClick)
    {
        if (TryUpdateExistingTabPage(graph, tabName))
            return;

        var tabPage = new ClosableTabPage($"Graph: {tabName}");
        var gViewer = new GViewer
        {
            Dock = DockStyle.Fill,
            Graph = graph,
        };

        gViewer.MouseClick += (sender, e) => HandleNodeClick(sender, e, onNodeClick);

        tabPage.Controls.Add(gViewer);
        AddTabPage(tabPage);

        gViewerTabPageMap[tabName] = tabPage;
    }

    private void AddTabPage(ClosableTabPage tabPage)
    {
        tabControl.TabPages.Add(tabPage);
        tabControl.SelectedTab = tabPage;
    }

    private bool TryUpdateExistingTabPage(Graph graph, string tabName)
    {
        if (!gViewerTabPageMap.TryGetValue(tabName, out var existingTabPage))
            return false;

        var gViewer = existingTabPage.Controls.OfType<GViewer>().FirstOrDefault();

        if (gViewer == null)
            return false;

        gViewer.Graph = graph;
        gViewer.Refresh();

        return true;
    }

    public void RemoveTabPage(string tabName)
    {
        if (!gViewerTabPageMap.TryGetValue(tabName, out var tabPage))
            return;

        tabControl.TabPages.Remove(tabPage);
        gViewerTabPageMap.Remove(tabName);
    }

    public bool IsTabPageOpen(string tabName)
    {
        return gViewerTabPageMap.ContainsKey(tabName);
    }

    private void HandleNodeClick(object sender, MouseEventArgs e, Action<string> onNodeClick)
    {
        if (sender is not GViewer viewer)
            return;

        var clickedObject = viewer.GetObjectAt(e.Location);

        if (clickedObject is not DNode clickedNode)
            return;

        onNodeClick?.Invoke(clickedNode.Node.Id);
    }
}
