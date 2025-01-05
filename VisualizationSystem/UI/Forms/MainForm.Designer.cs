﻿namespace VisualizationSystem.UI.Forms
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
            uploadToolStripMenuItem = new ToolStripMenuItem();
            showToolStripMenuItem = new ToolStripMenuItem();
            datasetsToolStripMenuItem = new ToolStripMenuItem();
            visualizationToolStripMenuItem = new ToolStripMenuItem();
            buildGraphToolStripMenuItem = new ToolStripMenuItem();
            saveGraphImageToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            tabControl = new TabControl();
            buildClusterEdgeGraphToolStripMenuItem = new ToolStripMenuItem();
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
            dataToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { uploadToolStripMenuItem, showToolStripMenuItem, datasetsToolStripMenuItem });
            dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            dataToolStripMenuItem.Size = new Size(43, 20);
            dataToolStripMenuItem.Text = "Data";
            // 
            // uploadToolStripMenuItem
            // 
            uploadToolStripMenuItem.Name = "uploadToolStripMenuItem";
            uploadToolStripMenuItem.Size = new Size(129, 22);
            uploadToolStripMenuItem.Text = "Load excel";
            uploadToolStripMenuItem.Click += uploadToolStripMenuItem_Click;
            // 
            // showToolStripMenuItem
            // 
            showToolStripMenuItem.ForeColor = Color.Black;
            showToolStripMenuItem.Name = "showToolStripMenuItem";
            showToolStripMenuItem.Size = new Size(129, 22);
            showToolStripMenuItem.Text = "View data";
            showToolStripMenuItem.Click += showToolStripMenuItem_Click;
            // 
            // datasetsToolStripMenuItem
            // 
            datasetsToolStripMenuItem.Name = "datasetsToolStripMenuItem";
            datasetsToolStripMenuItem.Size = new Size(129, 22);
            datasetsToolStripMenuItem.Text = "Datasets";
            // 
            // visualizationToolStripMenuItem
            // 
            visualizationToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { buildGraphToolStripMenuItem, saveGraphImageToolStripMenuItem, settingsToolStripMenuItem, buildClusterEdgeGraphToolStripMenuItem });
            visualizationToolStripMenuItem.Name = "visualizationToolStripMenuItem";
            visualizationToolStripMenuItem.Size = new Size(85, 20);
            visualizationToolStripMenuItem.Text = "Visualization";
            // 
            // buildGraphToolStripMenuItem
            // 
            buildGraphToolStripMenuItem.Name = "buildGraphToolStripMenuItem";
            buildGraphToolStripMenuItem.Size = new Size(202, 22);
            buildGraphToolStripMenuItem.Text = "Build graph";
            buildGraphToolStripMenuItem.Click += buildGraphToolStripMenuItem_Click;
            // 
            // saveGraphImageToolStripMenuItem
            // 
            saveGraphImageToolStripMenuItem.Name = "saveGraphImageToolStripMenuItem";
            saveGraphImageToolStripMenuItem.Size = new Size(202, 22);
            saveGraphImageToolStripMenuItem.Text = "Save graph";
            saveGraphImageToolStripMenuItem.Click += saveGraphImageToolStripMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(202, 22);
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
            // buildClusterEdgeGraphToolStripMenuItem
            // 
            buildClusterEdgeGraphToolStripMenuItem.Name = "buildClusterEdgeGraphToolStripMenuItem";
            buildClusterEdgeGraphToolStripMenuItem.Size = new Size(202, 22);
            buildClusterEdgeGraphToolStripMenuItem.Text = "Build cluster edge graph";
            buildClusterEdgeGraphToolStripMenuItem.Click += buildClusterEdgeGraphToolStripMenuItem_Click;
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
        private ToolStripMenuItem uploadToolStripMenuItem;
        private ToolStripMenuItem showToolStripMenuItem;
        private ToolStripMenuItem visualizationToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem buildGraphToolStripMenuItem;
        private ToolStripMenuItem datasetsToolStripMenuItem;
        private TabControl tabControl;
        private ToolStripMenuItem saveGraphImageToolStripMenuItem;
        private ToolStripMenuItem buildClusterEdgeGraphToolStripMenuItem;
    }
}
