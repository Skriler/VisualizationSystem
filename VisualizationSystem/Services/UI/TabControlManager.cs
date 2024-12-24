using VisualizationSystem.Models.Domain.Graphs;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.UI.Components.TabPages;

namespace VisualizationSystem.Services.UI;

public class TabControlManager
{
    private readonly TabControl tabControl;
    private readonly Dictionary<string, ClosableTabPageBase> tabPages;

    public TabControlManager(TabControl tabControl)
    {
        this.tabControl = tabControl;
        tabPages = new Dictionary<string, ClosableTabPageBase>();
    }

    private static string GetDataGridViewTabId(string tableName) => $"Dataset: {tableName}";

    private static string GetGViewerTabId(string tableName) => $"Graph: {tableName}";

    public void AddDataGridViewTabPage(NodeTable table)
    {
        var id = GetDataGridViewTabId(table.Name);

        if (TryUpdateTabPage(id, table))
            return;

        var tabPage = new ClosableDataGridViewTabPage(id, table);
        AddTabPage(id, tabPage);
    }

    public void AddGViewerTabPage(ExtendedGraph graph, string tableName)
    {
        var id = GetGViewerTabId(tableName);

        if (TryUpdateTabPage(id, graph))
            return;

        var tabPage = new ClosableGViewerTabPage(id, graph);
        AddTabPage(id, tabPage);
    }

    public void UpdateDataGridViewTabPageIfOpen(NodeTable table)
    {
        var id = GetDataGridViewTabId(table.Name);

        TryUpdateTabPage(id, table, false);
    }

    public void UpdateGViewerTabPageIfOpen(ExtendedGraph graph, string tableName)
    {
        var id = GetGViewerTabId(tableName);

        TryUpdateTabPage(id, graph, false);
    }

    public void RemoveTabPage(TabPage tabPage)
    {
        var tabId = tabPages
            .Where(kvp => kvp.Value == tabPage)
            .Select(kvp => kvp.Key)
            .FirstOrDefault();

        if (tabId == null)
            return;

        RemoveTabPage(tabPage, tabId);
    }

    public void RemoveTabPage(string tabId)
    {
        var tabPage = tabPages
            .Where(kvp => kvp.Key == tabId)
            .Select(kvp => kvp.Value)
            .FirstOrDefault();

        if (tabPage == null)
            return;

        RemoveTabPage(tabPage, tabId);
    }

    private void RemoveTabPage(TabPage tabPage, string tabId)
    {
        tabControl.TabPages.Remove(tabPage);
        tabPages.Remove(tabId);
    }

    public void RemoveRelatedTabPages(string tableName)
    {
        var dataGridViewTabId = GetDataGridViewTabId(tableName);
        var gViewerTabId = GetGViewerTabId(tableName);

        RemoveTabPage(dataGridViewTabId);
        RemoveTabPage(gViewerTabId);
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
