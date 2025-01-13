using Microsoft.ML.Data;

namespace VisualizationSystem.Models.Domain.PCA;

public class ReducedDataPoint
{
    [VectorType(2)]
    [ColumnName("PCAFeatures")]
    public float[] Features { get; set; } = default!;

    public float X => Features[0];

    public float Y => Features[1];

    public string Name { get; set; } = string.Empty;

    public int ClusterId { get; set; }
}
