using VisualizationSystem.Models.Domain.Graphs;
using VisualizationSystem.UI.Components.TabPages;

namespace VisualizationSystem.Services.UI.TabPages;

public class GViewerTabStrategy : ITabStrategy
{
    public bool CanHandle(object content) => content is ExtendedGraph;

    public string GetTabId(object content)
    {
        if (content is not ExtendedGraph graph)
            throw new ArgumentException("Invalid content type, ExtendedGraph expected");

        return $"{graph.Type}: {graph.Label.Text}";
    }

    public ClosableTabPageBase CreateTabContent(object content)
    {
        if (content is not ExtendedGraph graph)
            throw new ArgumentException("Invalid content type, ExtendedGraph expected");

        return new ClosableGViewerTabPage(GetTabId(graph), graph);
    }
}
