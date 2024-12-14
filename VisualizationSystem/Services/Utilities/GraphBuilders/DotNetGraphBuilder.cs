using DotNetGraph.Core;
using DotNetGraph.Extensions;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.Models.Storages.Clusters;
using Color = System.Drawing.Color;

namespace VisualizationSystem.Services.Utilities.GraphBuilders;

public sealed class DotNetGraphBuilder : GraphBuilder<DotGraph>
{
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

        AddNodes(graph, nodes);
        AddClusters(graph, clusters);

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
                var nodeColor = GetNodeColor(nodeObject.Name);
                AddNode(graph, nodeObject.Name, nodeColor);

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

        //var nodeSize = GetNodeSize(edgesCount, maxEdges);

        node = new DotNode()
            .WithIdentifier(nodeName)
            .WithColor(new DotColor(nodeColor.A, nodeColor.G, nodeColor.B));

        graph.Add(node);
    }

    protected override void AddEdge(DotGraph graph, string firstNodeName, string secondNodeName, float similarityPercentage)
    {
        // Interpolate edge color based on similarity
        var edgeColor = CalculateEdgeColor(similarityPercentage, Settings.MinSimilarityPercentage);

        var edge = new DotEdge()
            .From(firstNodeName)
            .To(secondNodeName)
            .WithColor(new DotColor(edgeColor.R, edgeColor.G, edgeColor.B));

        graph.Add(edge);
    }
}
