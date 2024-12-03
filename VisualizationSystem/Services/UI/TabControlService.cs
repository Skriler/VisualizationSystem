using DotNetGraph.Core;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Msagl.Drawing;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.UI.Components.TabPages;

namespace VisualizationSystem.Services.UI;

public class TabControlService
{
    private readonly TabControl tabControl;
    private readonly Dictionary<string, ClosableTabPageBase> tabPages;

    public TabControlService(TabControl tabControl)
    {
        this.tabControl = tabControl;
        tabPages = new Dictionary<string, ClosableTabPageBase>();
    }

    private static string GetDataGridViewTabId(string tableName) => $"Table: {tableName}";

    private static string GetGViewerTabId(string tableName) => $"Graph: {tableName}";

    public void AddDataGridViewTabPage(NodeTable table)
    {
        var id = GetDataGridViewTabId(table.Name);

        if (TryUpdateTabPage(id, table))
            return;

        var tabPage = new ClosableDataGridViewTabPage(id, table);
        AddTabPage(id, tabPage);
    }

    public void AddGViewerTabPage(Graph graph, string tableName, Action<string> onNodeClick)
    {
        var id = GetGViewerTabId(tableName);

        if (TryUpdateTabPage(id, graph))
            return;

        var tabPage = new ClosableGViewerTabPage(id, graph, onNodeClick);
        AddTabPage(id, tabPage);
    }

    public void UpdateDataGridViewTabPageIfOpen(NodeTable table)
    {
        var id = GetDataGridViewTabId(table.Name);

        TryUpdateTabPage(id, table, false);
    }

    public void UpdateGViewerTabPageIfOpen(Graph graph, string tableName)
    {
        var id = GetGViewerTabId(tableName);

        TryUpdateTabPage(id, graph, false);
    }

    public void RemoveTabPage(TabPage tabPage)
    {
        var tabPageId = tabPages
            .Where(kvp => kvp.Value == tabPage)
            .Select(kvp => kvp.Key)
            .FirstOrDefault();

        if (tabPageId == null)
            return;

        tabControl.TabPages.Remove(tabPage);
        tabPages.Remove(tabPageId);
    }

    private void AddTabPage(string tabId, ClosableTabPageBase tabPage)
    {
        tabControl.TabPages.Add(tabPage);
        tabPages.Add(tabId, tabPage);
        tabControl.SelectedTab = tabPage;
    }

    private bool TryUpdateTabPage(string tabId, object newData, bool setActive = true)
    {
        if (!tabPages.TryGetValue(tabId, out var existingTabPage))
            return false;

        existingTabPage.UpdateContent(newData);

        if (setActive)
            tabControl.SelectedTab = existingTabPage;

        return true;
    }
}
