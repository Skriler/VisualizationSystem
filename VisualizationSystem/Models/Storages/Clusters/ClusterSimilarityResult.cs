﻿namespace VisualizationSystem.Models.Storages.Clusters;

public class ClusterSimilarityResult
{
    public int FirstClusterId { get; set; }
    public int SecondClusterId { get; set; }
    public float Similarity { get; set; }

    public ClusterSimilarityResult()
    {
        Similarity = 0;
    }

    public void Update(int firstClusterId, int secondClusterId, float similarity)
    {
        FirstClusterId = firstClusterId;
        SecondClusterId = secondClusterId;
        Similarity = similarity;
    }
}
