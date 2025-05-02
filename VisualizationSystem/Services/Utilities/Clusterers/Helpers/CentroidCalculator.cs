using VisualizationSystem.Models.Domain.Nodes.Parameters;
using VisualizationSystem.Models.Domain.Nodes;

namespace VisualizationSystem.Services.Utilities.Clusterers.Helpers;

public class CentroidCalculator
{
    /// <summary>
    /// Threshold used to convert averaged one-hot values to 1 or 0.
    /// If the average is greater than or equal to the threshold, set to 1; otherwise, 0.
    /// </summary>
    private const float OneHotThreshold = 0.5f;

    /// <summary>
    /// Stores the cumulative sum of numeric parameter values by index.
    /// </summary>
    private readonly Dictionary<int, double> numericSums = new();

    /// <summary>
    /// Stores the cumulative sum of categorical one-hot encoded values by index.
    /// </summary>
    private readonly Dictionary<int, int[]> categoricalSums = new();

    /// <summary>
    /// Recalculates the centroid by averaging its parameters with the provided merge nodes.
    /// </summary>
    public CalculationNode Recalculate(CalculationNode centroid, List<CalculationNode> mergeNodes)
    {
        if (mergeNodes.Count == 0)
            return centroid;

        Validate(centroid, mergeNodes);

        ExtractValues(centroid);
        CalculateValuesSums(mergeNodes);
        var newValues = ApplyAverages(centroid, mergeNodes.Count + 1);

        return new CalculationNode(newValues);
    }

    /// <summary>
    /// Validates that all nodes have the same number of parameters as the centroid.
    /// </summary>
    private void Validate(CalculationNode centroid, List<CalculationNode> mergeNodes)
    {
        if (mergeNodes.Any(obj => obj.Parameters.Count != centroid.Parameters.Count))
            throw new ArgumentException("Object values count doesn't match centroid's");
    }

    /// <summary>
    /// Extracts and stores the initial values from the centroid for further averaging.
    /// </summary>
    private void ExtractValues(CalculationNode centroid)
    {
        numericSums.Clear();
        categoricalSums.Clear();

        for (int i = 0; i < centroid.Parameters.Count; i++)
        {
            var value = centroid.Parameters[i];

            switch (value)
            {
                case NumericParameter numeric:
                    numericSums[i] = numeric.Value;
                    break;

                case CategoricalParameter categorical:
                    categoricalSums[i] = (int[])categorical.OneHotValues.Clone();
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported value type at index {i}: {value?.GetType().Name}");
            }
        }
    }

    /// <summary>
    /// Calculates cumulative sums of parameter values from the merge nodes.
    /// </summary>
    private void CalculateValuesSums(List<CalculationNode> mergeNodes)
    {
        foreach (var mergeNode in mergeNodes)
        {
            for (int i = 0; i < mergeNode.Parameters.Count; ++i)
            {
                switch (mergeNode.Parameters[i])
                {
                    case NumericParameter numericParam when numericSums.ContainsKey(i):
                        numericSums[i] += numericParam.Value;
                        break;

                    case CategoricalParameter categoricalParam when categoricalSums.ContainsKey(i):
                        AccumulateCategoricalValues(categoricalSums[i], categoricalParam.OneHotValues);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Applies averaging to the accumulated values and constructs new parameters for the centroid.
    /// </summary>
    private List<BaseParameter> ApplyAverages(CalculationNode centroid, int mergedObjectsCount)
    {
        var newParameterValues = new BaseParameter[centroid.Parameters.Count];

        foreach (var pair in numericSums)
        {
            var newValue = pair.Value / mergedObjectsCount;

            newParameterValues[pair.Key] = new NumericParameter(newValue);
        }

        foreach (var pair in categoricalSums)
        {
            var newOneHot = new int[pair.Value.Length];

            for (int j = 0; j < newOneHot.Length; j++)
            {
                var avgValue = (double)pair.Value[j] / mergedObjectsCount;

                newOneHot[j] = avgValue >= OneHotThreshold ? 1 : 0;
            }

            newParameterValues[pair.Key] = new CategoricalParameter(newOneHot);
        }

        return newParameterValues.ToList();
    }

    /// <summary>
    /// Adds the values from a categorical parameter to the corresponding cumulative sum array.
    /// </summary>
    private static void AccumulateCategoricalValues(int[] sumArray, int[] values)
    {
        for (int j = 0; j < values.Length; j++)
        {
            sumArray[j] += values[j];
        }
    }
}
