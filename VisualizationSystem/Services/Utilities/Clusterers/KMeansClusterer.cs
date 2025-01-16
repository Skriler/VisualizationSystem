using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.Factories;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class KMeansClusterer : BaseClusterer
{
    private readonly Random random = new();
    private List<KMeansCluster> kMeansClusters;

    protected override ClusterAlgorithm Algorithm => ClusterAlgorithm.KMeans;

    public KMeansClusterer(
        DataNormalizer dataNormalizer,
        DistanceCalculatorFactory distanceCalculatorFactory,
        ISettingsSubject settingsSubject
        )
        : base(dataNormalizer, distanceCalculatorFactory, settingsSubject)
    { }

    public override async Task<List<Cluster>> ClusterAsync(NodeTable nodeTable)
    {
        var nodes = await dataNormalizer.GetCalculationNodesAsync(nodeTable, settings.ParameterStates);
        var parametersCount = nodes.FirstOrDefault()?.Parameters.Count ?? 0;

        if (nodes.Count < settings.AlgorithmSettings.NumberOfClusters)
            throw new InvalidOperationException("Nodes amount is less than the number of clusters");

        kMeansClusters = new List<KMeansCluster>(settings.AlgorithmSettings.NumberOfClusters);
        InitializeClusters(nodes, parametersCount);

        for (int iteration = 0; iteration < settings.AlgorithmSettings.MaxIterations; ++iteration)
        {
            var assignmentsChanged = false;

            foreach (var kMeansCluster in kMeansClusters)
                kMeansCluster.Nodes.Clear();

            for (int i = 0; i < nodes.Count; ++i)
            {
                var clusterIndex = GetNearestClusterIndex(nodes[i]);

                if (kMeansClusters[clusterIndex].Nodes.Contains(nodes[i]))
                    continue;

                assignmentsChanged = true;
                kMeansClusters[clusterIndex].AddNode(nodes[i]);
            }

            if (!assignmentsChanged)
                break;

            RecalculateClusters(nodes);
        }

        return kMeansClusters.Cast<Cluster>().ToList();
    }

    private void InitializeClusters(List<CalculationNode> nodes, int parametersCount)
    {
        var selectedIndices = new HashSet<int>();

        for (int i = 0; i < settings.AlgorithmSettings.NumberOfClusters; ++i)
        {
            int randomIndex;
            do
            {
                randomIndex = random.Next(nodes.Count);
            } while (!selectedIndices.Add(randomIndex));

            var cluster = new KMeansCluster(nodes[randomIndex]);
            kMeansClusters.Add(cluster);
        }
    }

    private int GetNearestClusterIndex(CalculationNode node)
    {
        var clusterIndex = 0;
        var minDistance = double.MaxValue;
        
        for (int i = 0; i < kMeansClusters.Count; ++i)
        {
            var distance = distanceCalculator.Calculate(node, kMeansClusters[i].Centroid);

            if (distance >= minDistance)
                continue;

            minDistance = distance;
            clusterIndex = i;
        }

        return clusterIndex;
    }

    private void RecalculateClusters(List<CalculationNode> data)
    {
        foreach (var kMeansCluster in kMeansClusters)
        {
            kMeansCluster.RecalculateCentroid();
        }
    }
}
