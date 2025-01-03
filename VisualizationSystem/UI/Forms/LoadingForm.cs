﻿namespace VisualizationSystem.UI.Forms;

public partial class LoadingForm : Form
{
    private const short DefaultBorderThickness = 4;
    private static readonly Color DefaultBorderColor = Color.Black;

    public LoadingForm(string loadingMessage)
    {
        InitializeComponent();

        lblMessage.Text = loadingMessage;
    }

    private void LoadingForm_Paint(object sender, PaintEventArgs e)
    {
        using var pen = new Pen(DefaultBorderColor, DefaultBorderThickness);

        e.Graphics.DrawRectangle(
            pen,
            0,
            0,
            ClientSize.Width - 1,
            ClientSize.Height - 1
        );
    }
}
