using Microsoft.ML;
using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Domain.PCA;
using VisualizationSystem.Services.Utilities.Mappers;

namespace VisualizationSystem.Services.Utilities.DimensionReducers;

public class PCAReducer : IDimensionReducer
{
    private const int DefaultSeed = 12;
    private const int TargetDimension = 2;

    private readonly DataPointMapper dataPointMapper;
    private readonly MLContext mlContext;

    public PCAReducer(DataPointMapper dataPointMapper)
    {
        this.dataPointMapper = dataPointMapper;
        mlContext = new MLContext(DefaultSeed);
    }

    public List<ReducedDataPoint> ReduceDimension(IEnumerable<Cluster> cluster)
    {
        var dataPoints = dataPointMapper.MapList(cluster);

        var dataView = mlContext.Data.LoadFromEnumerable(dataPoints);
        var pipeline = mlContext.Transforms.ProjectToPrincipalComponents(
            outputColumnName: "PCAFeatures",
            inputColumnName: nameof(DataPoint.Features),
            rank: TargetDimension)
            .Append(mlContext.Transforms.CopyColumns("Name", "Name"))
            .Append(mlContext.Transforms.CopyColumns("ClusterId", "ClusterId"));

        var model = pipeline.Fit(dataView);
        var transformedData = model.Transform(dataView);

        return mlContext.Data
            .CreateEnumerable<ReducedDataPoint>(transformedData, reuseRowObject: false)
            .ToList();
    }
}
