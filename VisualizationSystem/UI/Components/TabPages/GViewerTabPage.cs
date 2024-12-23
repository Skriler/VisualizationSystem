using Microsoft.Msagl.GraphViewerGdi;
using VisualizationSystem.Models.Storages.Graphs;
using VisualizationSystem.Models.Storages.Results;

namespace VisualizationSystem.UI.Components.TabPages;

public sealed class ClosableGViewerTabPage : ClosableTabPageBase
{
    private readonly Action<NodeSimilarityResult> onNodeClick;
    private readonly GViewer gViewer;

    public ExtendedGraph DisplayedGraph { get; set; }

    public ClosableGViewerTabPage(string text, ExtendedGraph graph, Action<NodeSimilarityResult> onNodeClick) 
        : base(text)
    {
        gViewer = new GViewer();

        DisplayedGraph = graph;
        this.onNodeClick = onNodeClick;

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

        var similarityResult = DisplayedGraph.NodeDataMap[clickedNode.Node.Id];

        onNodeClick?.Invoke(similarityResult);
    }
}
