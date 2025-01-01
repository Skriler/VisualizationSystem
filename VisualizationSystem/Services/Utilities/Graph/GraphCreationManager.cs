using VisualizationSystem.Models.Domain.Graphs;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.Comparers;
using VisualizationSystem.Services.Utilities.Graph.Builders;

namespace VisualizationSystem.Services.Utilities.Graph;

public class GraphCreationManager
{
    private readonly IGraphBuilder<ExtendedGraph> graphBuilder;
    private readonly NodeComparisonManager nodeComparisonManager;

    public GraphCreationManager(
        IGraphBuilder<ExtendedGraph> graphBuilder,
        NodeComparisonManager nodeComparisonManager
        )
    {
        this.graphBuilder = graphBuilder;
        this.nodeComparisonManager = nodeComparisonManager;
    }

    public ExtendedGraph BuildGraph(NodeTable nodeTable)
    {
        var similarityResults = nodeComparisonManager.CalculateSimilarNodes(nodeTable.NodeObjects);
        return graphBuilder.Build(nodeTable.Name, similarityResults);
    }

    public async Task<ExtendedGraph> BuildClusteredGraphAsync(NodeTable nodeTable)
    {
        var clusters = await nodeComparisonManager.CalculateClustersAsync(nodeTable);
        return graphBuilder.Build(nodeTable.Name, nodeTable.NodeObjects, clusters);
    }
}
