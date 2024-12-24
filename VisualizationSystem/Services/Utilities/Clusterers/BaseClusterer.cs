using VisualizationSystem.Models.Entities.Nodes.Normalized;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.DAL;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Domain.Clusters;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public abstract class BaseClusterer
{
    private readonly NormalizedNodeRepository normalizedNodeRepository;
    private readonly DataNormalizer dataNormalizer;

    public ClusterAlgorithmSettings AlgorithmSettings { get; set; }

    protected BaseClusterer(NormalizedNodeRepository normalizedNodeRepository, DataNormalizer dataNormalizer)
    {
        this.normalizedNodeRepository = normalizedNodeRepository;
        this.dataNormalizer = dataNormalizer;
    }

    public abstract Task<List<Cluster>> ClusterAsync(NodeTable nodeTable);

    public void UpdateSettings(ClusterAlgorithmSettings algorithmSettings) => AlgorithmSettings = algorithmSettings;

    protected async Task<List<NormalizedNode>> GeNormalizedNodesAsync(NodeTable nodeTable)
    {
        if (await normalizedNodeRepository.ExistsAsync(nodeTable.Name))
        {
            return await normalizedNodeRepository.GetByTableNameAsync(nodeTable.Name);
        }

        var normalizedNodes = dataNormalizer.GetNormalizedNodes(nodeTable.NodeObjects);
        normalizedNodes.ForEach(nn => nn.NodeTable = nodeTable);

        await normalizedNodeRepository.AddRangeAsync(normalizedNodes);
        return normalizedNodes;
    }
}
