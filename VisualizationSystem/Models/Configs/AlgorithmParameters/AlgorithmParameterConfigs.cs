namespace VisualizationSystem.Models.Configs.AlgorithmParameters;

public static class AlgorithmParameterConfigs
{
    public static readonly AlgorithmParameterConfig AgglomerativeConfig = new()
    {
        FirstParameterLabel = "Threshold:",
        FirstParameterMin = 0,
        FirstParameterMax = 100,
        FirstParameterDecimalPlaces = 3,

        HasSecondParameter = false,
    };

    public static readonly AlgorithmParameterConfig KMeansConfig = new()
    {
        FirstParameterLabel = "Number of clusters:",
        FirstParameterMin = 2,
        FirstParameterMax = 100,
        FirstParameterDecimalPlaces = 0,

        SecondParameterLabel = "Max iterations:",
        SecondParameterMin = 10,
        SecondParameterMax = 1000,
        SecondParameterDecimalPlaces = 0,

        HasSecondParameter = true,
    };

    public static readonly AlgorithmParameterConfig DBSCANConfig = new()
    {
        FirstParameterLabel = "Epsilon:",
        FirstParameterMin = 0,
        FirstParameterMax = 20,
        FirstParameterDecimalPlaces = 3,

        SecondParameterLabel = "Min points:",
        SecondParameterMin = 1,
        SecondParameterMax = 1000,
        SecondParameterDecimalPlaces = 0,

        HasSecondParameter = true,
    };
}
