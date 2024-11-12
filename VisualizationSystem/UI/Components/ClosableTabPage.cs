namespace VisualizationSystem.UI.Components;

public class ClosableTabPage : TabPage
{
    private readonly int CloseIconSize = 15;

    public ClosableTabPage(string text) : base(text)
    { }

    public void DrawTab(Rectangle tabRect, Graphics g)
    {
        TextRenderer.DrawText(
            g, 
            Text, 
            Font,
            tabRect,
            ForeColor
        );

        var closeIconRect = GetCloseIconRect(tabRect);

        g.DrawString(
            "x", 
            Font, 
            Brushes.Black, 
            closeIconRect.Location
        );
    }

    public bool IsCloseIconClicked(Rectangle tabRect, Point clickLocation)
    {
        var closeIconRect = GetCloseIconRect(tabRect);

        return closeIconRect.Contains(clickLocation);
    }

    private Rectangle GetCloseIconRect(Rectangle tabRect)
    {
        return new Rectangle(
            tabRect.Right - CloseIconSize, 
            tabRect.Top + (tabRect.Height - CloseIconSize) / 2, 
            CloseIconSize, 
            CloseIconSize
        );
    }
}