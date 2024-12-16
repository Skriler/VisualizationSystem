namespace VisualizationSystem.Models.Storages.Configs;

public class AlgorithmParameterConfig
{
    public string FirstParameterLabel { get; set; } = default!;
    public decimal FirstParameterMin { get; set; }
    public decimal FirstParameterMax { get; set; }
    public bool IsFirstParameterInt { get; set; }

    public string SecondParameterLabel { get; set; } = default!;
    public decimal SecondParameterMin { get; set; }
    public decimal SecondParameterMax { get; set; }
    public bool IsSecondParameterInt { get; set; }

    public bool HasSecondParameter { get; set; }
}
