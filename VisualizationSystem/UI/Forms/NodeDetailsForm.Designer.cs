namespace VisualizationSystem.UI.Forms
{
    partial class NodeDetailsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblNodeName = new Label();
            lstNodeParameters = new ListBox();
            lstSimilarNodes = new ListBox();
            SuspendLayout();
            // 
            // lblNodeName
            // 
            lblNodeName.AutoSize = true;
            lblNodeName.Location = new Point(23, 21);
            lblNodeName.Name = "lblNodeName";
            lblNodeName.Size = new Size(69, 15);
            lblNodeName.TabIndex = 0;
            lblNodeName.Text = "Node name";
            // 
            // lstNodeParameters
            // 
            lstNodeParameters.FormattingEnabled = true;
            lstNodeParameters.ItemHeight = 15;
            lstNodeParameters.Location = new Point(30, 60);
            lstNodeParameters.Name = "lstNodeParameters";
            lstNodeParameters.Size = new Size(120, 229);
            lstNodeParameters.TabIndex = 1;
            // 
            // lstSimilarNodes
            // 
            lstSimilarNodes.FormattingEnabled = true;
            lstSimilarNodes.ItemHeight = 15;
            lstSimilarNodes.Location = new Point(180, 60);
            lstSimilarNodes.Name = "lstSimilarNodes";
            lstSimilarNodes.Size = new Size(120, 229);
            lstSimilarNodes.TabIndex = 2;
            // 
            // NodeDetailsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(324, 309);
            Controls.Add(lstSimilarNodes);
            Controls.Add(lstNodeParameters);
            Controls.Add(lblNodeName);
            Name = "NodeDetailsForm";
            Text = "Node name";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNodeName;
        private ListBox lstNodeParameters;
        private ListBox lstSimilarNodes;
    }
}