namespace VisualizationSystem.UI.Forms
{
    partial class ListSelectionForm
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
            cmbItems = new ComboBox();
            btnSubmit = new Button();
            lblSelectItem = new Label();
            SuspendLayout();
            // 
            // cmbItems
            // 
            cmbItems.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbItems.FormattingEnabled = true;
            cmbItems.Location = new Point(12, 43);
            cmbItems.Name = "cmbItems";
            cmbItems.Size = new Size(235, 23);
            cmbItems.TabIndex = 0;
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
            // lblSelectItem
            // 
            lblSelectItem.AutoSize = true;
            lblSelectItem.Location = new Point(12, 14);
            lblSelectItem.Name = "lblSelectItem";
            lblSelectItem.Size = new Size(82, 15);
            lblSelectItem.TabIndex = 6;
            lblSelectItem.Text = "Select column";
            // 
            // ListSelectionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(259, 123);
            Controls.Add(lblSelectItem);
            Controls.Add(btnSubmit);
            Controls.Add(cmbItems);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ListSelectionForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Select column name";
            Paint += ColumnSelectionForm_Paint;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbItems;
        private Button btnSubmit;
        private Label lblSelectItem;
    }
}