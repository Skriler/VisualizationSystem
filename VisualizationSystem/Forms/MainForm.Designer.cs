namespace VisualizationSystem.Forms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip = new MenuStrip();
            dataToolStripMenuItem = new ToolStripMenuItem();
            loadToolStripMenuItem = new ToolStripMenuItem();
            showToolStripMenuItem = new ToolStripMenuItem();
            visualizationToolStripMenuItem = new ToolStripMenuItem();
            buildGraphToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            dataGridViewNodes = new DataGridView();
            gViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            tabControl = new TabControl();
            tabPageDataGridView = new TabPage();
            tabPageGViewer = new TabPage();
            menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewNodes).BeginInit();
            tabControl.SuspendLayout();
            tabPageDataGridView.SuspendLayout();
            tabPageGViewer.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { dataToolStripMenuItem, visualizationToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(800, 24);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip1";
            // 
            // dataToolStripMenuItem
            // 
            dataToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadToolStripMenuItem, showToolStripMenuItem });
            dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            dataToolStripMenuItem.Size = new Size(43, 20);
            dataToolStripMenuItem.Text = "Data";
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new Size(180, 22);
            loadToolStripMenuItem.Text = "Load";
            loadToolStripMenuItem.Click += loadToolStripMenuItem_Click;
            // 
            // showToolStripMenuItem
            // 
            showToolStripMenuItem.Name = "showToolStripMenuItem";
            showToolStripMenuItem.Size = new Size(180, 22);
            showToolStripMenuItem.Text = "Show";
            showToolStripMenuItem.Click += showToolStripMenuItem_Click;
            // 
            // visualizationToolStripMenuItem
            // 
            visualizationToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { buildGraphToolStripMenuItem, settingsToolStripMenuItem });
            visualizationToolStripMenuItem.Name = "visualizationToolStripMenuItem";
            visualizationToolStripMenuItem.Size = new Size(85, 20);
            visualizationToolStripMenuItem.Text = "Visualization";
            // 
            // buildGraphToolStripMenuItem
            // 
            buildGraphToolStripMenuItem.Name = "buildGraphToolStripMenuItem";
            buildGraphToolStripMenuItem.Size = new Size(136, 22);
            buildGraphToolStripMenuItem.Text = "Build Graph";
            buildGraphToolStripMenuItem.Click += buildGraphToolStripMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(136, 22);
            settingsToolStripMenuItem.Text = "Settings";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // dataGridViewNodes
            // 
            dataGridViewNodes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewNodes.Dock = DockStyle.Fill;
            dataGridViewNodes.Location = new Point(3, 3);
            dataGridViewNodes.Name = "dataGridViewNodes";
            dataGridViewNodes.Size = new Size(786, 392);
            dataGridViewNodes.TabIndex = 1;
            dataGridViewNodes.Visible = false;
            // 
            // gViewer
            // 
            gViewer.ArrowheadLength = 10D;
            gViewer.AsyncLayout = false;
            gViewer.AutoScroll = true;
            gViewer.BackwardEnabled = false;
            gViewer.BuildHitTree = true;
            gViewer.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.MDS;
            gViewer.Dock = DockStyle.Fill;
            gViewer.EdgeInsertButtonVisible = true;
            gViewer.FileName = "";
            gViewer.ForwardEnabled = false;
            gViewer.Graph = null;
            gViewer.IncrementalDraggingModeAlways = false;
            gViewer.InsertingEdge = false;
            gViewer.LayoutAlgorithmSettingsButtonVisible = true;
            gViewer.LayoutEditingEnabled = true;
            gViewer.Location = new Point(3, 3);
            gViewer.LooseOffsetForRouting = 0.25D;
            gViewer.MouseHitDistance = 0.05D;
            gViewer.Name = "gViewer";
            gViewer.NavigationVisible = true;
            gViewer.NeedToCalculateLayout = true;
            gViewer.OffsetForRelaxingInRouting = 0.6D;
            gViewer.PaddingForEdgeRouting = 8D;
            gViewer.PanButtonPressed = false;
            gViewer.SaveAsImageEnabled = true;
            gViewer.SaveAsMsaglEnabled = true;
            gViewer.SaveButtonVisible = true;
            gViewer.SaveGraphButtonVisible = true;
            gViewer.SaveInVectorFormatEnabled = true;
            gViewer.Size = new Size(786, 392);
            gViewer.TabIndex = 2;
            gViewer.TightOffsetForRouting = 0.125D;
            gViewer.ToolBarIsVisible = true;
            gViewer.Transform = (Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation)resources.GetObject("gViewer.Transform");
            gViewer.UndoRedoButtonsVisible = true;
            gViewer.WindowZoomButtonPressed = false;
            gViewer.ZoomF = 1D;
            gViewer.ZoomWindowThreshold = 0.05D;
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabPageDataGridView);
            tabControl.Controls.Add(tabPageGViewer);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 24);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(800, 426);
            tabControl.TabIndex = 3;
            // 
            // tabPageDataGridView
            // 
            tabPageDataGridView.Controls.Add(dataGridViewNodes);
            tabPageDataGridView.Location = new Point(4, 24);
            tabPageDataGridView.Name = "tabPageDataGridView";
            tabPageDataGridView.Padding = new Padding(3);
            tabPageDataGridView.Size = new Size(792, 398);
            tabPageDataGridView.TabIndex = 0;
            tabPageDataGridView.Text = "Table";
            tabPageDataGridView.UseVisualStyleBackColor = true;
            // 
            // tabPageGViewer
            // 
            tabPageGViewer.Controls.Add(gViewer);
            tabPageGViewer.Location = new Point(4, 24);
            tabPageGViewer.Name = "tabPageGViewer";
            tabPageGViewer.Padding = new Padding(3);
            tabPageGViewer.Size = new Size(792, 398);
            tabPageGViewer.TabIndex = 1;
            tabPageGViewer.Text = "Graph";
            tabPageGViewer.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tabControl);
            Controls.Add(menuStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            Text = "Main Form";
            Load += MainForm_Load;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewNodes).EndInit();
            tabControl.ResumeLayout(false);
            tabPageDataGridView.ResumeLayout(false);
            tabPageGViewer.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem dataToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem showToolStripMenuItem;
        private DataGridView dataGridViewNodes;
        private ToolStripMenuItem visualizationToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private Microsoft.Msagl.GraphViewerGdi.GViewer gViewer;
        private ToolStripMenuItem buildGraphToolStripMenuItem;
        private TabControl tabControl;
        private TabPage tabPageDataGridView;
        private TabPage tabPageGViewer;
    }
}
