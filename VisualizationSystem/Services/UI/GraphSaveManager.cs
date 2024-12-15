using DotNetGraph.Compilation;
using DotNetGraph.Core;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Storages.Clusters;
using VisualizationSystem.Models.Storages.Results;
using VisualizationSystem.Services.Utilities.GraphBuilders;

namespace VisualizationSystem.Services.UI;

public class GraphSaveManager
{
    private const string GraphName = "graph.dot";

    private readonly IGraphBuilder<DotGraph> graphBuilder;

    public GraphSaveManager(IGraphBuilder<DotGraph> graphBuilder)
    {
        this.graphBuilder = graphBuilder;
    }

    public void UpdateSettings(UserSettings settings) => graphBuilder.UpdateSettings(settings);

    public async Task SaveGraphAsync(string name, List<NodeSimilarityResult> similarityResults)
    {
        var graph = graphBuilder.Build(name, similarityResults);

        await WriteGraphAsync(graph);
    }


    public async Task SaveGraphAsync(string name, List<NodeObject> nodes, List<Cluster> clusters)
    {
        var graph = graphBuilder.Build(name, nodes, clusters);

        await WriteGraphAsync(graph);
    }

    private async Task WriteGraphAsync(DotGraph graph)
    {
        await using var writer = new StringWriter();
        var context = new CompilationContext(writer, new CompilationOptions());

        await graph.CompileAsync(context);
        var result = writer.GetStringBuilder().ToString();

        await File.WriteAllTextAsync(GraphName, result);
    }
}
