using VisualizationSystem.Models.Domain.Nodes.Parameters;

namespace VisualizationSystem.Models.Domain.Nodes;

public class Centroid : CalculationNode
{
    private const float MinThreshold = 0.5f;

    private Dictionary<int, double[]> categoricalValues = new();

    public int MergeCount { get; private set; }

    public Centroid(CalculationNode node)
        : base(node)
    { }

    public void Merge(List<CalculationNode> nodes)
    {
        MergeCount = 1;
        categoricalValues = InitializeCategoricalValues();

        foreach (var node in nodes)
        {
            MergeNode(node);
        }

        UpdateCategoricalParameters();
    }

    private Dictionary<int, double[]> InitializeCategoricalValues()
    {
        return Parameters.OfType<CategoricalParameter>()
            .ToDictionary(p => p.GetHashCode(), CreateOneHotVector);
    }

    private double[] CreateOneHotVector(CategoricalParameter parameter)
    {
        var values = new double[parameter.CategoryCount];

        foreach (int index in parameter.OneHotIndexes)
        {
            values[index] = 1;
        }

        return values;
    }

    private void MergeNode(CalculationNode node)
    {
        for (int i = 0; i < Parameters.Count; i++)
        {
            MergeParameter(Parameters[i], node.Parameters[i]);
        }

        MergeCount++;
    }

    private void MergeParameter(BaseParameter baseParam, BaseParameter mergeParam)
    {
        switch (baseParam)
        {
            case NumericParameter numericBase when mergeParam is NumericParameter numericMerge:
                MergeNumericParameter(numericBase, numericMerge);
                break;
            case CategoricalParameter categoricalBase when mergeParam is CategoricalParameter categoricalMerge:
                MergeCategoricalParameter(categoricalBase, categoricalMerge);
                break;
        }
    }


    private void MergeNumericParameter(NumericParameter param, NumericParameter mergeParam)
    {
        param.Value = CalculateAverage(param.Value, mergeParam.Value);
    }

    private void MergeCategoricalParameter(CategoricalParameter param, CategoricalParameter mergeParam)
    {
        var currentValues = categoricalValues[param.GetHashCode()];
        var mergeValues = CreateOneHotVector(mergeParam);

        categoricalValues[param.GetHashCode()] =
            CalculateCategoricalAverage(currentValues, mergeValues);
    }

    private double CalculateAverage(double value, double mergeValue)
    {
        var weightedSum = (value * MergeCount) + mergeValue;
        var totalCount = MergeCount + 1;
        return weightedSum / totalCount;
    }

    private double[] CalculateCategoricalAverage(double[] values, double[] mergeValues)
    {
        return values
            .Zip(mergeValues, CalculateAverage)
            .ToArray();
    }

    private void UpdateCategoricalParameters()
    {
        foreach (var parameter in Parameters.OfType<CategoricalParameter>())
        {
            UpdateParameterOneHotIndexes(parameter);
        }
    }

    private void UpdateParameterOneHotIndexes(CategoricalParameter parameter)
    {
        var mergedValues = categoricalValues[parameter.GetHashCode()];
        parameter.OneHotIndexes.Clear();

        for (int i = 0; i < mergedValues.Length; i++)
        {
            if (mergedValues[i] < MinThreshold)
                continue;

            parameter.OneHotIndexes.Add(i);
        }
    }
}
