using DotNetGraph.Core;
using DotNetGraph.Extensions;
using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Services.Utilities.Helpers.Colors;
using VisualizationSystem.Services.Utilities.Settings;
using Color = System.Drawing.Color;

namespace VisualizationSystem.Services.Utilities.Graphs.Builders;

public sealed class DotNetGraphBuilder : BaseGraphBuilder<DotGraph>
{
    public DotNetGraphBuilder(
        ColorHelper colorHelper,
        ISettingsSubject settingsSubject
        )
        : base(colorHelper, settingsSubject)
    { }

    public override DotGraph Build(TableAnalysisResult analysisResult)
    {
        if (settings.UseNormalGraph)
        {
            return BuildNormalGraph(analysisResult);
        }

        if (settings.UseClustering)
        {
            return BuildClusters(analysisResult);
        }

        return BuildClusteredGraph(analysisResult);
    }

    protected override DotGraph BuildNormalGraph(TableAnalysisResult analysisResult)
    {
        var graph = new DotGraph()
            .WithIdentifier(analysisResult.Name);

        AddNodes(graph, analysisResult);
        AddEdges(graph, analysisResult.SimilarityResults);

        return graph;
    }

    protected override DotGraph BuildClusters(TableAnalysisResult analysisResult)
    {
        var graph = new DotGraph()
            .WithIdentifier(analysisResult.Name);

        AddNodes(graph, analysisResult);
        AddClusters(graph, analysisResult.Clusters);

        return graph;
    }

    protected override DotGraph BuildClusteredGraph(TableAnalysisResult analysisResult)
    {
        var graph = new DotGraph()
            .WithIdentifier(analysisResult.Name);

        AddNodes(graph, analysisResult);
        AddEdges(graph, analysisResult.SimilarityResults);

        return graph;
    }

    protected override void AddClusters(DotGraph graph, List<Cluster> clusters)
    {
        foreach (var cluster in clusters)
        {
            var subgraph = new DotSubgraph()
                .WithIdentifier(cluster.Id.ToString());

            foreach (var nodeObject in cluster.Nodes)
            {
                AddNode(graph, nodeObject.Name, Color.AliceBlue);

                var node = graph.GetNodeByIdentifier(nodeObject.Name);

                if (node == null)
                    continue;

                subgraph.Add(node);
            }

            graph.Add(subgraph);
        }
    }

    protected override void AddNode(DotGraph graph, string nodeName, Color nodeColor)
    {
        var node = graph.GetNodeByIdentifier(nodeName);

        if (node != null)
            return;

        node = new DotNode()
            .WithIdentifier(nodeName)
            .WithColor(new DotColor(nodeColor.A, nodeColor.G, nodeColor.B));

        graph.Add(node);
    }

    protected override void AddEdge(DotGraph graph, string sourceName, string targetName, float similarityPercentage)
    {
        // Interpolate edge color based on similarity
        var edgeColor = colorHelper.CalculateEdgeColor(similarityPercentage, settings.MinSimilarityPercentage);

        var edge = new DotEdge()
            .From(sourceName)
            .To(targetName)
            .WithColor(new DotColor(edgeColor.R, edgeColor.G, edgeColor.B));

        graph.Add(edge);
    }
}
