using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WinForms;
using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Domain.PCA;
using VisualizationSystem.Models.Domain.Plots;
using VisualizationSystem.Services.Utilities.Helpers.Colors;
using ScottColor = ScottPlot.Color;
using SystemColor = System.Drawing.Color;

namespace VisualizationSystem.Services.Utilities.Plots;

public class PlotConfigurator
{
    private const int DefaultMarkerSize = 5;
    private const int DefaultLineWidth = 0;
    private const int DefaultLabelFontSize = 8;
    private const float DefaultLabelOffsetY = -10;

    private readonly ColorHelper colorHelper;

    private Plot plot = default!;

    public PlotConfigurator(ColorHelper colorHelper)
    {
        this.colorHelper = colorHelper;
    }

    public void UpdatePlot(FormsPlot formsPlot, ClusteredPlot clusteredPlot)
    {
        plot = formsPlot.Plot;

        ConfigurePlot(clusteredPlot.Name);

        var clusterIds = clusteredPlot.Points
            .Select(p => p.ClusterId)
            .Distinct()
            .Order()
            .ToList();

        var colors = colorHelper.GetDistinctColors(clusterIds, ColorBrightness.All);

        AddPointsToPlot(clusteredPlot.Points, colors);

        formsPlot.Plot.Legend.IsVisible = false;
        formsPlot.Refresh();
    }

    private void ConfigurePlot(string title)
    {
        plot.Clear();
        plot.Title(title);
        plot.XLabel("");
        plot.YLabel("");
        plot.Axes.Bottom.IsVisible = false;
        plot.Axes.Right.IsVisible = false;
        plot.Axes.Left.IsVisible = false;
    }

    private void AddPointsToPlot(List<ReducedDataPoint> points, Dictionary<int, SystemColor> colorMap)
    {
        foreach (var point in points)
        {
            var color = point.ClusterId >= 0
                ? colorMap[point.ClusterId]
                : SystemColor.Gray;

            var scatter = plot.Add.Scatter(
                new[] { (double)point.X },
                new[] { (double)point.Y }
            );
            ConfigureScatter(scatter, point.Name, color);

            var label = plot.Add.Text(point.Name, point.X, point.Y);
            ConfigureLabel(label, point.Name, point.X, point.Y, color);
        }
    }

    private static void ConfigureScatter(Scatter scatter, string legendText, SystemColor color)
    {
        scatter.Color = ScottColor.FromColor(color);
        scatter.MarkerStyle.Size = DefaultMarkerSize;
        scatter.LineStyle.Width = DefaultLineWidth;
        scatter.LegendText = legendText;
    }

    private static void ConfigureLabel(Text label, string text, double x, double y, SystemColor color)
    {
        label.Alignment = Alignment.MiddleCenter;
        label.OffsetY = DefaultLabelOffsetY;
        label.LabelFontSize = DefaultLabelFontSize;
        label.LabelFontColor = ScottColor.FromColor(color);
    }
}
