namespace VisualizationSystem.UI.Forms
{
    partial class ColumnSelectionForm
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
            cmbColumnNames = new ComboBox();
            btnSubmit = new Button();
            lblSelectColumn = new Label();
            SuspendLayout();
            // 
            // cmbColumnNames
            // 
            cmbColumnNames.FormattingEnabled = true;
            cmbColumnNames.Location = new Point(12, 43);
            cmbColumnNames.Name = "cmbColumnNames";
            cmbColumnNames.Size = new Size(235, 23);
            cmbColumnNames.TabIndex = 0;
            // 
            // btnSubmit
            // 
            btnSubmit.Location = new Point(12, 81);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(235, 30);
            btnSubmit.TabIndex = 5;
            btnSubmit.Text = "Submit";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // lblSelectColumn
            // 
            lblSelectColumn.AutoSize = true;
            lblSelectColumn.Location = new Point(12, 14);
            lblSelectColumn.Name = "lblSelectColumn";
            lblSelectColumn.Size = new Size(82, 15);
            lblSelectColumn.TabIndex = 6;
            lblSelectColumn.Text = "Select column";
            // 
            // ColumnSelectionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(259, 123);
            Controls.Add(lblSelectColumn);
            Controls.Add(btnSubmit);
            Controls.Add(cmbColumnNames);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ColumnSelectionForm";
            Text = "Select column name";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbColumnNames;
        private Button btnSubmit;
        private Label lblSelectColumn;
    }
}