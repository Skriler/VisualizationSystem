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
            calculateSimilarToolStripMenuItem = new ToolStripMenuItem();
            visualizationToolStripMenuItem = new ToolStripMenuItem();
            buildGraphToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            dataGridViewNodes = new DataGridView();
            gViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewNodes).BeginInit();
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
            dataToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadToolStripMenuItem, showToolStripMenuItem, calculateSimilarToolStripMenuItem });
            dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            dataToolStripMenuItem.Size = new Size(43, 20);
            dataToolStripMenuItem.Text = "Data";
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new Size(161, 22);
            loadToolStripMenuItem.Text = "Load";
            loadToolStripMenuItem.Click += loadToolStripMenuItem_Click;
            // 
            // showToolStripMenuItem
            // 
            showToolStripMenuItem.Name = "showToolStripMenuItem";
            showToolStripMenuItem.Size = new Size(161, 22);
            showToolStripMenuItem.Text = "Show";
            showToolStripMenuItem.Click += showToolStripMenuItem_Click;
            // 
            // calculateSimilarToolStripMenuItem
            // 
            calculateSimilarToolStripMenuItem.Name = "calculateSimilarToolStripMenuItem";
            calculateSimilarToolStripMenuItem.Size = new Size(161, 22);
            calculateSimilarToolStripMenuItem.Text = "Calculate similar";
            calculateSimilarToolStripMenuItem.Click += calculateSimilarToolStripMenuItem_Click;
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
            dataGridViewNodes.Location = new Point(12, 27);
            dataGridViewNodes.Name = "dataGridViewNodes";
            dataGridViewNodes.Size = new Size(776, 411);
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
            gViewer.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.UseSettingsOfTheGraph;
            gViewer.EdgeInsertButtonVisible = true;
            gViewer.FileName = "";
            gViewer.ForwardEnabled = false;
            gViewer.Graph = null;
            gViewer.IncrementalDraggingModeAlways = false;
            gViewer.InsertingEdge = false;
            gViewer.LayoutAlgorithmSettingsButtonVisible = true;
            gViewer.LayoutEditingEnabled = true;
            gViewer.Location = new Point(11, 27);
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
            gViewer.Size = new Size(777, 411);
            gViewer.TabIndex = 2;
            gViewer.TightOffsetForRouting = 0.125D;
            gViewer.ToolBarIsVisible = true;
            gViewer.Transform = (Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation)resources.GetObject("gViewer.Transform");
            gViewer.UndoRedoButtonsVisible = true;
            gViewer.WindowZoomButtonPressed = false;
            gViewer.ZoomF = 1D;
            gViewer.ZoomWindowThreshold = 0.05D;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(gViewer);
            Controls.Add(dataGridViewNodes);
            Controls.Add(menuStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            Text = "Main Form";
            Load += MainForm_Load;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewNodes).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem dataToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem showToolStripMenuItem;
        private DataGridView dataGridViewNodes;
        private ToolStripMenuItem calculateSimilarToolStripMenuItem;
        private ToolStripMenuItem visualizationToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private Microsoft.Msagl.GraphViewerGdi.GViewer gViewer;
        private ToolStripMenuItem buildGraphToolStripMenuItem;
    }
}
