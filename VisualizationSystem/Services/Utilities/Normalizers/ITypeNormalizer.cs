using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Normalized;

namespace VisualizationSystem.Services.Utilities.Normalizers;

public interface ITypeNormalizer
{
    int Id { get; }

    ParameterValueType Type { get; }

    int CategoryCount { get; }

    void AddValue(string value);

    NormalizedParameter CreateNormalizedParameter(NodeParameter parameter, NodeObject node, NormalizedParameterState state);
}
