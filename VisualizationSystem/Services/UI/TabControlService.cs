using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.Windows.Automation;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.UI.Components;

namespace VisualizationSystem.Services.UI;

public class TabControlService
{
    private readonly TabControl tabControl;
    private readonly List<TabPageInfo> tabPages;

    public TabControlService(TabControl tabControl)
    {
        this.tabControl = tabControl;
        tabPages = new List<TabPageInfo>();
    }

    public void AddOrUpdateDataGridViewTabPage(NodeTable table, string tabName)
    {
        if (TryUpdateExistingTabPage(
                tabName, 
                TabControlType.DataGridView, 
                tabPage => UpdateDataGridViewTab(tabPage, table))
            )
            return;

        var tabPage = new ClosableTabPage($"Table: {table.Name}");
        var dataGridView = new NodeTableDataGridView(table)
        {
            Dock = DockStyle.Fill,
        };

        tabPage.Controls.Add(dataGridView);
        AddTabPage(tabPage);

        tabPages.Add(new TabPageInfo(tabPage, TabControlType.DataGridView, tabName));
    }

    public void AddOrUpdateGViewerTabPage(Graph graph, string tabName, Action<string> onNodeClick)
    {
        if (TryUpdateExistingTabPage(
                tabName, 
                TabControlType.GViewer, 
                tabPage => UpdateGViewerTab(tabPage, graph))
            )
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

        tabPages.Add(new TabPageInfo(tabPage, TabControlType.GViewer, tabName));
    }

    private void AddTabPage(ClosableTabPage tabPage)
    {
        tabControl.TabPages.Add(tabPage);
        tabControl.SelectedTab = tabPage;
    }

    private bool TryUpdateExistingTabPage(string tabName, TabControlType controlType, Action<TabPage> updateAction)
    {
        var existingTabInfo = tabPages
            .FirstOrDefault(t => t.ControlType == controlType && t.Name == tabName);

        if (existingTabInfo == null)
            return false;

        updateAction(existingTabInfo.Page);
        tabControl.SelectedTab = existingTabInfo.Page;

        return true;
    }

    private void UpdateDataGridViewTab(TabPage tabPage, NodeTable table)
    {
        var dataGridView = tabPage.Controls.OfType<DataGridView>().FirstOrDefault();

        if (dataGridView == null)
            return;

        dataGridView.DataSource = table;
        dataGridView.Refresh();
    }

    private void UpdateGViewerTab(TabPage tabPage, Graph graph)
    {
        var gViewer = tabPage.Controls.OfType<GViewer>().FirstOrDefault();

        if (gViewer == null)
            return;

        gViewer.Graph = graph;
        gViewer.Refresh();
    }

    public void RemoveTabPage(TabPage tabPage)
    {
        var tabInfo = tabPages.FirstOrDefault(t => t.Page == tabPage);

        if (tabInfo == null)
            return;

        tabControl.TabPages.Remove(tabPage);
        tabPages.Remove(tabInfo);
    }

    public bool IsTabPageOpen(string tabName)
    {
        return tabPages.Any(t => t.Name == tabName);
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
