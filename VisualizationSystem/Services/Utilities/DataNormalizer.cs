using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.Models.Storages.Matrixes;

namespace VisualizationSystem.Services.Utilities;

public class DataNormalizer
{
    public readonly DataMatrixManager matrixManager;

    private List<NodeObject> nodes;

    public DataNormalizer()
    {
        matrixManager = new DataMatrixManager();
    }

    public void NormalizeNodeParameters(List<NodeObject> nodes)
    {
        matrixManager.Clear();

        this.nodes = nodes;

        InitializeParameterRangesAndStringParameters();
       ProcessNodesForNormalization();
    }

    private void InitializeParameterRangesAndStringParameters()
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
        double value = Convert.ToDouble(parameter.Value);

        if (!matrixManager.ParameterRanges.ContainsKey(parameter.ParameterTypeId))
        {
            matrixManager.ParameterRanges[parameter.ParameterTypeId] = new ParameterRange(value, value);
        }
        else
        {
            var currentRange = matrixManager.ParameterRanges[parameter.ParameterTypeId];
            matrixManager.ParameterRanges[parameter.ParameterTypeId] = new ParameterRange(
                Math.Min(currentRange.Min, value),
                Math.Max(currentRange.Max, value)
            );
        }
    }

    private void AddToStringParameterList(NodeParameter parameter)
    {
        if (string.IsNullOrWhiteSpace(parameter.Value))
            return;

        if (!matrixManager.StringParameters.ContainsKey(parameter.ParameterTypeId))
        {
            matrixManager.StringParameters[parameter.ParameterTypeId] = new List<string>();
        }

        if (!matrixManager.StringParameters[parameter.ParameterTypeId].Contains(parameter.Value))
        {
            matrixManager.StringParameters[parameter.ParameterTypeId].Add(parameter.Value);
        }
    }

    private void ProcessNodesForNormalization()
    {
        for (int row = 0; row < nodes.Count; ++row)
        {
            ProcessNodeForNormalization(nodes[row], row);
        }
    }

    private void ProcessNodeForNormalization(NodeObject node, int rowIndex)
    {
        int numericColumnIndex = 0;
        int categoricalColumnIndex = 0;

        foreach (var parameter in node.Parameters)
        {
            if (IsNumeric(parameter.Value))
            {
                ProcessNumericParameter(parameter, rowIndex, numericColumnIndex, node.Parameters.Count);
                numericColumnIndex++;
            }
            else
            {
                ProcessCategoricalParameter(parameter, rowIndex, categoricalColumnIndex);
                categoricalColumnIndex++;
            }
        }
    }

    private void ProcessNumericParameter(NodeParameter node, int rowIndex, int columnIndex, int parametersCount)
    {
        double value = Convert.ToDouble(node.Value);

        if (matrixManager.ParameterRanges.ContainsKey(node.ParameterTypeId))
        {
            var range = matrixManager.ParameterRanges[node.ParameterTypeId];
            double normalizedValue = NormalizeMinMax(value, range.Min, range.Max);

            EnsureMatrixExists(node.ParameterTypeId, nodes.Count, parametersCount);
            matrixManager.Matrices[node.ParameterTypeId].SetValue(rowIndex, columnIndex, normalizedValue);
        }
    }

    private void ProcessCategoricalParameter(NodeParameter parameter, int rowIndex, int categoricalColumnIndex)
    {
        var possibleValues = matrixManager.StringParameters[parameter.ParameterTypeId];
        double[] encodedValue = ConvertStringToOneHot(parameter.Value, possibleValues);

        EnsureMatrixExists(parameter.ParameterTypeId, nodes.Count, possibleValues.Count);

        for (int i = 0; i < encodedValue.Length; i++)
        {
            matrixManager.Matrices[parameter.ParameterTypeId].SetValue(rowIndex, categoricalColumnIndex++, encodedValue[i]);
        }
    }

    private void EnsureMatrixExists(int parameterTypeId, int rows, int columns)
    {
        if (matrixManager.Matrices.ContainsKey(parameterTypeId))
            return;

        matrixManager.AddMatrix(parameterTypeId, new DataMatrix(rows, columns));
    }

    private double NormalizeMinMax(double value, double min, double max)
    {
        return (value - min) / (max - min);
    }

    private bool IsNumeric(string value)
    {
        return double.TryParse(value, out _);
    }

    private static double[] ConvertStringToOneHot(string value, List<string> possibleValues)
    {
        double[] encoded = new double[possibleValues.Count];

        int index = possibleValues.IndexOf(value);

        if (index != -1)
        {
            encoded[index] = 1;
        }

        return encoded;
    }
}
