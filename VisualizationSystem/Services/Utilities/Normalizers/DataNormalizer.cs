using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Nodes.Normalized;
using VisualizationSystem.Models.Storages.Ranges;

namespace VisualizationSystem.Services.Utilities.Normalizers;

public class DataNormalizer
{
    private readonly List<ParameterNumericRange> numericRanges;
    private readonly List<ParameterStringRange> stringRanges;

    public DataNormalizer()
    {
        numericRanges = new();
        stringRanges = new();
    }

    public List<NormalizedNode> GetNormalizedNodes(List<NodeObject> nodes)
    {
        numericRanges.Clear();
        stringRanges.Clear();

        InitializeParameterRanges(nodes);
        var normalizedNodes = NormalizeNodes(nodes);

        return normalizedNodes;
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

    private List<NormalizedNode> NormalizeNodes(List<NodeObject> nodes)
    {
        var normalizedNodes = new List<NormalizedNode>();

        foreach (var node in nodes)
        {
            var normalizedNode = new NormalizedNode(node);
            ProcessNodeForNormalization(normalizedNode, node);

            normalizedNodes.Add(normalizedNode);
        }

        return normalizedNodes;
    }

    private void ProcessNodeForNormalization(NormalizedNode normalizedNode, NodeObject node)
    {
        foreach (var parameter in node.Parameters)
        {
            var normalizedParameter = new NormalizedNodeParameter()
            {
                Id = parameter.ParameterTypeId,
            };

            if (IsNumeric(parameter.Value) && numericRanges.Any(range => range.Id == parameter.ParameterTypeId))
            {
                normalizedParameter.Value = ProcessNumericParameter(normalizedNode, parameter);
                normalizedNode.NormalizedParameters.Add(normalizedParameter);
            }
            else if (stringRanges.Any(range => range.Id == parameter.ParameterTypeId))
            {
                var oneHotArray = ProcessCategoricalParameter(normalizedNode, parameter);

                foreach (var val in oneHotArray)
                {
                    normalizedParameter = new NormalizedNodeParameter
                    {
                        Id = parameter.ParameterTypeId,
                        Value = val
                    };
                    normalizedNode.NormalizedParameters.Add(normalizedParameter);
                }
            }
            else
            {
                normalizedParameter.Value = 0;
                normalizedNode.NormalizedParameters.Add(normalizedParameter);
            }
        }
    }

    private double ProcessNumericParameter(NormalizedNode normalizedNode, NodeParameter parameter)
    {
        var range = numericRanges
            .FirstOrDefault(range => range.Id == parameter.ParameterTypeId);

        if (range == null)
            return 0;

        var value = Convert.ToDouble(parameter.Value);
        return NormalizeMinMax(value, range.Min, range.Max);
    }

    private double[] ProcessCategoricalParameter(NormalizedNode normalizedNode, NodeParameter parameter)
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
