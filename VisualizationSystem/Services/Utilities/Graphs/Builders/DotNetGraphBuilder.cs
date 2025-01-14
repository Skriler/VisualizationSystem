using DotNetGraph.Core;
using DotNetGraph.Extensions;
using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;
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

    public override DotGraph Build(string name, List<NodeSimilarityResult> similarityResults)
    {
        var graph = new DotGraph()
            .WithIdentifier(name);

        AddNodes(graph, similarityResults);
        AddEdges(graph, similarityResults);

        return graph;
    }

    public override DotGraph Build(string name, List<NodeObject> nodes, List<Cluster> clusters)
    {
        var graph = new DotGraph()
            .WithIdentifier(name);

        AddNodes(graph, nodes, clusters);
        AddClusters(graph, clusters);

        return graph;
    }

    public override DotGraph Build(string name, List<NodeSimilarityResult> similarityResults, List<Cluster> clusters)
    {
        var graph = new DotGraph()
            .WithIdentifier(name);

        AddNodes(graph, similarityResults);
        AddEdges(graph, similarityResults);

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
