using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Layout.MDS;
using VisualizationSystem.Models.Domain.Graphs;
using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Services.Utilities.Helpers.Colors;
using VisualizationSystem.Services.Utilities.Settings;
using Cluster = VisualizationSystem.Models.Domain.Clusters.Cluster;
using MsaglColor = Microsoft.Msagl.Drawing.Color;
using SystemColor = System.Drawing.Color;

namespace VisualizationSystem.Services.Utilities.Graphs.Builders;

public class MsaglGraphBuilder : BaseGraphBuilder<ExtendedGraph>
{
    private const int MinEdgesPerNodeClusteredGraph = 3;

    public MsaglGraphBuilder(
        ColorHelper colorHelper,
        ISettingsSubject settingsSubject
    )
    : base(colorHelper, settingsSubject)
    { }

    public override ExtendedGraph Build(TableAnalysisResult analysisResult)
    {
        if (settings.UseClusteredGraph)
        {
            return BuildClusteredGraph(analysisResult);
        }

        if (settings.UseClustering)
        {
            return BuildClusters(analysisResult);
        }

        return BuildNormalGraph(analysisResult);
    }

    protected override ExtendedGraph BuildNormalGraph(TableAnalysisResult analysisResult)
    {
        var graph = new ExtendedGraph(analysisResult.Name, "Graph")
        {
            LayoutAlgorithmSettings = new MdsLayoutSettings(),
        };

        AddNodes(graph, analysisResult);
        AddEdges(graph, analysisResult.SimilarityResults);

        return graph;
    }

    protected override ExtendedGraph BuildClusters(TableAnalysisResult analysisResult)
    {
        var graph = new ExtendedGraph(analysisResult.Name, "Clusters")
        {
            LayoutAlgorithmSettings = new SugiyamaLayoutSettings(),
        };

        AddNodes(graph, analysisResult);
        AddClusters(graph, analysisResult.Clusters);

        return graph;
    }

    protected override ExtendedGraph BuildClusteredGraph(TableAnalysisResult analysisResult)
    {
        var graph = new ExtendedGraph(analysisResult.Name, "Clustered graph")
        {
            LayoutAlgorithmSettings = new MdsLayoutSettings(),
        };

        AddNodes(graph, analysisResult);
        AddEdges(graph, analysisResult.SimilarityResults);

        return graph;
    }

    protected override void AddClusters(ExtendedGraph graph, List<Cluster> clusters)
    {
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

    protected override void AddNodes(ExtendedGraph graph, TableAnalysisResult analysisResult)
    {
        base.AddNodes(graph, analysisResult);

        graph.NodeDataMap = analysisResult.SimilarityResults.ToDictionary(sr => sr.Node.Name, sr => sr);
    }

    protected override void AddEdges(ExtendedGraph graph, List<NodeSimilarityResult> similarityResults)
    {
        base.AddEdges(graph, similarityResults);

        if (settings.UseClusteredGraph)
            AddEdgesForMinRequirement(graph, similarityResults);
    }

    protected override void AddNode(ExtendedGraph graph, string nodeName, SystemColor nodeColor)
    {
        var node = new Node(nodeName)
        {
            LabelText = nodeName,
            Attr =
            {
                FillColor = new MsaglColor(nodeColor.R, nodeColor.G, nodeColor.B),
            }
        };

        graph.AddNode(node);
    }

    protected override void AddEdge(ExtendedGraph graph, string sourceName, string targetName, float similarityPercentage)
    {
        var edge = graph.AddEdge(sourceName, targetName);

        edge.Attr.ArrowheadAtSource = ArrowStyle.None;
        edge.Attr.ArrowheadAtTarget = ArrowStyle.None;

        var minSimilarityPercentage = Math.Min(settings.MinSimilarityPercentage, similarityPercentage);

        var edgeColor = colorHelper.CalculateEdgeColor(similarityPercentage, minSimilarityPercentage);
        edge.Attr.Color = new MsaglColor(edgeColor.A, edgeColor.R, edgeColor.G, edgeColor.B);
    }

    protected void AddEdgesForMinRequirement(ExtendedGraph graph, List<NodeSimilarityResult> similarityResults)
    {
        foreach (var similarityResult in similarityResults)
        {
            var source = graph.FindNode(similarityResult.Node.Name);

            if (source == null)
                continue;

            var edgesCount = source.InEdges.Count() + source.OutEdges.Count();

            if (edgesCount > MinEdgesPerNodeClusteredGraph)
                continue;

            var similarNodes = GetSimilarNodesForExtraEdges(similarityResult);

            AddEdges(graph, source, similarNodes);
        }
    }

    protected List<SimilarNode> GetSimilarNodesForExtraEdges(NodeSimilarityResult similarityResult)
    {
        return similarityResult.SimilarNodes
            .Where(IsInExtraEdgeSimilarityRange)
            .OrderByDescending(sr => sr.SimilarityPercentage)
            .Take(MinEdgesPerNodeClusteredGraph)
            .ToList();
    }

    protected void AddEdges(ExtendedGraph graph, Node source, List<SimilarNode> similarNodes)
    {
        foreach (var similarNode in similarNodes)
        {
            var target = graph.FindNode(similarNode.Node.Name);

            if (graph.Edges.Any(e => HasEdge(e, source, target)))
                continue;

            AddEdge(graph, source.Id, target.Id, similarNode.SimilarityPercentage);
        }
    }

    protected bool IsInExtraEdgeSimilarityRange(SimilarNode node) =>
        node.SimilarityPercentage > 0 && node.SimilarityPercentage < settings.MinSimilarityPercentage;

    protected static bool HasEdge(Edge edge, Node source, Node target)
    {
        return (edge.SourceNode == source && edge.TargetNode == target) ||
               (edge.SourceNode == target && edge.TargetNode == source);
    }
}
