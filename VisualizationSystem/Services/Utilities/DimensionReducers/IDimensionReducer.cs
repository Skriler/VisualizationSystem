using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Domain.PCA;

namespace VisualizationSystem.Services.Utilities.DimensionReducers;

public interface IDimensionReducer
{
    List<ReducedDataPoint> ReduceDimension(IEnumerable<Cluster> clusters);
}
