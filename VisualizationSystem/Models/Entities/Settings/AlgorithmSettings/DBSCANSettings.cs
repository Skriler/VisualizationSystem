using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities.Settings.AlgorithmSettings;

public class DBSCANSettings : ClusterAlgorithmSettings
{
    private const float DefaultEpsilon = 4;
    private const int DefaultMinPoints = 4;

    [Required] 
    public float Epsilon { get; set; }

    [Required] 
    public int MinPoints { get; set; }

    public DBSCANSettings()
    {
        Epsilon = DefaultEpsilon;
        MinPoints = DefaultMinPoints;
    }
}
