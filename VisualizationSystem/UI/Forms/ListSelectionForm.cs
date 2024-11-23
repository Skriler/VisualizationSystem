using Microsoft.IdentityModel.Tokens;
using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.UI.Forms;

public partial class ListSelectionForm : Form
{
    private const short DefaultBorderThickness = 4;
    private static readonly Color DefaultBorderColor = Color.Black;

    private readonly List<string> items;

    public  ListSelectionResult SelectedItem { get; private set; }

    public ListSelectionForm(string categoryName, List<string> items)
    {
        InitializeComponent();

        this.items = items;
        SelectedItem = new ListSelectionResult();

        lblSelectItem.Text = $"Select {categoryName}";
        InitializeComboBox();
    }

    private void ColumnSelectionForm_Paint(object sender, PaintEventArgs e)
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

    private void btnSubmit_Click(object sender, EventArgs e)
    {
        if (cmbItems.SelectedIndex < 0)
            throw new InvalidOperationException("A column must be selected");

        SelectedItem.SelectedIndex = cmbItems.SelectedIndex;
        SelectedItem.SelectedName = cmbItems.SelectedItem.ToString();

        DialogResult = DialogResult.OK;
        Close();
    }

    private void InitializeComboBox()
    {
        cmbItems.Items.Clear();

        foreach (var item in items)
        {
            cmbItems.Items.Add(item);
        }

        cmbItems.SelectedIndex = 0;
    }
}
