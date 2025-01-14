using VisualizationSystem.Models.Domain.PCA;

namespace VisualizationSystem.Models.Domain.Plots;

public class ClusteredPlot
{
    public string Name { get; }

    public List<ReducedDataPoint> Points { get; }

    public ClusteredPlot(string name, IEnumerable<ReducedDataPoint> points)
    {
        Name = name;
        Points = points.ToList();
    }
}
