using ScottPlot;
using ScottPlot.WinForms;
using VisualizationSystem.Models.Domain.Plots;
using ScottColor = ScottPlot.Color;
using SystemColor = System.Drawing.Color;

namespace VisualizationSystem.UI.Components.TabPages;

public class ClosableClusteredPlotTabPage : ClosableTabPageBase
{
    private readonly FormsPlot formsPlot;
    //private Plot plot;

    public ClusteredPlot DisplayedPlot { get; set; }

    public ClosableClusteredPlotTabPage(string text, ClusteredPlot clusteredPlot)
        : base(text)
    {
        formsPlot = new FormsPlot();
        DisplayedPlot = clusteredPlot;

        //plot = formsPlot.Plot;

        InitializeContent();
    }

    protected override void InitializeContent()
    {
        formsPlot.Dock = DockStyle.Fill;

        formsPlot.Plot.Title(DisplayedPlot.Name);
        //plot.XLabel("First Principal Component");
        //plot.YLabel("Second Principal Component");

        Controls.Add(formsPlot);

        UpdatePlot(DisplayedPlot);
    }

    public override void UpdateContent(object newData)
    {
        if (newData is not ClusteredPlot clusteredPlot)
            throw new ArgumentException("Invalid input data");

        UpdatePlot(clusteredPlot);
    }

    private void UpdatePlot(ClusteredPlot clusteredPlot)
    {
        formsPlot.Plot.Clear();

        var xs = clusteredPlot.Points.Select(dp => dp.X).ToArray();
        var ys = clusteredPlot.Points.Select(dp => dp.Y).ToArray();

        var scatter = formsPlot.Plot.Add.Scatter(xs, ys);
        scatter.Color = Colors.Navy;
        scatter.MarkerStyle.Size = 8;
        scatter.LineStyle.Width = 0;

        for (int i = 0; i < xs.Length; i++)
        {
            var label = formsPlot.Plot.Add.Text(
                text: clusteredPlot.Points[i].Name,
                x: xs[i],
                y: ys[i]
            );
            label.Alignment = Alignment.MiddleCenter;
            label.OffsetY = -10;
            label.LabelFontSize = 8;
        }

        formsPlot.Refresh();
    }
}
