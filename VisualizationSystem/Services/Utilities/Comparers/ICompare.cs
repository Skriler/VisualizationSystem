namespace VisualizationSystem.Services.Utilities.Comparers;

public interface ICompare
{
    bool Compare(string firstValue, string secondValue, float tolerance);
}
