using VisualizationSystem.Models.Domain.Nodes.Parameters;

namespace VisualizationSystem.Models.Domain.Nodes;

public class Centroid : CalculationNode
{
    private const float MinThreshold = 0.5f;

    private readonly Dictionary<int, double[]> categoricalValues;

    public int MergeCount { get; private set; }

    public Centroid(CalculationNode node)
        : base(node)
    { }

    public void Merge(List<CalculationNode> nodes)
    {
        MergeCount = 1;

        var categoricalValues = new Dictionary<int, double[]>();

        var categoricals = Parameters
            .OfType<CategoricalParameter>()
            .ToList();

        foreach (var categorical in categoricals)
        {
            var paramValues = new double[categorical.CategoryCount];

            foreach (int index in categorical.OneHotIndexes)
            {
                paramValues[index] = 1;
            }

            categoricalValues[categorical.GetHashCode()] = paramValues;
        }

        foreach (var node in nodes)
        {
            Merge(node, categoricalValues);
        }

        foreach (var categorical in categoricals)
        {
            var mergedValues = categoricalValues[categorical.GetHashCode()];

            categorical.OneHotIndexes.Clear();

            for (int i = 0; i < mergedValues.Length; ++i)
            {
                if (mergedValues[i] < MinThreshold)
                    continue;

                categorical.OneHotIndexes.Add(i);
            }
        }
    }

    public void Merge(CalculationNode node, Dictionary<int, double[]> categoricalValues)
    {
        for (int i = 0; i < Parameters.Count; ++i)
        {
            if (Parameters[i] is NumericParameter numericParam &&
                node.Parameters[i] is NumericParameter numericMergeParam)
            {
                MergeNumeric(numericParam, numericMergeParam);
            }
            else if (Parameters[i] is CategoricalParameter categoricalParam &&
                node.Parameters[i] is CategoricalParameter categoricalMergeParam)
            {
                MergeCategorical(categoricalParam, categoricalMergeParam, categoricalValues);
            }
        }

        ++MergeCount;
    }

    private void MergeNumeric(NumericParameter param, NumericParameter mergeParam)
    {
        param.Value = CalculateAverage(param.Value, mergeParam.Value);
    }

    private void MergeCategorical(CategoricalParameter param, CategoricalParameter mergeParam, Dictionary<int, double[]> categoricalValues)
    {
        var paramValues = categoricalValues[param.GetHashCode()];

        var mergeValues = new double[paramValues.Length];
        foreach (int index in mergeParam.OneHotIndexes)
        {
            mergeValues[index] = 1;
        }

        var averagedValues = CalculateCategoricalAverage(paramValues, mergeValues);

        categoricalValues[param.GetHashCode()] = averagedValues;
    }

    private double CalculateAverage(double value, double mergeValue)
    {
        var weightedSum = value * MergeCount + mergeValue;
        var totalCount = MergeCount + 1;
        return weightedSum / totalCount;
    }

    private double[] CalculateCategoricalAverage(double[] values, double[] mergeValues)
    {
        var newValues = new double[values.Length];

        for (int i = 0; i < values.Length; ++i)
        {
            newValues[i] = CalculateAverage(values[i], mergeValues[i]);
        }

        return newValues;
    }
}
