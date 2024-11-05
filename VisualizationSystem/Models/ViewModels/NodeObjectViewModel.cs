using Microsoft.IdentityModel.Tokens;
using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.ViewModels;

public class NodeObjectViewModel
{
    public static List<string> Headers { get; private set; } = new List<string>();

    public List<string> Parameters { get; private set; } = new List<string>();

    public NodeObjectViewModel(NodeObject node)
    {
        Parameters = GetParameters(node);
    }

    public static void InitializeHeaders(IEnumerable<string> headers)
    {
        Headers.Clear();
        Headers.Add("Name"); // TODO

        foreach (var header in headers)
        {
            Headers.Add(header);
        }
    }

    private List<string> GetParameters(NodeObject node)
    {
        if (node == null || node.Parameters.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(node));

        var parameters = new List<string>
        {
            node.Name ?? string.Empty
        };

        foreach (var parameter in node.Parameters)
        {
            parameters.Add(parameter.Value ?? string.Empty);
        }

        return parameters;
    }
}
