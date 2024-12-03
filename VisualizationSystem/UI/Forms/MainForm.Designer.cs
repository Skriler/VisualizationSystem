namespace VisualizationSystem.UI.Forms
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
            uploadExcelFileToolStripMenuItem = new ToolStripMenuItem();
            showToolStripMenuItem = new ToolStripMenuItem();
            loadTableToolStripMenuItem = new ToolStripMenuItem();
            visualizationToolStripMenuItem = new ToolStripMenuItem();
            buildGraphToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            tabControl = new TabControl();
            saveGraphImageToolStripMenuItem = new ToolStripMenuItem();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { dataToolStripMenuItem, visualizationToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(884, 24);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip1";
            // 
            // dataToolStripMenuItem
            // 
            dataToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { uploadExcelFileToolStripMenuItem, showToolStripMenuItem, loadTableToolStripMenuItem, saveGraphImageToolStripMenuItem });
            dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            dataToolStripMenuItem.Size = new Size(43, 20);
            dataToolStripMenuItem.Text = "Data";
            // 
            // uploadExcelFileToolStripMenuItem
            // 
            uploadExcelFileToolStripMenuItem.Name = "uploadExcelFileToolStripMenuItem";
            uploadExcelFileToolStripMenuItem.Size = new Size(180, 22);
            uploadExcelFileToolStripMenuItem.Text = "Upload Excel File";
            uploadExcelFileToolStripMenuItem.Click += uploadExcelFileToolStripMenuItem_Click;
            // 
            // showToolStripMenuItem
            // 
            showToolStripMenuItem.Name = "showToolStripMenuItem";
            showToolStripMenuItem.Size = new Size(180, 22);
            showToolStripMenuItem.Text = "Show";
            showToolStripMenuItem.Click += showToolStripMenuItem_Click;
            // 
            // loadTableToolStripMenuItem
            // 
            loadTableToolStripMenuItem.Name = "loadTableToolStripMenuItem";
            loadTableToolStripMenuItem.Size = new Size(180, 22);
            loadTableToolStripMenuItem.Text = "Load Table";
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
            // tabControl
            // 
            tabControl.Dock = DockStyle.Fill;
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.Location = new Point(0, 24);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(884, 637);
            tabControl.TabIndex = 3;
            tabControl.DrawItem += tabControl_DrawItem;
            tabControl.MouseDown += tabControl_MouseDown;
            // 
            // saveGraphImageToolStripMenuItem
            // 
            saveGraphImageToolStripMenuItem.Name = "saveGraphImageToolStripMenuItem";
            saveGraphImageToolStripMenuItem.Size = new Size(180, 22);
            saveGraphImageToolStripMenuItem.Text = "Save graph image";
            saveGraphImageToolStripMenuItem.Click += saveGraphImageToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(884, 661);
            Controls.Add(tabControl);
            Controls.Add(menuStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            Text = "Visualization System";
            Load += MainForm_Load;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem dataToolStripMenuItem;
        private ToolStripMenuItem uploadExcelFileToolStripMenuItem;
        private ToolStripMenuItem showToolStripMenuItem;
        private ToolStripMenuItem visualizationToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem buildGraphToolStripMenuItem;
        private ToolStripMenuItem loadTableToolStripMenuItem;
        private TabControl tabControl;
        private ToolStripMenuItem saveGraphImageToolStripMenuItem;
    }
}
