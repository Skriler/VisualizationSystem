using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities;

public class UserSettings
{
    private static readonly float DefaultMinSimilarityPercentage = 55f;
    private static readonly float DefaultDeviationPercent = 10f;

    [Key]
    public int Id { get; set; }

    [Required]
    public float MinSimilarityPercentage { get; set; }

    [Required]
    public float DeviationPercent { get; set; }

    [Required]
    public int NodeTableId { get; set; }
    public NodeTable NodeTable { get; set; } = default!;

    public List<ParameterState> ParameterStates { get; set; } = new List<ParameterState>();

    public UserSettings()
    {
        ResetCoreValues();
    }

    public UserSettings(NodeTable nodeTable)
    {
        NodeTable = nodeTable;
        ParameterStates = nodeTable.ParameterTypes
            .Select(p => new ParameterState(p, this))
            .ToList();

        ResetCoreValues();
    }

    public void ResetToDefaults()
    {
        ResetCoreValues();
        ResetParameterStates();
    }

    public void ResetCoreValues()
    {
        MinSimilarityPercentage = DefaultMinSimilarityPercentage;
        DeviationPercent = DefaultDeviationPercent;
    }

    public void ResetParameterStates()
    {
        ParameterStates.ForEach(p => p.ResetToDefaults());
    }

    public List<ParameterState> GetActiveParameters()
    {
        return ParameterStates.Where(p => p.IsActive).ToList();
    }
}
