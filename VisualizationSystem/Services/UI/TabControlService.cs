using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.UI.Components;
using VisualizationSystem.UI.Forms;

namespace VisualizationSystem.Services.UI;

public class TabControlService
{
    private readonly TabControl tabControl;
    private readonly Dictionary<string, TabPage> gViewerTabPageMap;
    private Action<string> OnNodeClick;

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

        OnNodeClick = onNodeClick;
        gViewer.MouseClick += GViewer_MouseClick;

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

    private void GViewer_MouseClick(object sender, MouseEventArgs e)
    {
        if (sender is not GViewer viewer)
            return;

        var clickedObject = viewer.GetObjectAt(e.Location);

        if (clickedObject is not DNode clickedNode)
            return;

        OnNodeClick?.Invoke(clickedNode.Node.Id);
    }
}
