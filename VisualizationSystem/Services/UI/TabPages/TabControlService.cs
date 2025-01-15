using VisualizationSystem.UI.Components.TabPages;

namespace VisualizationSystem.Services.UI.TabPages;

public class TabControlService
{
    private readonly IEnumerable<ITabStrategy> tabStrategies;
    private readonly Dictionary<string, ClosableTabPageBase> tabPages;

    public TabControl TabControl { get; private set; } = default!;

    public TabControlService(IEnumerable<ITabStrategy> tabStrategies)
    {
        this.tabStrategies = tabStrategies;
        tabPages = new Dictionary<string, ClosableTabPageBase>();
    }

    public void Initialize(TabControl tabControl) => TabControl = tabControl;

    public void AddTab(object content)
    {
        if (TabControl == null)
            throw new InvalidOperationException("TabControl not initialized");

        var strategy = GetStrategy(content);
        var tabId = strategy.GetTabId(content);

        if (TryUpdateTab(tabId, content))
            return;

        var tabPage = strategy.CreateTabContent(content);
        AddNewTab(tabId, tabPage);
    }

    public void UpdateTabIfOpen(object content, bool setActive = true)
    {
        var strategy = GetStrategy(content);
        var tabId = strategy.GetTabId(content);

        TryUpdateTab(tabId, content, setActive);
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

    public void RemoveRelatedTabPages(string contentName)
    {
        var relatedPages = tabPages
            .Where(kvp => kvp.Key.Contains(contentName))
            .ToList();

        if (relatedPages == null || relatedPages.Count == 0)
            return;

        relatedPages.ForEach(g => RemoveTabPage(g.Value, g.Key));
    }

    private bool TryUpdateTab(string tabId, object newData, bool setActive = true)
    {
        if (!tabPages.TryGetValue(tabId, out var existingTab))
            return false;

        existingTab.UpdateContent(newData);
        TabControl.SelectedTab = setActive ? existingTab : TabControl.SelectedTab;
        return true;
    }

    private void AddNewTab(string tabId, ClosableTabPageBase tabPage)
    {
        TabControl.TabPages.Add(tabPage);
        tabPages.Add(tabId, tabPage);
        TabControl.SelectedTab = tabPage;
    }

    private void RemoveTabPage(TabPage tabPage, string tabId)
    {
        TabControl.TabPages.Remove(tabPage);
        tabPages.Remove(tabId);
    }

    private ITabStrategy GetStrategy(object content)
    {
        var strategy = tabStrategies.FirstOrDefault(s => s.CanHandle(content));

        if (strategy == null)
            throw new ArgumentException("No strategy found for content type");

        return strategy;
    }
}
