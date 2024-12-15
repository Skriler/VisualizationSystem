namespace VisualizationSystem.Models.Storages.Configs;

public class AlgorithmParameterConfigs
{
    public static readonly AlgorithmParameterConfig AgglomerativeConfig = new AlgorithmParameterConfig
    {
        FirstParameterLabel = "Threshold:",
        FirstParameterMin = 0,
        FirstParameterMax = 100,
        HasSecondParameter = false,
    };

    public static readonly AlgorithmParameterConfig KMeansConfig = new AlgorithmParameterConfig
    {
        FirstParameterLabel = "Number of clusters:",
        FirstParameterMin = 2,
        FirstParameterMax = 100,

        SecondParameterLabel = "Max iterations:",
        SecondParameterMin = 10,
        SecondParameterMax = 1000,
    };

    public static readonly AlgorithmParameterConfig DBSCANConfig = new AlgorithmParameterConfig
    {
        FirstParameterLabel = "Epsilon:",
        FirstParameterMin = 0,
        FirstParameterMax = 20,

        SecondParameterLabel = "Min points:",
        SecondParameterMin = 1,
        SecondParameterMax = 1000,
    };
}
