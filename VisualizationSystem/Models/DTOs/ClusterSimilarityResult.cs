namespace VisualizationSystem.Models.DTOs;

public class ClusterSimilarityResult
{
    public int FirstClusterId { get; set; }
    public int SecondClusterId { get; set; }
    public double Similarity { get; set; }

    public ClusterSimilarityResult()
    {
        Similarity = double.MaxValue;
    }

    public void Update(int firstClusterId, int secondClusterId, double similarity)
    {
        FirstClusterId = firstClusterId;
        SecondClusterId = secondClusterId;
        Similarity = similarity;
    }
}
