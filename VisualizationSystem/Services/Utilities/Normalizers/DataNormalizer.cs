using VisualizationSystem.Models.Domain.Ranges;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.DAL;

namespace VisualizationSystem.Services.Utilities.Normalizers;

public class DataNormalizer
{
    private readonly NormalizedNodeRepository normalizedNodesRepository;

    private readonly List<ParameterNumericRange> numericRanges = new();
    private readonly List<ParameterStringRange> stringRanges = new();
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

        numericRanges.Clear();
        stringRanges.Clear();

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
                if (IsNumeric(parameter.Value))
                {
                    AddToNumericRange(parameter);
                }
                else
                {
                    AddToStringParameterList(parameter);
                }
            }
        }
    }

    private void AddToNumericRange(NodeParameter parameter)
    {
        if (stringRanges.Any(range => range.Id == parameter.ParameterTypeId))
            throw new InvalidOperationException($"Conflict detected: string data already exists for {parameter.ParameterTypeId}");

        var value = Convert.ToDouble(parameter.Value);

        var range = numericRanges
            .FirstOrDefault(range => range.Id == parameter.ParameterTypeId);

        if (range == null)
        {
            range = new ParameterNumericRange(parameter.ParameterTypeId, value, value);
            numericRanges.Add(range);
            return;
        }

        range.Update(
            Math.Min(range.Min, value),
            Math.Max(range.Max, value)
            );
    }

    private void AddToStringParameterList(NodeParameter parameter)
    {
        if (string.IsNullOrWhiteSpace(parameter.Value))
            return;

        if (numericRanges.Any(range => range.Id == parameter.ParameterTypeId))
            throw new InvalidOperationException($"Conflict detected: numeric range already exists for {parameter.ParameterTypeId}");

        var range = stringRanges
            .FirstOrDefault(range => range.Id == parameter.ParameterTypeId);

        if (range == null)
        {
            range = new ParameterStringRange(parameter.ParameterTypeId);
            stringRanges.Add(range);
        }

        range.AddValue(parameter.Value);
    }

    private void InitializeParameterStates(NodeObject node)
    {
        foreach (var parameter in node.Parameters)
        {
            if (!IsStringParameter(parameter))
            {
                parameterStates.Add(new NormalizedParameterState() { Weight = 1.0 });
                continue;
            }

            var range = stringRanges.First(r => r.Id == parameter.ParameterTypeId);
            var weight = 1.0 / range.Values.Count;

            var newParameterStates = Enumerable
                .Range(0, range.Values.Count)
                .Select(i => new NormalizedParameterState() { Weight = weight })
                .ToList();

            parameterStates.AddRange(newParameterStates);
        }
    }

    private void ProcessNodeForNormalization(NodeObject node)
    {
        foreach (var parameter in node.Parameters)
        {
            if (IsNumericParameter(parameter))
            {
                AddNormalizedNumericParameter(node, parameter);
            }
            else if (IsStringParameter(parameter))
            {
                AddNormalizedStringParameter(node, parameter);
            }
        }
    }

    private bool IsNumericParameter(NodeParameter parameter) => numericRanges.Any(range => range.Id == parameter.ParameterTypeId);

    private bool IsStringParameter(NodeParameter parameter) => stringRanges.Any(range => range.Id == parameter.ParameterTypeId);

    private void AddNormalizedNumericParameter(NodeObject node, NodeParameter parameter)
    {
        var currentIndex = node.NormalizedParameters.Count;
        var normParameter = new NormalizedParameter()
        {
            Value = GetNormalizedNumericParameter(parameter),
            NodeObject = node,
            NormalizedParameterState = parameterStates[currentIndex],
        };

        node.NormalizedParameters.Add(normParameter);
    }

    private double GetNormalizedNumericParameter(NodeParameter parameter)
    {
        if (!IsNumeric(parameter.Value))
            return 0;

        var range = numericRanges
            .FirstOrDefault(range => range.Id == parameter.ParameterTypeId);

        if (range == null)
            return 0;

        var value = Convert.ToDouble(parameter.Value);
        return NormalizeMinMax(value, range.Min, range.Max);
    }

    private void AddNormalizedStringParameter(NodeObject node, NodeParameter parameter)
    {
        var currentIndex = node.NormalizedParameters.Count;
        var oneHotArray = GetNormalizedStringParameter(parameter);

        for (int i = 0; i < oneHotArray.Length; i++)
        {
            var normalizedParameter = new NormalizedParameter
            {
                Value = oneHotArray[i],
                NodeObject = node,
                NormalizedParameterState = parameterStates[currentIndex + i]
            };

            node.NormalizedParameters.Add(normalizedParameter);
        }
    }

    private double[] GetNormalizedStringParameter(NodeParameter parameter)
    {
        var range = stringRanges
            .FirstOrDefault(range => range.Id == parameter.ParameterTypeId);

        if (range == null)
            return Array.Empty<double>();

        return ConvertStringToOneHot(parameter.Value, range.Values);
    }

    private double NormalizeMinMax(double value, double min, double max)
    {
        if (max == min)
            return 1;

        return (value - min) / (max - min);
    }

    private bool IsNumeric(string value)
    {
        return double.TryParse(value, out _);
    }

    private static double[] ConvertStringToOneHot(string value, List<string> possibleValues)
    {
        var encoded = new double[possibleValues.Count];
        var index = possibleValues.IndexOf(value);

        if (index != -1)
        {
            encoded[index] = 1;
        }

        return encoded;
    }
}
