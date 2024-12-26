using VisualizationSystem.Models.Domain.Ranges;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Nodes.Normalized;
using VisualizationSystem.Services.DAL;

namespace VisualizationSystem.Services.Utilities.Normalizers;

public class DataNormalizer
{
    private readonly NormalizedNodeRepository normalizedNodeRepository;

    private readonly List<ParameterNumericRange> numericRanges = new();
    private readonly List<ParameterStringRange> stringRanges = new();

    public DataNormalizer(NormalizedNodeRepository normalizedNodeRepository)
    {
        this.normalizedNodeRepository = normalizedNodeRepository;
    }

    public async Task<List<NormalizedNode>> GeNormalizedNodesAsync(NodeTable nodeTable)
    {
        if (await normalizedNodeRepository.ExistsAsync(nodeTable.Name))
        {
            return await normalizedNodeRepository.GetByTableNameAsync(nodeTable.Name);
        }

        var normalizedNodes = CreateNormalizedNodes(nodeTable.NodeObjects);

        normalizedNodes.ForEach(nn => nn.NodeTable = nodeTable);
        await normalizedNodeRepository.AddRangeAsync(normalizedNodes);

        return normalizedNodes;
    }

    private List<NormalizedNode> CreateNormalizedNodes(List<NodeObject> nodes)
    {
        numericRanges.Clear();
        stringRanges.Clear();

        InitializeParameterRanges(nodes);
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

    private List<NormalizedNode> MapNodesToNormalizedNodes(List<NodeObject> nodes)
    {
        var normalizedNodes = new List<NormalizedNode>();

        foreach (var node in nodes)
        {
            var normalizedNode = new NormalizedNode()
            {
                NodeObject = node,
            };

            ProcessNodeForNormalization(normalizedNode, node);
            normalizedNodes.Add(normalizedNode);
        }

        return normalizedNodes;
    }

    private void ProcessNodeForNormalization(NormalizedNode normalizedNode, NodeObject node)
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

    private void ProcessNumericParameter(NormalizedNode normalizedNode, NodeParameter parameter)
    {
        var normalizedParameter = new NormalizedNodeParameter()
        {
            Value = GetNormalizedNumericParameter(normalizedNode, parameter),
            Weight = 1,
        };

        normalizedNode.NormalizedParameters.Add(normalizedParameter);
    }

    private double GetNormalizedNumericParameter(NormalizedNode normalizedNode, NodeParameter parameter)
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

    private void ProcessStringParameter(NormalizedNode normalizedNode, NodeParameter parameter)
    {
        var oneHotArray = GetNormalizedStringParameter(normalizedNode, parameter);

        foreach (var val in oneHotArray)
        {
            var normalizedParameter = new NormalizedNodeParameter
            {
                Value = val,
                Weight = 1 / oneHotArray.Length,
            };

            normalizedNode.NormalizedParameters.Add(normalizedParameter);
        }
    }

    private double[] GetNormalizedStringParameter(NormalizedNode normalizedNode, NodeParameter parameter)
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
