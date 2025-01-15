using VisualizationSystem.Models.Domain.Plots;
using VisualizationSystem.Services.Utilities.Plots;
using VisualizationSystem.UI.Components.TabPages;

namespace VisualizationSystem.Services.UI.TabPages;

public class ClusteredPlotTabStrategy : ITabStrategy
{
    private readonly PlotConfigurator plotConfigurator;

    public ClusteredPlotTabStrategy(PlotConfigurator plotConfigurator)
    {
        this.plotConfigurator = plotConfigurator;
    }

    public bool CanHandle(object content) => content is ClusteredPlot;

    public string GetTabId(object content)
    {
        if (content is not ClusteredPlot plot)
            throw new ArgumentException("Invalid content type, ClusteredPlot expected");

        return $"Clustered plot: {plot.Name}";
    }

    public ClosableTabPageBase CreateTabContent(object content)
    {
        if (content is not ClusteredPlot plot)
            throw new ArgumentException("Invalid content type, ClusteredPlot expected");

        return new ClosableClusteredPlotTabPage(GetTabId(plot), plot, plotConfigurator);
    }
}
