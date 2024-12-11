using Microsoft.Identity.Client;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.Models.Storages.Matrixes;

namespace VisualizationSystem.Services.Utilities;

public class DataNormalizer
{
    private readonly DataMatrixManager matrixManager;

    private List<NodeObject> nodes;

    public DataNormalizer()
    {
        matrixManager = new DataMatrixManager();
    }

    public List<NormalizedNodeData> GetNormalizedData(List<NodeObject> nodes)
    {
        matrixManager.Clear();
        this.nodes = nodes;

        InitializeParameterRangesAndStringParameters();
        ProcessNodesForNormalization();

        var normalizedMatrix = CombineMatrices();

        return normalizedMatrix;
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
        if (matrixManager.StringParameters.ContainsKey(parameter.ParameterTypeId))
            throw new InvalidOperationException($"Conflict detected: string data already exists for {parameter.ParameterTypeId}");

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

        if (matrixManager.ParameterRanges.ContainsKey(parameter.ParameterTypeId))
            throw new InvalidOperationException($"Conflict detected: numeric range already exists for {parameter.ParameterTypeId}");

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
        foreach (var parameter in node.Parameters)
        {
            if (IsNumeric(parameter.Value))
            {
                ProcessNumericParameter(parameter, rowIndex);
            }
            else
            {
                ProcessCategoricalParameter(parameter, rowIndex);
            }
        }
    }

    private void ProcessNumericParameter(NodeParameter parameter, int rowIndex)
    {
        if (!matrixManager.ParameterRanges.TryGetValue(parameter.ParameterTypeId, out var range))
            return;

        var value = Convert.ToDouble(parameter.Value);
        var normalizedValue = NormalizeMinMax(value, range.Min, range.Max);

        EnsureMatrixExists(parameter.ParameterTypeId, nodes.Count, 1);
        matrixManager.Matrices[parameter.ParameterTypeId].SetValue(rowIndex, 0, normalizedValue);
    }

    private void ProcessCategoricalParameter(NodeParameter parameter, int rowIndex)
    {
        if (string.IsNullOrWhiteSpace(parameter.Value))
            return;

        if (!matrixManager.StringParameters.TryGetValue(parameter.ParameterTypeId, out var possibleValues))
            return;

        var columnIndex = GetOneHotIndex(parameter.Value, possibleValues);

        EnsureMatrixExists(parameter.ParameterTypeId, nodes.Count, possibleValues.Count);
        matrixManager.Matrices[parameter.ParameterTypeId].SetValue(rowIndex, columnIndex, 1);
    }

    private void EnsureMatrixExists(int parameterTypeId, int rows, int columns)
    {
        if (matrixManager.Matrices.ContainsKey(parameterTypeId))
            return;

        matrixManager.AddMatrix(parameterTypeId, new DataMatrix(rows, columns));
    }

    private List<NormalizedNodeData> CombineMatrices()
    {
        var normalizedDataList = new List<NormalizedNodeData>();

        int currentColumnOffset = 0;
        foreach (var node in nodes)
        {
            var normalizedNodeData = CreateNormalizedNodeData(node, currentColumnOffset);

            currentColumnOffset += matrixManager.GetTotalColumns();
            normalizedDataList.Add(normalizedNodeData);
        }

        return normalizedDataList;
    }

    private NormalizedNodeData CreateNormalizedNodeData(NodeObject node, int currentColumnOffset)
    {
        var normalizedNodeData = new NormalizedNodeData(node, new double[matrixManager.GetTotalColumns()]);

        foreach (var matrix in matrixManager.Matrices.Values)
        {
            CopyMatrixToNormalizedData(matrix, normalizedNodeData.NormalizedParameters, currentColumnOffset);

            currentColumnOffset += matrix.Columns;
        }

        return normalizedNodeData;
    }

    private void CopyMatrixToNormalizedData(DataMatrix matrix, double[] normalizedParameters, int columnOffset)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                normalizedParameters[columnOffset + j] = matrix.Matrix[i, j];
            }
        }
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

    private int GetOneHotIndex(string value, List<string> possibleValues)
    {
        return possibleValues.IndexOf(value);
    }
}
