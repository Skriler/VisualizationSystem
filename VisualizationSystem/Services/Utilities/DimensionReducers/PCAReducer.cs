using Microsoft.ML;
using Microsoft.ML.Data;
using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.Domain.Nodes.Parameters;
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

    public List<ReducedDataPoint> ReduceDimension(IEnumerable<Cluster> clusters)
    {
        var firstNode = GetValidNode(clusters);
        var featuresCount = firstNode.GetFeaturesCount();

        if (featuresCount == 0)
            throw new InvalidOperationException("No valid features found.");

        var schemaDef = CreateSchema(featuresCount);

        var dataPoints = dataPointMapper.MapList(clusters);
        var trainingDataView = mlContext.Data.LoadFromEnumerable(dataPoints, schemaDef);

        var pipeline = mlContext.Transforms.ProjectToPrincipalComponents(
            outputColumnName: "PCAFeatures",
            inputColumnName: nameof(DataPoint.Features),
            rank: TargetDimension)
            .Append(mlContext.Transforms.CopyColumns("Name", "Name"))
            .Append(mlContext.Transforms.CopyColumns("ClusterId", "ClusterId"));

        var model = pipeline.Fit(trainingDataView);
        var transformedData = model.Transform(trainingDataView);

        return mlContext.Data
            .CreateEnumerable<ReducedDataPoint>(transformedData, reuseRowObject: false)
            .ToList();
    }

    private CalculationNode GetValidNode(IEnumerable<Cluster> clusters)
    {
        var node = clusters
            .FirstOrDefault(c => c.Nodes.Count > 0)
            ?.Nodes.FirstOrDefault();

        if (node == null)
            throw new InvalidOperationException("No valid nodes found in clusters.");

        return node;
    }

    private SchemaDefinition CreateSchema(int featuresCount)
    {
        var schemaDef = SchemaDefinition.Create(typeof(DataPoint));
        schemaDef["Features"].ColumnType = new VectorDataViewType(NumberDataViewType.Single, featuresCount);
        return schemaDef;
    }
}
