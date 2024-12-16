namespace VisualizationSystem.Models.Storages.Configs;

public class AlgorithmParameterConfigs
{
    public static readonly AlgorithmParameterConfig AgglomerativeConfig = new()
    {
        FirstParameterLabel = "Threshold:",
        FirstParameterMin = 0,
        FirstParameterMax = 100,
        IsFirstParameterInt = false,

        HasSecondParameter = false,
    };

    public static readonly AlgorithmParameterConfig KMeansConfig = new()
    {
        FirstParameterLabel = "Number of clusters:",
        FirstParameterMin = 2,
        FirstParameterMax = 100,
        IsFirstParameterInt = true,

        SecondParameterLabel = "Max iterations:",
        SecondParameterMin = 10,
        SecondParameterMax = 1000,
        IsSecondParameterInt = true,

        HasSecondParameter = true,
    };

    public static readonly AlgorithmParameterConfig DBSCANConfig = new()
    {
        FirstParameterLabel = "Epsilon:",
        FirstParameterMin = 0,
        FirstParameterMax = 20,
        IsFirstParameterInt = false,

        SecondParameterLabel = "Min points:",
        SecondParameterMin = 1,
        SecondParameterMax = 1000,
        IsSecondParameterInt = true,

        HasSecondParameter = true,
    };
}
