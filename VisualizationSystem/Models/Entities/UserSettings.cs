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

    public List<ParameterType> ParameterStates { get; set; } = new List<ParameterType>();

    public UserSettings()
    {
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

    public void InitializeParameterStates(NodeTable nodeTable)
    {
        NodeTable = nodeTable;
        ParameterStates = nodeTable.ParameterTypes;
    }

    public List<ParameterType> GetActiveParameters()
    {
        return ParameterStates.Where(p => p.IsActive).ToList();
    }
}
