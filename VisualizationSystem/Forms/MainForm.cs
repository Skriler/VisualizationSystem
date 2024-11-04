using Microsoft.IdentityModel.Tokens;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.ViewModels;
using VisualizationSystem.SL;
using VisualizationSystem.SL.DAL;

namespace VisualizationSystem.Forms
{
    public partial class MainForm : Form
    {
        private static readonly string FileDialogTitle = "Select an Excel File";
        private static readonly string InitialDirectory = "D:\\";
        private static readonly string ExcelFilterPattern
            = "Excel Workbook(*.xlsx)|*.xlsx|Excel 97- Excel 2003 Workbook(*.xls)|*.xls";

        private readonly VisualizationSystemDbContext db;
        private readonly NodeRepository nodeRepository;

        private List<NodeObject> nodes = new List<NodeObject>();

        public MainForm(VisualizationSystemDbContext context)
        {
            InitializeComponent();

            db = context;
            nodeRepository = new NodeRepository(db);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            nodes = await nodeRepository.GetAllAsync();
        }

        private async void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadExcelData();
            await SaveDataToDatabaseAsync();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nodes.IsNullOrEmpty())
                return;

            var nodeViewModels = nodes
                .Select(n => new NodeObjectViewModel(n))
                .ToList();

            dataGridViewNodes.Visible = true;
            dataGridViewNodes.DataSource = nodeViewModels;

            //dataGridViewNodes.AutoGenerateColumns = false;
            //dataGridViewNodes.Columns.Clear();

            //dataGridViewNodes.Columns.Add(new DataGridViewTextBoxColumn
            //{
            //    HeaderText = "Node Name",
            //    DataPropertyName = "Name"
            //});

            //for (int i = 0; i < NodeObjectViewModel.Headers.Count; ++i)
            //{
            //    dataGridViewNodes.Columns.Add(new DataGridViewTextBoxColumn
            //    {
            //        HeaderText = NodeObjectViewModel.Headers[i],
            //        DataPropertyName = $"ParameterValues[{i}]"
            //    });
            //}

            //dataGridViewNodes.DataSource = nodeViewModels;
        }

        private void LoadExcelData()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                InitializeFileDialogParameters(openFileDialog);

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                string filePath = openFileDialog.FileName;

                try
                {
                    nodes = ExcelReader.ReadFile(filePath);
                    MessageBox.Show("Data read successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InitializeFileDialogParameters(OpenFileDialog openFileDialog)
        {
            openFileDialog.Title = FileDialogTitle;
            openFileDialog.InitialDirectory = InitialDirectory;
            openFileDialog.Filter = ExcelFilterPattern;
            openFileDialog.RestoreDirectory = true;
        }

        private async Task SaveDataToDatabaseAsync()
        {
            try
            {
                await nodeRepository.AddListAsync(nodes);
                MessageBox.Show("Data saved to database successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while saving data to database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
