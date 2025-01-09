using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Normalized;
using VisualizationSystem.Services.DAL;

namespace VisualizationSystem.Services.Utilities.Normalizers;

public class DataNormalizer
{
    private readonly NormalizedNodeRepository normalizedNodesRepository;

    private readonly Dictionary<int, ITypeNormalizer> parameterNormalizers = new();
    private readonly List<NormalizedParameterState> parameterStates = new();

    public DataNormalizer(NormalizedNodeRepository normalizedNodesRepository)
    {
        this.normalizedNodesRepository = normalizedNodesRepository;
    }

    public async Task<List<NodeObject>> GetNormalizedNodesAsync(NodeTable nodeTable)
    {
        if (await normalizedNodesRepository.ExistsAsync(nodeTable.Name))
        {
            return await normalizedNodesRepository.GetByTableNameAsync(nodeTable.Name);
        }

        parameterNormalizers.Clear();
        parameterStates.Clear();

        InitializeParameterRanges(nodeTable.NodeObjects);
        InitializeParameterStates(nodeTable.NodeObjects.First());
        await normalizedNodesRepository.AddNormalizedParameterStateListAsync(parameterStates);

        nodeTable.NodeObjects.ForEach(ProcessNodeForNormalization);
        await normalizedNodesRepository.AddAllNormalizedParametersAsync(nodeTable.NodeObjects);

        return nodeTable.NodeObjects;
    }

    private void InitializeParameterRanges(List<NodeObject> nodes)
    {
        foreach (var node in nodes)
        {
            foreach (var parameter in node.Parameters)
            {
                var valueType = GetParameterValueType(parameter.Value);
                AddToParameterRange(parameter, valueType);
            }
        }
    }

    private ParameterValueType GetParameterValueType(string value)
        => IsNumeric(value) ? ParameterValueType.Numeric : ParameterValueType.Categorical;

    private void AddToParameterRange(NodeParameter parameter, ParameterValueType valueType)
    {
        if (parameterNormalizers.TryGetValue(parameter.ParameterTypeId, out var existingRange))
        {
            existingRange.AddValue(parameter.Value);
            return;
        }

        var range = CreateParameterRange(valueType, parameter.ParameterTypeId, parameter.Value);
        parameterNormalizers.Add(parameter.ParameterTypeId, range);
    }

    private ITypeNormalizer CreateParameterRange(ParameterValueType valueType, int id, string value)
    {
        return valueType switch
        {
            ParameterValueType.Numeric => new NumericNormalizer(id, Convert.ToDouble(value)),
            ParameterValueType.Categorical => new CategoricalNormalizer(id, value),
            _ => throw new ArgumentException($"Unsupported parameter type: {valueType}")
        };
    }

    private void InitializeParameterStates(NodeObject node)
    {
        foreach (var parameter in node.Parameters)
        {
            var range = parameterNormalizers[parameter.ParameterTypeId];

            var parameterState = new NormalizedParameterState
            {
                CategoryCount = range.CategoryCount,
                ValueType = range.Type,
                ParameterType = parameter.ParameterType
            };

            parameterStates.Add(parameterState);
        }
    }

    private void ProcessNodeForNormalization(NodeObject node)
    {
        foreach (var parameter in node.Parameters)
        {
            var range = parameterNormalizers[parameter.ParameterTypeId];
            var normalizedParameter = range.CreateNormalizedParameter(
                parameter,
                node,
                parameterStates[node.NormalizedParameters.Count]
                );

            node.NormalizedParameters.Add(normalizedParameter);
        }
    }

    private static bool IsNumeric(string value)
    {
        return double.TryParse(value, out _);
    }
}
