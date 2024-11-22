namespace VisualizationSystem.Models.Storages;

public class ColumnSelectionResult
{
    public int ColumnIndex { get; set; }
    public string ColumnName { get; set; }

    public ColumnSelectionResult(int columnIndex, string columnName)
    {
        ColumnIndex = columnIndex;
        ColumnName = columnName;
    }
}
