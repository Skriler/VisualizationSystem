using VisualizationSystem.Models.Domain.Nodes;

namespace VisualizationSystem.Services.Utilities.Graphs.Builders;

public interface IGraphBuilder<TGraph>
{
    TGraph Build(TableAnalysisResult analysisResult);
}
