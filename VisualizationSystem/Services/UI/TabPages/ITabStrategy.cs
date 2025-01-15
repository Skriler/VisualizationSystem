using VisualizationSystem.UI.Components.TabPages;

namespace VisualizationSystem.Services.UI.TabPages;

public interface ITabStrategy
{
    string GetTabId(object content);

    bool CanHandle(object content);

    ClosableTabPageBase CreateTabContent(object content);
}
