using DotNetGraph.Core;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.Utilities.Comparers;
using VisualizationSystem.Services.Utilities.FileSystem;
using VisualizationSystem.Services.Utilities.Graph.Builders;

namespace VisualizationSystem.Services.Utilities.Graph;

public class GraphSaveManager
{
    private const string GraphName = "graph.dot";

    private readonly IGraphBuilder<DotGraph> graphBuilder;
    private readonly NodeComparisonManager nodeComparisonManager;
    private readonly FileWriter fileWriter;

    public DotGraph? Graph { get; private set; }

    public GraphSaveManager(IGraphBuilder<DotGraph> graphBuilder, NodeComparisonManager nodeComparisonManager, FileWriter fileWriter)
    {
        this.graphBuilder = graphBuilder;
        this.nodeComparisonManager = nodeComparisonManager;
        this.fileWriter = fileWriter;
    }

    public void UpdateSettings(UserSettings settings)
    {
        nodeComparisonManager.UpdateSettings(settings);
        graphBuilder.UpdateSettings(settings);
    }

    public async Task SaveGraphAsync(NodeTable nodeTable)
    {
        var similarityResults = nodeComparisonManager.CalculateSimilarNodes(nodeTable.NodeObjects);
        var graph = graphBuilder.Build(nodeTable.Name, similarityResults);

        await fileWriter.WriteGraphAsync(graph, GraphName);
    }

    public async Task SaveClusteredGraphAsync(NodeTable nodeTable)
    {
        var clusters = await nodeComparisonManager.CalculateClustersAsync(nodeTable);
        var graph = graphBuilder.Build(nodeTable.Name, nodeTable.NodeObjects, clusters);

        await fileWriter.WriteGraphAsync(graph, GraphName);
    }
}
