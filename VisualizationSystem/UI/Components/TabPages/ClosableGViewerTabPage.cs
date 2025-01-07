using Microsoft.Msagl.GraphViewerGdi;
using VisualizationSystem.Models.Domain.Graphs;
using VisualizationSystem.UI.Forms;

namespace VisualizationSystem.UI.Components.TabPages;

public sealed class ClosableGViewerTabPage : ClosableTabPageBase
{
    private readonly GViewer gViewer;

    public ExtendedGraph DisplayedGraph { get; set; }

    public ClosableGViewerTabPage(string text, ExtendedGraph graph)
        : base(text)
    {
        gViewer = new GViewer();
        DisplayedGraph = graph;

        InitializeContent();
    }

    protected override void InitializeContent()
    {
        gViewer.Dock = DockStyle.Fill;
        gViewer.Graph = DisplayedGraph;

        gViewer.DoubleClick += HandleGViewerNodeDoubleClick;

        Controls.Add(gViewer);
    }

    public override void UpdateContent(object newData)
    {
        if (newData is not ExtendedGraph newGraph)
            throw new ArgumentException("Invalid input data");

        DisplayedGraph = newGraph;
        gViewer.Graph = DisplayedGraph;
        gViewer.Refresh();
    }

    private void HandleGViewerNodeDoubleClick(object? sender, EventArgs e)
    {
        if (sender is not GViewer viewer || e is not MouseEventArgs mouseEventArgs)
            return;

        var clickedObject = viewer.GetObjectAt(mouseEventArgs.Location);

        if (clickedObject is not DNode clickedNode)
            return;

        OpenNodeDetailsForm(clickedNode.Node.Id);
    }

    private void OpenNodeDetailsForm(string nodeName)
    {
        var similarityResult = DisplayedGraph.NodeDataMap[nodeName];

        if (similarityResult == null)
            return;

        var detailsForm = new NodeDetailsForm(similarityResult, OpenNodeDetailsForm);
        detailsForm.Show();
        detailsForm.BringToFront();
    }
}
