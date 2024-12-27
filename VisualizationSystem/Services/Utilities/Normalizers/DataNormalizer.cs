using VisualizationSystem.Models.Domain.Ranges;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Nodes.Normalized;
using VisualizationSystem.Services.DAL;

namespace VisualizationSystem.Services.Utilities.Normalizers;

public class DataNormalizer
{
    private readonly NormalizedNodeRepository normNodeRepository;

    private readonly List<ParameterNumericRange> numericRanges = new();
    private readonly List<ParameterStringRange> stringRanges = new();
    private readonly List<NormParameterState> parameterStates = new();

    public DataNormalizer(NormalizedNodeRepository normalizedNodeRepository)
    {
        this.normNodeRepository = normalizedNodeRepository;
    }

    public async Task<List<NormNode>> GeNormalizedNodesAsync(NodeTable nodeTable)
    {
        if (await normNodeRepository.ExistsAsync(nodeTable.Name))
        {
            return await normNodeRepository.GetByTableNameAsync(nodeTable.Name);
        }

        var normNodes = CreateNormalizedNodes(nodeTable.NodeObjects);

        normNodes.ForEach(nn => nn.NodeTable = nodeTable);
        await normNodeRepository.AddRangeAsync(normNodes);

        return normNodes;
    }

    private List<NormNode> CreateNormalizedNodes(List<NodeObject> nodes)
    {
        numericRanges.Clear();
        stringRanges.Clear();
        parameterStates.Clear();

        InitializeParameterRanges(nodes);
        CreateParameterStates(nodes.First());
        return MapNodesToNormalizedNodes(nodes);
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

    private void CreateParameterStates(NodeObject node)
    {
        foreach (var parameter in node.Parameters)
        {
            if (!IsStringParameter(parameter))
            {
                parameterStates.Add(new NormParameterState() { Weight = 1.0 });
                continue;
            }

            var range = stringRanges.First(r => r.Id == parameter.ParameterTypeId);
            var weight = 1.0 / range.Values.Count;

            var newParameterStates = Enumerable
                .Range(0, range.Values.Count)
                .Select(i => new NormParameterState() { Weight = weight })
                .ToList();

            parameterStates.AddRange(newParameterStates);
        }
    }

    private List<NormNode> MapNodesToNormalizedNodes(List<NodeObject> nodes)
    {
        var normNodes = new List<NormNode>();

        foreach (var node in nodes)
        {
            var normNode = new NormNode()
            {
                NodeObject = node,
            };

            ProcessNodeForNormalization(normNode, node);
            normNodes.Add(normNode);
        }

        return normNodes;
    }

    private void ProcessNodeForNormalization(NormNode normalizedNode, NodeObject node)
    {
        foreach (var parameter in node.Parameters)
        {
            if (IsNumericParameter(parameter))
            {
                ProcessNumericParameter(normalizedNode, parameter);
            }
            else if (IsStringParameter(parameter))
            {
                ProcessStringParameter(normalizedNode, parameter);
            }
        }
    }

    private bool IsNumericParameter(NodeParameter parameter) => numericRanges.Any(range => range.Id == parameter.ParameterTypeId);

    private bool IsStringParameter(NodeParameter parameter) => stringRanges.Any(range => range.Id == parameter.ParameterTypeId);

    private void ProcessNumericParameter(NormNode normalizedNode, NodeParameter parameter)
    {
        var currentIndex = normalizedNode.NormParameters.Count;
        var normParameter = new NormParameter()
        {
            Value = GetNormalizedNumericParameter(normalizedNode, parameter),
            NormParameterState = parameterStates[currentIndex],
        };

        normalizedNode.NormParameters.Add(normParameter);
    }

    private double GetNormalizedNumericParameter(NormNode normalizedNode, NodeParameter parameter)
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

    private void ProcessStringParameter(NormNode normalizedNode, NodeParameter parameter)
    {
        var currentIndex = normalizedNode.NormParameters.Count;
        var oneHotArray = GetNormalizedStringParameter(normalizedNode, parameter);

        for (int i = 0; i < oneHotArray.Length; i++)
        {
            var normalizedParameter = new NormParameter
            {
                Value = oneHotArray[i],
                NormParameterState = parameterStates[currentIndex + i]
            };

            normalizedNode.NormParameters.Add(normalizedParameter);
        }
    }

    private double[] GetNormalizedStringParameter(NormNode normalizedNode, NodeParameter parameter)
    {
        var range = stringRanges
            .FirstOrDefault(range => range.Id == parameter.ParameterTypeId);

        if (range == null)
            return new double[0];

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
