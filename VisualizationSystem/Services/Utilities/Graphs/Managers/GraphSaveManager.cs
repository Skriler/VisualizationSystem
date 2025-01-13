using DotNetGraph.Core;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.Comparers;
using VisualizationSystem.Services.Utilities.Factories;
using VisualizationSystem.Services.Utilities.FileSystem;
using VisualizationSystem.Services.Utilities.Graphs.Builders;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Graphs.Managers;

public class GraphSaveManager : GraphCreationManager<DotGraph>
{
    private const string GraphName = "graph.dot";

    private readonly FileWriter fileWriter;

    public GraphSaveManager(
        IGraphBuilder<DotGraph> graphBuilder,
        FileWriter fileWriter,
        SimilarityComparer similarityComparer,
        ClustererFactory clustererFactory,
        ISettingsSubject settingsSubject
        )
        : base(graphBuilder, similarityComparer, clustererFactory, settingsSubject)
    {
        this.fileWriter = fileWriter;
    }

    public async Task SaveGraphAsync(NodeTable nodeTable)
    {
        var graph = await BuildGraphAsync(nodeTable);

        await fileWriter.WriteGraphAsync(graph, GraphName);
    }
}
