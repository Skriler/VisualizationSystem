namespace VisualizationSystem.Models.Domain.Nodes.Parameters;

public abstract class BaseParameter
{
    public int MergeCount { get; protected set; }

    protected BaseParameter(int mergeCount)
    {
        MergeCount = mergeCount;
    }

    public abstract BaseParameter Clone();

    public abstract BaseParameter Merge(BaseParameter other);
}
