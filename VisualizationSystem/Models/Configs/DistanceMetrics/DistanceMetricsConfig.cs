using VisualizationSystem.Services.Utilities.DistanceCalculators.CategoricalMetrics;
using VisualizationSystem.Services.Utilities.DistanceCalculators.NumericMetrics;

namespace VisualizationSystem.Models.Configs.DistanceMetrics;

public class DistanceMetricsConfig
{
    public INumericDistanceMetric NumericDistanceMetric { get; private set; }
    public ICategoricalDistanceMetric CategoricalDistanceMetric { get; private set; }

    public DistanceMetricsConfig(INumericDistanceMetric numericDistanceMetric, ICategoricalDistanceMetric categoricalDistanceMetric)
    {
        NumericDistanceMetric = numericDistanceMetric;
        CategoricalDistanceMetric = categoricalDistanceMetric;
    }
}
