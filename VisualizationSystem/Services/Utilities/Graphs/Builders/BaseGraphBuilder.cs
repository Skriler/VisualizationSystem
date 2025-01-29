using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Domain.Graphs;
using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.Utilities.Helpers.Colors;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Graphs.Builders;

public abstract class BaseGraphBuilder<TGraph> : IGraphBuilder<TGraph>, ISettingsObserver
{
    protected readonly ColorHelper colorHelper;

    protected UserSettings settings = default!;

    protected BaseGraphBuilder(
        ColorHelper colorHelper,
        ISettingsSubject settingsSubject
        )
    {
        this.colorHelper = colorHelper;

        settingsSubject.Attach(this);
    }

    public abstract TGraph Build(TableAnalysisResult analysisResult);

    protected abstract TGraph BuildNormalGraph(TableAnalysisResult analysisResult);

    protected abstract TGraph BuildClusters(TableAnalysisResult analysisResult);

    protected abstract TGraph BuildClusteredGraph(TableAnalysisResult analysisResult);

    public void Update(UserSettings settings) => this.settings = settings;

    protected virtual void AddNodes(TGraph graph, TableAnalysisResult analysisResult)
    {
        var nodeColors = settings.UseClustering
            ? GetNodeColorsBasedOnClusters(analysisResult.Nodes, analysisResult.Clusters)
            : GetNodeColorsBasedOnSimilarity(analysisResult.SimilarityResults);

        foreach (var node in analysisResult.Nodes)
        {
            var currentNodeName = node.Name;

            AddNode(
                graph,
                currentNodeName,
                nodeColors[node]
                );
        }
    }

    protected Dictionary<NodeObject, Color> GetNodeColorsBasedOnSimilarity(List<NodeSimilarityResult> similarityResults)
    {
        var nodeColors = new Dictionary<NodeObject, Color>();
        var maxSimilarNodesAboveThreshold = similarityResults
            .Max(result => result.SimilarNodes
                .Count(sn => sn.SimilarityPercentage > settings.MinSimilarityPercentage)
            );

        foreach (var similarityResult in similarityResults)
        {
            var nodeColor = colorHelper.GetNodeColorFromDensityWithSigmoid(
                similarityResult,
                maxSimilarNodesAboveThreshold,
                settings.MinSimilarityPercentage
                );

            nodeColors.Add(similarityResult.Node, nodeColor);
        }

        return nodeColors;
    }

    protected Dictionary<NodeObject, Color> GetNodeColorsBasedOnClusters(List<NodeObject> nodes, List<Cluster> clusters)
    {
        var nodeColors = new Dictionary<NodeObject, Color>();

        var clustersIds = clusters.ConvertAll(cluster => cluster.Id);
        var colors = colorHelper.GetDistinctColors(clustersIds);

        foreach (var node in nodes)
        {
            var cluster = clusters.FirstOrDefault(c => c.Nodes.Any(n => n.Name == node.Name));
            var nodeColor = cluster != null ? colors[cluster.Id] : Color.WhiteSmoke;

            nodeColors.Add(node, nodeColor);
        }

        return nodeColors;
    }

    protected virtual void AddEdges(TGraph graph, List<NodeSimilarityResult> similarityResults)
    {
        var addedEdges = new HashSet<CustomEdge>();

        foreach (var similarityResult in similarityResults)
        {
            var sourceNode = similarityResult.Node;

            foreach (var similarNode in similarityResult.SimilarNodes)
            {
                if (!IsNeighbor(similarNode))
                    continue;

                var targetNode = similarNode.Node;
                var edge = new CustomEdge(sourceNode, targetNode);

                if (addedEdges.Any(e => e.Equals(edge)))
                    continue;

                AddEdge(graph, sourceNode.Name, targetNode.Name, similarNode.SimilarityPercentage);
                addedEdges.Add(edge);
            }
        }
    }

    protected bool IsNeighbor(SimilarNode similarNode) => similarNode.SimilarityPercentage >= settings.MinSimilarityPercentage;

    protected abstract void AddClusters(TGraph graph, List<Cluster> clusters);
    protected abstract void AddNode(TGraph graph, string nodeName, Color nodeColor);
    protected abstract void AddEdge(TGraph graph, string sourceName, string targetName, float similarityPercentage);
}
