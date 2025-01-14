﻿using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Normalized;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.DAL;

namespace VisualizationSystem.Services.Utilities.Normalizers;

public class DataNormalizer
{
    private readonly NormalizedNodeRepository normalizedNodesRepository;

    private readonly Dictionary<int, ITypeNormalizer> parameterNormalizers = new();
    private readonly List<NormalizedParameterState> normParameterStates = new();

    public DataNormalizer(NormalizedNodeRepository normalizedNodesRepository)
    {
        this.normalizedNodesRepository = normalizedNodesRepository;
    }

    public async Task<List<CalculationNode>> GetCalculationNodesAsync(
        NodeTable nodeTable,
        List<ParameterState> parameterStates
        )
    {
        if (await normalizedNodesRepository.ExistsAsync(nodeTable.Name))
        {
            var normalizedNodes = await normalizedNodesRepository.GetByTableNameAsync(nodeTable.Name);
            return GetFilteredCalculationNodes(normalizedNodes, parameterStates);
        }

        parameterNormalizers.Clear();
        normParameterStates.Clear();

        var nodes = nodeTable.NodeObjects;
        InitializeParameterRanges(nodes);
        InitializeNormalizedParameterStates(nodes.First());
        await normalizedNodesRepository.AddNormalizedParameterStateListAsync(normParameterStates);

        nodes.ForEach(ProcessNodeForNormalization);
        await normalizedNodesRepository.AddAllNormalizedParametersAsync(nodes);

        return GetFilteredCalculationNodes(nodes, parameterStates);
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
    {
        if (value == string.Empty)
            return ParameterValueType.None;

        return IsNumeric(value) ? ParameterValueType.Numeric : ParameterValueType.Categorical;
    }

    private void AddToParameterRange(NodeParameter parameter, ParameterValueType valueType)
    {
        if (valueType == ParameterValueType.None)
            return;

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

    private void InitializeNormalizedParameterStates(NodeObject node)
    {
        foreach (var parameter in node.Parameters)
        {
            if (!parameterNormalizers.ContainsKey(parameter.ParameterTypeId))
                continue;

            var range = parameterNormalizers[parameter.ParameterTypeId];
            var normParameterState = new NormalizedParameterState
            {
                CategoryCount = range.CategoryCount,
                ValueType = range.Type,
                ParameterType = parameter.ParameterType
            };

            normParameterStates.Add(normParameterState);
        }
    }

    private void ProcessNodeForNormalization(NodeObject node)
    {
        foreach (var parameter in node.Parameters)
        {
            if (!parameterNormalizers.ContainsKey(parameter.ParameterTypeId))
                continue;

            var range = parameterNormalizers[parameter.ParameterTypeId];
            var normalizedParameter = range.CreateNormalizedParameter(
                parameter,
                node,
                normParameterStates[node.NormalizedParameters.Count]
                );

            node.NormalizedParameters.Add(normalizedParameter);
        }
    }

    private static bool IsNumeric(string value)
    {
        return double.TryParse(value, out _);
    }

    private List<CalculationNode> GetFilteredCalculationNodes(
        List<NodeObject> nodes,
        List<ParameterState> parameterStates
        )
    {
        var calculationNodes = new List<CalculationNode>();

        foreach (var node in nodes)
        {
            var filteredParameters = GetFilteredNormalizedParameters(node.NormalizedParameters, parameterStates);
            var calculationNode = new CalculationNode(node.Name, filteredParameters);

            calculationNodes.Add(calculationNode);
        }

        return calculationNodes;
    }

    private List<NormalizedParameter> GetFilteredNormalizedParameters(
        List<NormalizedParameter> parameters,
        List<ParameterState> parameterStates
        )
    {
        var activeParameterTypeIds = parameterStates
            .Where(ps => ps.IsActive)
            .Select(ps => ps.ParameterTypeId)
            .ToList();

        return parameters
            .Where(np => activeParameterTypeIds.Contains(np.NormalizedParameterState.ParameterTypeId))
            .ToList();
    }
}
