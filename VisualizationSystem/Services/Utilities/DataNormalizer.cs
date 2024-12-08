using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.Models.Storages.Matrixes;

namespace VisualizationSystem.Services.Utilities;

public class DataNormalizer
{
    public DataMatrixManager NormalizeNodeParameters(List<NodeObject> nodes)
    {
        DataMatrixManager matrixManager = new DataMatrixManager();

        InitializeParameterRangesAndStringParameters(nodes, matrixManager);
        ProcessNodesForNormalization(nodes, matrixManager);

        return matrixManager;
    }

    private void InitializeParameterRangesAndStringParameters(List<NodeObject> nodes, DataMatrixManager matrixManager)
    {
        foreach (var node in nodes)
        {
            foreach (var parameter in node.Parameters)
            {
                if (IsNumeric(parameter.Value))
                {
                    AddToNumericRange(parameter, matrixManager);
                }
                else
                {
                    AddToStringParameterList(parameter, matrixManager);
                }
            }
        }
    }

    private void AddToNumericRange(NodeParameter parameter, DataMatrixManager matrixManager)
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

    private void AddToStringParameterList(NodeParameter parameter, DataMatrixManager matrixManager)
    {
        if (!matrixManager.StringParameters.ContainsKey(parameter.ParameterTypeId))
        {
            matrixManager.StringParameters[parameter.ParameterTypeId] = new List<string>();
        }

        if (!matrixManager.StringParameters[parameter.ParameterTypeId].Contains(parameter.Value))
        {
            matrixManager.StringParameters[parameter.ParameterTypeId].Add(parameter.Value);
        }
    }

    private void ProcessNodesForNormalization(List<NodeObject> nodes, DataMatrixManager matrixManager)
    {
        int rowIndex = 0;

        foreach (var node in nodes)
        {
            ProcessNodeForNormalization(node, rowIndex, matrixManager);
            rowIndex++;
        }
    }

    private void ProcessNodeForNormalization(NodeObject node, int rowIndex, DataMatrixManager matrixManager)
    {
        int numericColumnIndex = 0;
        int categoricalColumnIndex = 0;

        foreach (var parameter in node.Parameters)
        {
            if (IsNumeric(parameter.Value))
            {
                ProcessNumericParameter(parameter, rowIndex, numericColumnIndex, matrixManager);
                numericColumnIndex++;
            }
            else
            {
                ProcessCategoricalParameter(parameter, rowIndex, categoricalColumnIndex, matrixManager);
                categoricalColumnIndex++;
            }
        }
    }

    private void ProcessNumericParameter(NodeParameter node, int rowIndex, int numericColumnIndex, DataMatrixManager matrixManager)
    {
        double value = Convert.ToDouble(node.Value);

        if (matrixManager.ParameterRanges.ContainsKey(node.ParameterTypeId))
        {
            var range = matrixManager.ParameterRanges[node.ParameterTypeId];
            double normalizedValue = NormalizeMinMax(value, range.Min, range.Max);

            //TODO EnsureMatrixExists(node.ParameterTypeId,, node.Count, matrixManager);
            matrixManager.Matrices[node.ParameterTypeId].SetValue(rowIndex, numericColumnIndex, normalizedValue);
        }
    }

    private void ProcessCategoricalParameter(NodeParameter parameter, int rowIndex, int categoricalColumnIndex, DataMatrixManager matrixManager)
    {
        var possibleValues = matrixManager.StringParameters[parameter.ParameterTypeId];
        double[] encodedValue = ConvertStringToOneHot(parameter.Value, possibleValues);

        //TODO EnsureMatrixExists(parameter.ParameterTypeId, matrixManager.NodesCount, possibleValues.Count, matrixManager);

        for (int i = 0; i < encodedValue.Length; i++)
        {
            matrixManager.Matrices[parameter.ParameterTypeId].SetValue(rowIndex, categoricalColumnIndex++, encodedValue[i]);
        }
    }

    private void EnsureMatrixExists(int parameterTypeId, int rows, int columns, DataMatrixManager matrixManager)
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
