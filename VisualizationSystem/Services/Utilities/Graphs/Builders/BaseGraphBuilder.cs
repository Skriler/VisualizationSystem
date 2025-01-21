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

    public abstract TGraph Build(string name, List<NodeSimilarityResult> similarityResults);

    public abstract TGraph Build(string name, List<NodeObject> nodes, List<Cluster> clusters);

    public abstract TGraph Build(string name, List<NodeSimilarityResult> similarityResults, List<Cluster> clusters);

    public void Update(UserSettings settings) => this.settings = settings;

    protected virtual void AddNodes(TGraph graph, List<NodeSimilarityResult> similarityResults)
    {
        var minSimilarity = similarityResults.Min(result => result.SimilarNodes.Min(s => s.SimilarityPercentage));
        var maxSimilarity = similarityResults.Max(result => result.SimilarNodes.Max(s => s.SimilarityPercentage));
        var maxSimilarNodesAboveThreshold = similarityResults
            .Max(result => result.SimilarNodes
                .Count(sn => sn.SimilarityPercentage > settings.MinSimilarityPercentage)
            );

        foreach (var similarityResult in similarityResults)
        {
            var currentNodeName = similarityResult.Node.Name;

            var nodeColor = colorHelper.GetNodeColorFromDensityWithSigmoid(
                similarityResult,
                maxSimilarNodesAboveThreshold,
                settings.MinSimilarityPercentage
                );

            AddNode(
                graph,
                currentNodeName,
                nodeColor
                );
        }
    }

    protected virtual void AddNodes(TGraph graph, List<NodeObject> nodes, List<Cluster> clusters)
    {
        var clustersIds = clusters.ConvertAll(cluster => cluster.Id);
        var colors = colorHelper.GetDistinctColors(clustersIds);

        foreach (var node in nodes)
        {
            var currentNodeName = node.Name;
            var cluster = clusters
                .FirstOrDefault(c => c.Nodes.Any(n => n.Name == currentNodeName));

            var nodeColor = cluster != null ? colors[cluster.Id] : Color.WhiteSmoke;

            AddNode(
                graph,
                currentNodeName,
                nodeColor
            );
        }
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

    private bool IsNeighbor(SimilarNode similarNode) => similarNode.SimilarityPercentage >= settings.MinSimilarityPercentage;

    protected double GetNodeSize(int connectionCount, int maxConnections)
    {
        return 1 + connectionCount / (double)maxConnections * 20;
    }

    protected abstract void AddClusters(TGraph graph, List<Cluster> clusters);
    protected abstract void AddNode(TGraph graph, string nodeName, Color nodeColor);
    protected abstract void AddEdge(TGraph graph, string sourceName, string targetName, float similarityPercentage);
}
