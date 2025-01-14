using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.Domain.Nodes.Parameters;
using VisualizationSystem.Models.Domain.PCA;

namespace VisualizationSystem.Services.Utilities.Mappers;

public class DataPointMapper
{
    public List<DataPoint> MapList(IEnumerable<Cluster> clusters)
    {
        var dataPoints = new List<DataPoint>();

        foreach (var cluster in clusters)
        {
            var clusterDataPoints = cluster.Nodes
                .Select(node => CreateDataPoint(node, cluster.Id));

            dataPoints.AddRange(clusterDataPoints);
        }

        return dataPoints;
    }

    private DataPoint CreateDataPoint(CalculationNode node, int clusterId)
    {
        var features = node.Parameters
            .SelectMany(ParseParameter)
            .ToArray();

        return new DataPoint
        {
            Features = features,
            Name = node.Name,
            ClusterId = clusterId,
        };
    }

    private IEnumerable<float> ParseParameter(BaseParameter parameter) => parameter switch
    {
        CategoricalParameter categorical => ParseCategoricalParameter(categorical),
        NumericParameter numeric => new[] { ParseNumericParameter(numeric) },
        _ => throw new ArgumentException($"Unsupported parameter type: {parameter.GetType()}", nameof(parameter))
    };

    private float ParseNumericParameter(NumericParameter parameter) => (float)parameter.Value;

    private float[] ParseCategoricalParameter(CategoricalParameter parameter)
    {
        var features = new float[parameter.CategoryCount];

        foreach (var index in parameter.OneHotIndexes)
        {
            features[index] = 1;
        }

        return features;
    }
}