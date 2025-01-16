using VisualizationSystem.Models.Configs.DistanceMetrics;
using VisualizationSystem.Models.Domain.Nodes;

namespace VisualizationSystem.Services.Utilities.DistanceCalculators;

public interface IDistanceCalculator
{
    double Calculate(CalculationNode firstNode, CalculationNode secondNode);

    void Initialize(DistanceMetricsConfig config);
}
