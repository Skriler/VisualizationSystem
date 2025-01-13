﻿using VisualizationSystem.Models.Domain.Nodes.Parameters;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Normalized;

namespace VisualizationSystem.Models.Domain.Nodes;

public class CalculationNode
{
    public string Name { get; }

    public List<BaseParameter> Parameters { get; private set; }

    public CalculationNode(NodeObject node)
    {
        Name = node.Name;
        Parameters = node.NormalizedParameters
            .ConvertAll(ConvertParameter);
    }

    public CalculationNode(CalculationNode other)
    {
        Name = other.Name;
        Parameters = other.Parameters
            .ConvertAll(p => p.Clone());
    }

    public void Merge(CalculationNode other)
    {
        Parameters = Parameters
            .Zip(other.Parameters, (param, otherParam) => param.Merge(otherParam))
            .ToList();
    }

    private static BaseParameter ConvertParameter(NormalizedParameter param) => param switch
    {
        NormalizedNumericParameter numericParam =>
            new NumericParameter(numericParam.Value),
        NormalizedCategoricalParameter categoricalParam =>
            new CategoricalParameter(
                categoricalParam.OneHotIndexes,
                categoricalParam.NormalizedParameterState.CategoryCount
            ),
        _ => throw new ArgumentException($"Unknown parameter type: {param.GetType()}")
    };
}