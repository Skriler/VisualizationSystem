namespace VisualizationSystem.Models.DTOs;

public class ListSelectionResult
{
    public int SelectedIndex { get; set; }
    public string SelectedName { get; set; }

    public ListSelectionResult(int selectedIndex = -1, string selectedName = "")
    {
        SelectedIndex = selectedIndex;
        SelectedName = selectedName;
    }
}
