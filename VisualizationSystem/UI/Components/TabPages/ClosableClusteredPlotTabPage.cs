using ScottPlot.WinForms;
using VisualizationSystem.Models.Domain.Plots;
using VisualizationSystem.Services.Utilities.Plots;

namespace VisualizationSystem.UI.Components.TabPages;

public class ClosableClusteredPlotTabPage : ClosableTabPageBase
{
    private readonly FormsPlot formsPlot;
    private readonly PlotConfigurator plotConfigurator;

    public ClusteredPlot DisplayedPlot { get; set; }

    public ClosableClusteredPlotTabPage(
        string text,
        ClusteredPlot clusteredPlot,
        PlotConfigurator plotConfigurator
        )
        : base(text)
    {
        formsPlot = new FormsPlot();
        this.plotConfigurator = plotConfigurator;

        DisplayedPlot = clusteredPlot;
        InitializeContent();
    }

    protected override void InitializeContent()
    {
        formsPlot.Dock = DockStyle.Fill;
        Controls.Add(formsPlot);

        plotConfigurator.UpdatePlot(formsPlot, DisplayedPlot);
    }

    public override void UpdateContent(object newData)
    {
        if (newData is not ClusteredPlot clusteredPlot)
            throw new ArgumentException("Invalid input data");

        DisplayedPlot = clusteredPlot;
        plotConfigurator.UpdatePlot(formsPlot, clusteredPlot);
    }
}
