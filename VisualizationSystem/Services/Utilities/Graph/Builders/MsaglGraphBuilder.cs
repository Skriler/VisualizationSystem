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

public class MsaglGraphBuilder : BaseGraphBuilder<ExtendedGraph>
{
    private const int MinEdgesPerNode = 3;

    public MsaglGraphBuilder(
        GraphColorAssigner colorAssigner,
        ISettingsSubject settingsSubject
        )
        : base(colorAssigner, settingsSubject)
    { }

    public override ExtendedGraph Build(string name, List<NodeSimilarityResult> similarityResults)
    {
        var graph = new ExtendedGraph(name, "Graph")
        {
            LayoutAlgorithmSettings = new MdsLayoutSettings(),
        };

        AddNodes(graph, similarityResults);
        AddEdges(graph, similarityResults);

        return graph;
    }

    public override ExtendedGraph Build(string name, List<NodeObject> nodes, List<Cluster> clusters)
    {
        var graph = new ExtendedGraph(name, "Graph")
        {
            LayoutAlgorithmSettings = new SugiyamaLayoutSettings(),
        };

        AddNodes(graph, nodes, clusters);
        AddClusters(graph, clusters);

        return graph;
    }

    public override ExtendedGraph Build(string name, List<NodeSimilarityResult> similarityResults, List<Cluster> clusters)
    {
        var graph = new ExtendedGraph(name, "Clusters")
        {
            LayoutAlgorithmSettings = new MdsLayoutSettings(),
        };

        AddNodes(graph, similarityResults, clusters);
        AddEdges(graph, similarityResults);
        AddExtraEdges(graph, similarityResults);

        return graph;
    }

    protected override void AddClusters(ExtendedGraph graph, List<Cluster> clusters)
    {
        graph.NodeDataMap.Clear();

        foreach (var cluster in clusters)
        {
            var subgraph = new Subgraph(cluster.Id.ToString())
            {
                LabelText = string.Empty,
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

    protected void AddNodes(ExtendedGraph graph, List<NodeSimilarityResult> similarityResults, List<Cluster> clusters)
    {
        var nodes = similarityResults
            .ConvertAll(result => result.Node);

        base.AddNodes(graph, nodes, clusters);

        graph.NodeDataMap = similarityResults
            .ToDictionary(sr => sr.Node.Name, sr => sr);
    }

    protected void AddExtraEdges(ExtendedGraph graph, List<NodeSimilarityResult> similarityResults)
    {
        foreach (var similarityResult in similarityResults)
        {
            var sourceNode = graph.FindNode(similarityResult.Node.Name);

            if (sourceNode == null)
                continue;

            var edgesCount = sourceNode.InEdges.Count() +
                sourceNode.OutEdges.Count();

            if (edgesCount > MinEdgesPerNode)
                continue;

            var similarNodes = similarityResult.SimilarNodes
                .Where(sn => sn.SimilarityPercentage > 0)
                .OrderByDescending(sr => sr.SimilarityPercentage)
                .Take(edgesCount + MinEdgesPerNode)
                .ToList();

            foreach (var similarNode in similarNodes)
            {
                var targetNode = graph.FindNode(similarNode.Node.Name);

                if (graph.Edges.Any(e => HasEdge(e, sourceNode, targetNode)))
                    continue;

                AddEdge(graph, sourceNode.Id, targetNode.Id, similarNode.SimilarityPercentage);
            }
        }
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

    protected override void AddEdge(ExtendedGraph graph, string sourceName, string targetName, float similarityPercentage)
    {
        var edge = graph.AddEdge(sourceName, targetName);

        edge.Attr.ArrowheadAtSource = ArrowStyle.None;
        edge.Attr.ArrowheadAtTarget = ArrowStyle.None;

        // TODO
        //edge.LabelText = $"{similarityPercentage:F1}%";
        //edge.Attr.Length = 1.0 / similarityPercentage;

        var minSimilarityPercentage = Math.Min(settings.MinSimilarityPercentage, similarityPercentage);
        var edgeColor = colorAssigner.CalculateEdgeColor(similarityPercentage, minSimilarityPercentage);
        edge.Attr.Color = new MsaglColor(edgeColor.A, edgeColor.R, edgeColor.G, edgeColor.B);
    }

    protected bool HasEdge(Edge edge, Node source, Node target)
    {
        return (edge.SourceNode == source && edge.TargetNode == target) ||
               (edge.SourceNode == target && edge.TargetNode == source);
    }
}
