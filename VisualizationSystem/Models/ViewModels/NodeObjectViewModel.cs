using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.ViewModels;

public class NodeObjectViewModel
{
    public static List<string> Headers { get; private set; }

    public string Name { get; set; }

    public List<string> Parameters { get; private set; }

    static NodeObjectViewModel()
    {
        Headers = new List<string>();
    }

    public NodeObjectViewModel(NodeObject node)
    {
        Name = node.Name;
        Parameters = new List<string>();

        foreach (var parameter in node.Parameters)
        {
            Parameters.Add(parameter.Value);
            AddHeaders(parameter.Name);
        }
    }

    public static void AddHeaders(string header)
    {
        if (!Headers.Contains(header))
        {
            Headers.Add(header);
        }
    }
}
