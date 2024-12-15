using System.ComponentModel.DataAnnotations;
using VisualizationSystem.Services.Utilities.Clusterers;

namespace VisualizationSystem.Models.Entities.Settings.AlgorithmSettings;

public abstract class ClusterAlgorithmSettings
{
    [Key]
    public int Id { get; set; }

    [Required]
    public ClusterAlgorithm SelectedAlgorithm { get; set; }
}
