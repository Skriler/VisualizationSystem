using VisualizationSystem.Models.Storages;
using VisualizationSystem.Models.Storages.Matrixes;

namespace VisualizationSystem.Services.Utilities;

public class DataMatrixManager
{
    public Dictionary<int, ParameterRange> ParameterRanges { get; private set; } = new();

    public Dictionary<int, List<string>> StringParameters { get; private set; } = new();

    public Dictionary<int, DataMatrix> Matrices { get; set; } = new();

    public void AddMatrix(int parameterTypeId, DataMatrix matrix)
    {
        Matrices[parameterTypeId] = matrix;
    }

    public void Clear()
    {
        ParameterRanges.Clear();
        StringParameters.Clear();
        Matrices.Clear();
    }

    public int GetTotalColumns() => Matrices.Values.Sum(matrix => matrix.Columns);
}
