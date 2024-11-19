namespace VisualizationSystem.Models.ViewModels;

public class NodeDetailsViewModel
{
    public string NodeName { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public List<(string SimilarNodeName, float SimilarityPercentage)> SimilarNodes { get; set; } = new();

    public NodeDetailsViewModel(string nodeName, Dictionary<string, object> parameters, List<(string, float)> similarNodes)
    {
        NodeName = nodeName;
        Parameters = parameters;
        SimilarNodes = similarNodes;
    }
}
