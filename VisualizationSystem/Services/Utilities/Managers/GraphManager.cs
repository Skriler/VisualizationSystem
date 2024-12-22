using Microsoft.Msagl.Drawing;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Storages.Results;
using VisualizationSystem.Services.UI;
using VisualizationSystem.Services.Utilities.Comparers;
using VisualizationSystem.Services.Utilities.GraphBuilders;

namespace VisualizationSystem.Services.Utilities.Managers;

public class GraphManager
{
    private readonly IGraphBuilder<Graph> graphBuilder;
    private readonly GraphSaveManager graphSaveManager;
    private readonly NodeComparisonManager nodeComparisonManager;

    public Graph? Graph { get; private set; }

    public NodeTable? NodeTable { get; private set; }

    public GraphManager(
        IGraphBuilder<Graph> graphBuilder,
        GraphSaveManager graphSaveManager,
        NodeComparisonManager nodeComparisonManager
        )
    {
        this.graphBuilder = graphBuilder;
        this.graphSaveManager = graphSaveManager;
        this.nodeComparisonManager = nodeComparisonManager;
    }

    public Graph BuildGraph(NodeTable nodeTable)
    {
        NodeTable = nodeTable;

        var similarityResults = nodeComparisonManager.CalculateSimilarNodes(nodeTable.NodeObjects);
        Graph = graphBuilder.Build(nodeTable.Name, similarityResults);

        return Graph;
    }

    public async Task<Graph> BuildClusteredGraphAsync(NodeTable nodeTable)
    {
        NodeTable = nodeTable;

        var clusters = await nodeComparisonManager.CalculateClustersAsync(nodeTable);
        Graph = graphBuilder.Build(nodeTable.Name, nodeTable.NodeObjects, clusters);

        return Graph;
    }

    public async Task SaveGraphAsync(NodeTable nodeTable)
    {
        var similarityResults = nodeComparisonManager.CalculateSimilarNodes(nodeTable.NodeObjects);
        await graphSaveManager.SaveGraphAsync(nodeTable.Name, similarityResults);
    }

    public async Task SaveClusteredGraphAsync(NodeTable nodeTable)
    {
        var clusters = await nodeComparisonManager.CalculateClustersAsync(nodeTable);
        await graphSaveManager.SaveClusteredGraphAsync(nodeTable.Name, nodeTable.NodeObjects, clusters);
    }

    public void UpdateSettings(UserSettings userSettings)
    {
        nodeComparisonManager.UpdateSettings(userSettings);
        graphBuilder.UpdateSettings(userSettings);
        graphSaveManager.UpdateSettings(userSettings);
    }

    public bool TryGetNodeSimilarityResult(string nodeName, out NodeSimilarityResult nodeSimilarityResult)
    {
        return graphBuilder.NodeDataMap.TryGetValue(nodeName, out nodeSimilarityResult);
    }
}
