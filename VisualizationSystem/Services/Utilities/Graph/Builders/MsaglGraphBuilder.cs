using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Layout.MDS;
using VisualizationSystem.Models.Domain.Graphs;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.Graph.Helpers;
using VisualizationSystem.Services.Utilities.Settings;
using Cluster = VisualizationSystem.Models.Domain.Clusters.Cluster;
using MsaglColor = Microsoft.Msagl.Drawing.Color;
using SystemColor = System.Drawing.Color;

namespace VisualizationSystem.Services.Utilities.Graph.Builders;

public class MsaglGraphBuilder : GraphBuilder<ExtendedGraph>
{
    public MsaglGraphBuilder(
        GraphColorAssigner colorAssigner,
        ISettingsSubject settingsSubject
        )
        : base(colorAssigner, settingsSubject)
    { }

    public override ExtendedGraph Build(string name, List<NodeSimilarityResult> similarityResults)
    {
        var graph = new ExtendedGraph(name)
        {
            LayoutAlgorithmSettings = new MdsLayoutSettings(),
        };

        AddNodes(graph, similarityResults);
        AddEdges(graph, similarityResults);

        return graph;
    }

    public override ExtendedGraph Build(string name, List<NodeObject> nodes, List<Cluster> clusters)
    {
        var graph = new ExtendedGraph(name)
        {
            LayoutAlgorithmSettings = new SugiyamaLayoutSettings(),
        };

        AddNodes(graph, nodes, clusters);
        //AddEdges(graph, similarityResults);
        AddClusters(graph, clusters);

        return graph;
    }

    protected override void AddClusters(ExtendedGraph graph, List<Cluster> clusters)
    {
        graph.NodeDataMap.Clear();

        foreach (var cluster in clusters)
        {
            var subgraph = new Subgraph(cluster.Id.ToString())
            {
                LabelText = $"Cluster: {cluster.Id}",
            };

            foreach (var node in cluster.Nodes)
            {
                var existingNode = graph.FindNode(node.Name);

                if (existingNode == null)
                    continue;

                subgraph.AddNode(existingNode);
            }

            graph.RootSubgraph.AddSubgraph(subgraph);
        }
    }

    protected override void AddNodes(ExtendedGraph graph, List<NodeSimilarityResult> similarityResults)
    {
        base.AddNodes(graph, similarityResults);

        graph.NodeDataMap = similarityResults
            .ToDictionary(sr => sr.Node.Name, sr => sr);
    }

    protected override void AddNode(ExtendedGraph graph, string nodeName, SystemColor nodeColor)
    {
        var node = new Node(nodeName)
        {
            LabelText = nodeName,
            Attr =
            {
                FillColor = new MsaglColor(nodeColor.R, nodeColor.G, nodeColor.B),
                //LabelMargin = (int)GetNodeSize(edgesCount, maxEdges) TODO
            }
        };

        graph.AddNode(node);
    }

    protected override void AddEdge(ExtendedGraph graph, string firstNodeName, string secondNodeName, float similarityPercentage)
    {
        var edge = graph.AddEdge(firstNodeName, secondNodeName);

        edge.Attr.ArrowheadAtSource = ArrowStyle.None;
        edge.Attr.ArrowheadAtTarget = ArrowStyle.None;

        // TODO
        //edge.LabelText = $"{similarityPercentage:F1}%";
        //edge.Attr.Length = 1.0 / similarityPercentage;

        var edgeColor = colorAssigner.CalculateEdgeColor(similarityPercentage, settings.MinSimilarityPercentage);
        edge.Attr.Color = new MsaglColor(edgeColor.A, edgeColor.R, edgeColor.G, edgeColor.B);
    }
}
