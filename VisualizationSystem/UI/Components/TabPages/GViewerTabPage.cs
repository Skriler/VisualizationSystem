using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;

namespace VisualizationSystem.UI.Components.TabPages;

public sealed class ClosableGViewerTabPage : ClosableTabPageBase
{
    private readonly Action<string> onNodeClick;
    private GViewer gViewer;

    public Graph DisplayedGraph { get; set; }

    public ClosableGViewerTabPage(string text, Graph graph, Action<string> onNodeClick) 
        : base(text)
    {
        DisplayedGraph = graph;
        this.onNodeClick = onNodeClick;

        InitializeContent();
    }

    protected override void InitializeContent()
    {
        gViewer = new GViewer
        {
            Dock = DockStyle.Fill,
            Graph = DisplayedGraph,
        };

        gViewer.MouseClick += HandleGViewerNodeClick;
        Controls.Add(gViewer);
    }

    public override void UpdateContent(object newData)
    {
        if (newData is not Graph newGraph)
            throw new ArgumentException("Invalid input data");

        DisplayedGraph = newGraph;
        gViewer.Graph = DisplayedGraph;
        gViewer.Refresh();
    }

    private void HandleGViewerNodeClick(object? sender, MouseEventArgs e)
    {
        if (sender is not GViewer viewer)
            return;

        var clickedObject = viewer.GetObjectAt(e.Location);

        if (clickedObject is not DNode clickedNode)
            return;

        onNodeClick?.Invoke(clickedNode.Node.Id);
    }
}
