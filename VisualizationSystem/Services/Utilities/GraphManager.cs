using Microsoft.Msagl.Drawing;
using System.Xml;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Storages.Results;
using VisualizationSystem.Services.UI;
using VisualizationSystem.Services.Utilities.Comparers;
using VisualizationSystem.Services.Utilities.GraphBuilders;

namespace VisualizationSystem.Services.Utilities;

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

        nodeComparisonManager.CalculateSimilarNodes(nodeTable.NodeObjects);
        Graph = graphBuilder.Build(nodeTable.Name, nodeComparisonManager.SimilarityResults);

        return Graph;
    }

    public Graph BuildClusteredGraph(NodeTable nodeTable)
    {
        NodeTable = nodeTable;

        nodeComparisonManager.CalculateClusters(nodeTable.NodeObjects);
        Graph = graphBuilder.Build(nodeTable.Name, nodeTable.NodeObjects, nodeComparisonManager.Clusters);

        return Graph;
    }

    public async Task SaveGraphAsync(NodeTable nodeTable)
    {
        await graphSaveManager.SaveGraphAsync(nodeTable.Name, nodeComparisonManager.SimilarityResults);
    }

    public async Task SaveClusteredGraphAsync(NodeTable nodeTable)
    {
        await graphSaveManager.SaveClusteredGraphAsync(nodeTable.Name, nodeTable.NodeObjects, nodeComparisonManager.Clusters);
    }

    public void UpdateSettings(UserSettings userSettings)
    {
        nodeComparisonManager.UpdateSettings(userSettings);
        graphBuilder.UpdateSettings(userSettings);
        graphSaveManager.UpdateSettings(userSettings);

        if (Graph != null)
        {
            Graph = userSettings.UseClustering ?
                BuildClusteredGraph(NodeTable) :
                BuildGraph(NodeTable);
        }
    }

    public bool TryGetNodeSimilarityResult(string nodeName, out NodeSimilarityResult nodeSimilarityResult)
    {
        return graphBuilder.NodeDataMap.TryGetValue(nodeName, out nodeSimilarityResult);
    }
}
