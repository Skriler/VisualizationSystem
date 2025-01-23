namespace VisualizationSystem.Services.Utilities.Comparers;

public class DefaultComparer : ICompare
{
    public bool Compare(string firstValue, string secondValue, float deviationPercent)
    {
        if (double.TryParse(firstValue, out var firstNumber) &&
            double.TryParse(secondValue, out var secondNumber))
        {
            return CompareNumerical(firstNumber, secondNumber, deviationPercent);
        }

        return CompareStrings(firstValue, secondValue);
    }

    private static bool CompareNumerical(double firstNumber, double secondNumber, float deviationPercent)
    {
        var maxNumber = Math.Max(firstNumber, secondNumber);
        var tolerance = maxNumber * deviationPercent / 100;

        return Math.Abs(firstNumber - secondNumber) <= tolerance;
    }

    private static bool CompareStrings(string firstValue, string secondValue)
    {
        if (string.IsNullOrWhiteSpace(firstValue) || string.IsNullOrWhiteSpace(secondValue))
            return false;

        var firstValues = GetSplitValues(firstValue);
        var secondValues = GetSplitValues(secondValue);

        return firstValues.SequenceEqual(secondValues);
    }

    private static string[] GetSplitValues(string values)
    {
        return values.Split(',')
            .Select(v => v.Trim())
            .Order()
            .ToArray();
    }
}
