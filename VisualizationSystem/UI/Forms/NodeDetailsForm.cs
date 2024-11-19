using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.UI.Forms;

public partial class NodeDetailsForm : Form
{
    public NodeDetailsForm(NodeObject node)
    {
        InitializeComponent();

        Text = $"Details for {node.Name}";
    }
}