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
            sqlCommand1 = new Microsoft.Data.SqlClient.SqlCommand();
            dgvNodeParameters = new DataGridView();
            Parameter = new DataGridViewTextBoxColumn();
            Value = new DataGridViewTextBoxColumn();
            dgvSimilarNodes = new DataGridView();
            Node = new DataGridViewLinkColumn();
            SimilarityPercent = new DataGridViewTextBoxColumn();
            lblNodeParameters = new Label();
            lblSimilarNodes = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvNodeParameters).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvSimilarNodes).BeginInit();
            SuspendLayout();
            // 
            // sqlCommand1
            // 
            sqlCommand1.CommandTimeout = 30;
            sqlCommand1.EnableOptimizedParameterBinding = false;
            // 
            // dgvNodeParameters
            // 
            dgvNodeParameters.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvNodeParameters.Columns.AddRange(new DataGridViewColumn[] { Parameter, Value });
            dgvNodeParameters.Location = new Point(20, 40);
            dgvNodeParameters.Name = "dgvNodeParameters";
            dgvNodeParameters.Size = new Size(350, 400);
            dgvNodeParameters.TabIndex = 5;
            // 
            // Parameter
            // 
            Parameter.FillWeight = 150F;
            Parameter.HeaderText = "Parameter";
            Parameter.Name = "Parameter";
            Parameter.ReadOnly = true;
            Parameter.Resizable = DataGridViewTriState.False;
            Parameter.Width = 150;
            // 
            // Value
            // 
            Value.FillWeight = 150F;
            Value.HeaderText = "Value";
            Value.Name = "Value";
            Value.ReadOnly = true;
            Value.Resizable = DataGridViewTriState.False;
            Value.Width = 150;
            // 
            // dgvSimilarNodes
            // 
            dgvSimilarNodes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSimilarNodes.Columns.AddRange(new DataGridViewColumn[] { Node, SimilarityPercent });
            dgvSimilarNodes.Location = new Point(400, 40);
            dgvSimilarNodes.Name = "dgvSimilarNodes";
            dgvSimilarNodes.Size = new Size(350, 400);
            dgvSimilarNodes.TabIndex = 6;
            // 
            // Node
            // 
            Node.FillWeight = 150F;
            Node.HeaderText = "Node";
            Node.Name = "Node";
            Node.ReadOnly = true;
            Node.Resizable = DataGridViewTriState.False;
            Node.SortMode = DataGridViewColumnSortMode.Automatic;
            Node.Width = 150;
            // 
            // SimilarityPercent
            // 
            SimilarityPercent.FillWeight = 150F;
            SimilarityPercent.HeaderText = "Similarity %";
            SimilarityPercent.Name = "SimilarityPercent";
            SimilarityPercent.ReadOnly = true;
            SimilarityPercent.Resizable = DataGridViewTriState.False;
            SimilarityPercent.Width = 150;
            // 
            // lblNodeParameters
            // 
            lblNodeParameters.AutoSize = true;
            lblNodeParameters.Location = new Point(20, 12);
            lblNodeParameters.Name = "lblNodeParameters";
            lblNodeParameters.Size = new Size(187, 15);
            lblNodeParameters.TabIndex = 7;
            lblNodeParameters.Text = "Node parameters (Type and Value)";
            // 
            // lblSimilarNodes
            // 
            lblSimilarNodes.AutoSize = true;
            lblSimilarNodes.Location = new Point(400, 12);
            lblSimilarNodes.Name = "lblSimilarNodes";
            lblSimilarNodes.Size = new Size(203, 15);
            lblSimilarNodes.TabIndex = 8;
            lblSimilarNodes.Text = "Similar nodes (Sorted by similarity %)";
            // 
            // NodeDetailsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(764, 461);
            Controls.Add(lblSimilarNodes);
            Controls.Add(lblNodeParameters);
            Controls.Add(dgvSimilarNodes);
            Controls.Add(dgvNodeParameters);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "NodeDetailsForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Node name";
            ((System.ComponentModel.ISupportInitialize)dgvNodeParameters).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvSimilarNodes).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Microsoft.Data.SqlClient.SqlCommand sqlCommand1;
        private DataGridView dgvNodeParameters;
        private DataGridView dgvSimilarNodes;
        private DataGridViewTextBoxColumn Parameter;
        private DataGridViewTextBoxColumn Value;
        private DataGridViewLinkColumn Node;
        private DataGridViewTextBoxColumn SimilarityPercent;
        private Label lblNodeParameters;
        private Label lblSimilarNodes;
    }
}