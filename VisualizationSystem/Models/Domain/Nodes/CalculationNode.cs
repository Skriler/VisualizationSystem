using VisualizationSystem.Models.Domain.Nodes.Parameters;
using VisualizationSystem.Models.Entities.Normalized;

namespace VisualizationSystem.Models.Domain.Nodes;

public class CalculationNode
{
    public string Name { get; }

    public List<BaseParameter> Parameters { get; private set; }

    public CalculationNode(string name, List<NormalizedParameter> parameters)
    {
        Name = name;
        Parameters = parameters.ConvertAll(ConvertParameter);
    }

    public CalculationNode(CalculationNode other)
    {
        Name = other.Name;
        Parameters = other.Parameters
            .ConvertAll(p => p.Clone());
    }

    public int GetFeaturesCount()
    {
        if (Parameters == null || Parameters.Count == 0)
            return 0;

        return Parameters.Sum(parameter => parameter switch
        {
            NumericParameter => 1,
            CategoricalParameter categorical => categorical.CategoryCount,
            _ => 0
        });
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
