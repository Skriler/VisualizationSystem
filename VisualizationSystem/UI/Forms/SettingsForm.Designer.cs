namespace VisualizationSystem.UI.Forms
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            lblMinMatchingParameters = new Label();
            lblDeviationPercent = new Label();
            trcbrMinMatchingParameters = new TrackBar();
            trcbrDeviationPercent = new TrackBar();
            btnSubmit = new Button();
            lblMinMatchingParametersValue = new Label();
            lblDeviationPercentValue = new Label();
            lblSelectedParameters = new Label();
            clbSelectedParams = new CheckedListBox();
            ((System.ComponentModel.ISupportInitialize)trcbrMinMatchingParameters).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trcbrDeviationPercent).BeginInit();
            SuspendLayout();
            // 
            // lblMinMatchingParameters
            // 
            lblMinMatchingParameters.AutoSize = true;
            lblMinMatchingParameters.Location = new Point(11, 11);
            lblMinMatchingParameters.Name = "lblMinMatchingParameters";
            lblMinMatchingParameters.Size = new Size(147, 15);
            lblMinMatchingParameters.TabIndex = 0;
            lblMinMatchingParameters.Text = "Min matching parameters ";
            // 
            // lblDeviationPercent
            // 
            lblDeviationPercent.AutoSize = true;
            lblDeviationPercent.Location = new Point(29, 123);
            lblDeviationPercent.Name = "lblDeviationPercent";
            lblDeviationPercent.Size = new Size(100, 15);
            lblDeviationPercent.TabIndex = 1;
            lblDeviationPercent.Text = "Deviation percent";
            // 
            // trcbrMinMatchingParameters
            // 
            trcbrMinMatchingParameters.Location = new Point(11, 41);
            trcbrMinMatchingParameters.Maximum = 60;
            trcbrMinMatchingParameters.Name = "trcbrMinMatchingParameters";
            trcbrMinMatchingParameters.Size = new Size(147, 45);
            trcbrMinMatchingParameters.TabIndex = 2;
            trcbrMinMatchingParameters.Value = 20;
            trcbrMinMatchingParameters.Scroll += trcbrMinMatchingParameters_Scroll;
            // 
            // trcbrDeviationPercent
            // 
            trcbrDeviationPercent.Location = new Point(6, 141);
            trcbrDeviationPercent.Maximum = 60;
            trcbrDeviationPercent.Name = "trcbrDeviationPercent";
            trcbrDeviationPercent.Size = new Size(147, 45);
            trcbrDeviationPercent.TabIndex = 3;
            trcbrDeviationPercent.Value = 20;
            trcbrDeviationPercent.Scroll += trcbrDeviationPercent_Scroll;
            // 
            // btnSubmit
            // 
            btnSubmit.Location = new Point(6, 228);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(319, 29);
            btnSubmit.TabIndex = 4;
            btnSubmit.Text = "Submit";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // lblMinMatchingParametersValue
            // 
            lblMinMatchingParametersValue.AutoSize = true;
            lblMinMatchingParametersValue.Location = new Point(57, 89);
            lblMinMatchingParametersValue.Name = "lblMinMatchingParametersValue";
            lblMinMatchingParametersValue.Size = new Size(47, 15);
            lblMinMatchingParametersValue.TabIndex = 5;
            lblMinMatchingParametersValue.Text = "Value: 0";
            // 
            // lblDeviationPercentValue
            // 
            lblDeviationPercentValue.AutoSize = true;
            lblDeviationPercentValue.Location = new Point(58, 189);
            lblDeviationPercentValue.Name = "lblDeviationPercentValue";
            lblDeviationPercentValue.Size = new Size(47, 15);
            lblDeviationPercentValue.TabIndex = 6;
            lblDeviationPercentValue.Text = "Value: 0";
            // 
            // lblSelectedParameters
            // 
            lblSelectedParameters.AutoSize = true;
            lblSelectedParameters.Location = new Point(178, 11);
            lblSelectedParameters.Name = "lblSelectedParameters";
            lblSelectedParameters.Size = new Size(147, 15);
            lblSelectedParameters.TabIndex = 7;
            lblSelectedParameters.Text = "Min matching parameters ";
            // 
            // clbSelectedParams
            // 
            clbSelectedParams.FormattingEnabled = true;
            clbSelectedParams.Location = new Point(178, 41);
            clbSelectedParams.Name = "clbSelectedParams";
            clbSelectedParams.Size = new Size(147, 166);
            clbSelectedParams.TabIndex = 8;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(348, 272);
            Controls.Add(clbSelectedParams);
            Controls.Add(lblSelectedParameters);
            Controls.Add(lblDeviationPercentValue);
            Controls.Add(lblMinMatchingParametersValue);
            Controls.Add(btnSubmit);
            Controls.Add(trcbrDeviationPercent);
            Controls.Add(trcbrMinMatchingParameters);
            Controls.Add(lblDeviationPercent);
            Controls.Add(lblMinMatchingParameters);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "SettingsForm";
            Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)trcbrMinMatchingParameters).EndInit();
            ((System.ComponentModel.ISupportInitialize)trcbrDeviationPercent).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblMinMatchingParameters;
        private Label lblDeviationPercent;
        private TrackBar trcbrMinMatchingParameters;
        private TrackBar trcbrDeviationPercent;
        private Button btnSubmit;
        private Label lblMinMatchingParametersValue;
        private Label lblDeviationPercentValue;
        private Label lblSelectedParameters;
        private CheckedListBox clbSelectedParams;
    }
}