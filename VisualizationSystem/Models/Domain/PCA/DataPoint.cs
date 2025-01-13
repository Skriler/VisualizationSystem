using Microsoft.ML.Data;

namespace VisualizationSystem.Models.Domain.PCA;

public class DataPoint
{
    [VectorType(161)]
    public float[] Features { get; set; } = default!;

    public string Name { get; set; } = string.Empty;

    public int ClusterId { get; set; }
}
