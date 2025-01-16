namespace VisualizationSystem.Services.Utilities.DistanceCalculators.NumericMetrics;

public interface INumericDistanceMetric
{
    double CalculateDistance(List<double> x, List<double> y);
}
