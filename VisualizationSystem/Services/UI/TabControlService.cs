using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.UI.Components;

namespace VisualizationSystem.Services.UI;

public class TabControlService
{
    private readonly TabControl tabControl;

    public TabControlService(TabControl tabControl)
    {
        this.tabControl = tabControl;
    }

    public void AddDataGridViewTabPage(NodeTable table)
    {
        var tabPage = new ClosableTabPage("Table: " + table.Name);
        var dataGridView = new NodeTableDataGridView(table)
        {
            Dock = DockStyle.Fill,
        };

        tabPage.Controls.Add(dataGridView);
        AddTab(tabPage);
    }

    public void AddGViewerTabPage(Graph graph, string tabName)
    {
        var tabPage = new ClosableTabPage("Graph: " + tabName);
        var gViewer = new GViewer
        {
            Dock = DockStyle.Fill,
            Graph = graph,
        };

        tabPage.Controls.Add(gViewer);
        AddTab(tabPage);
    }

    private void AddTab(ClosableTabPage tabPage)
    {
        tabControl.TabPages.Add(tabPage);
        tabControl.SelectedTab = tabPage;
    }
}
