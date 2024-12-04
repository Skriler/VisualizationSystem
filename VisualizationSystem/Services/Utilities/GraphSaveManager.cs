using DotNetGraph.Compilation;
using DotNetGraph.Core;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.Services.Utilities.GraphBuilders;

namespace VisualizationSystem.Services.Utilities;

public class GraphSaveManager
{
    private readonly IGraphBuilder<DotGraph> graphBuilder;

    public GraphSaveManager(IGraphBuilder<DotGraph> graphBuilder)
    {
        this.graphBuilder = graphBuilder;
    }

    public void UpdateSettings(UserSettings settings) => graphBuilder.UpdateSettings(settings);

    public async Task SaveGraphAsync(string name, List<NodeSimilarityResult> similarityResults, List<Cluster> clusters)
    {
        var graph = graphBuilder.Build(name, similarityResults, clusters);

        await using var writer = new StringWriter();
        var context = new CompilationContext(writer, new CompilationOptions());

        await graph.CompileAsync(context);
        var result = writer.GetStringBuilder().ToString();

        File.WriteAllText("graph.dot", result);
    }
}
