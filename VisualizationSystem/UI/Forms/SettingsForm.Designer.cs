﻿namespace VisualizationSystem.UI.Forms
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
            lblMinSimilarityPercentage = new Label();
            lblDeviationPercent = new Label();
            btnSubmit = new Button();
            panelParameterStates = new Panel();
            lblWeight = new Label();
            nudWeight = new NumericUpDown();
            chkbxIsActive = new CheckBox();
            cmbNames = new ComboBox();
            nudMinSimilarityPercentage = new NumericUpDown();
            nudDeviationPercent = new NumericUpDown();
            btnSetDefaults = new Button();
            panelClusteringOptions = new Panel();
            nudSecondParameter = new NumericUpDown();
            lblSecondParameter = new Label();
            lblFirstParameter = new Label();
            cmbClusterAlgorithm = new ComboBox();
            nudFirstParameter = new NumericUpDown();
            chkbxUseClustering = new CheckBox();
            chkbxWithEdges = new CheckBox();
            panelParameterStates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMinSimilarityPercentage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDeviationPercent).BeginInit();
            panelClusteringOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudSecondParameter).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudFirstParameter).BeginInit();
            SuspendLayout();
            // 
            // lblMinSimilarityPercentage
            // 
            lblMinSimilarityPercentage.AutoSize = true;
            lblMinSimilarityPercentage.Location = new Point(12, 15);
            lblMinSimilarityPercentage.Name = "lblMinSimilarityPercentage";
            lblMinSimilarityPercentage.Size = new Size(141, 15);
            lblMinSimilarityPercentage.TabIndex = 0;
            lblMinSimilarityPercentage.Text = "Min similarity percentage";
            // 
            // lblDeviationPercent
            // 
            lblDeviationPercent.AutoSize = true;
            lblDeviationPercent.Location = new Point(12, 55);
            lblDeviationPercent.Name = "lblDeviationPercent";
            lblDeviationPercent.Size = new Size(100, 15);
            lblDeviationPercent.TabIndex = 1;
            lblDeviationPercent.Text = "Deviation percent";
            // 
            // btnSubmit
            // 
            btnSubmit.Location = new Point(12, 375);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(120, 30);
            btnSubmit.TabIndex = 4;
            btnSubmit.Text = "Submit";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // panelParameterStates
            // 
            panelParameterStates.AccessibleDescription = "";
            panelParameterStates.AccessibleName = "";
            panelParameterStates.Controls.Add(lblWeight);
            panelParameterStates.Controls.Add(nudWeight);
            panelParameterStates.Controls.Add(chkbxIsActive);
            panelParameterStates.Controls.Add(cmbNames);
            panelParameterStates.Location = new Point(10, 132);
            panelParameterStates.Name = "panelParameterStates";
            panelParameterStates.Size = new Size(250, 90);
            panelParameterStates.TabIndex = 9;
            // 
            // lblWeight
            // 
            lblWeight.AutoSize = true;
            lblWeight.Location = new Point(12, 55);
            lblWeight.Name = "lblWeight";
            lblWeight.Size = new Size(48, 15);
            lblWeight.TabIndex = 3;
            lblWeight.Text = "Weight:";
            // 
            // nudWeight
            // 
            nudWeight.DecimalPlaces = 1;
            nudWeight.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudWeight.Location = new Point(65, 52);
            nudWeight.Name = "nudWeight";
            nudWeight.Size = new Size(76, 23);
            nudWeight.TabIndex = 2;
            // 
            // chkbxIsActive
            // 
            chkbxIsActive.AutoSize = true;
            chkbxIsActive.Location = new Point(165, 55);
            chkbxIsActive.Name = "chkbxIsActive";
            chkbxIsActive.Size = new Size(70, 19);
            chkbxIsActive.TabIndex = 1;
            chkbxIsActive.Text = "Is Active";
            chkbxIsActive.UseVisualStyleBackColor = true;
            // 
            // cmbNames
            // 
            cmbNames.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNames.FormattingEnabled = true;
            cmbNames.Location = new Point(12, 12);
            cmbNames.Name = "cmbNames";
            cmbNames.Size = new Size(232, 23);
            cmbNames.TabIndex = 0;
            cmbNames.SelectedValueChanged += cmbNames_SelectedValueChanged;
            // 
            // nudMinSimilarityPercentage
            // 
            nudMinSimilarityPercentage.DecimalPlaces = 1;
            nudMinSimilarityPercentage.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudMinSimilarityPercentage.Location = new Point(178, 12);
            nudMinSimilarityPercentage.Name = "nudMinSimilarityPercentage";
            nudMinSimilarityPercentage.Size = new Size(76, 23);
            nudMinSimilarityPercentage.TabIndex = 10;
            nudMinSimilarityPercentage.Value = new decimal(new int[] { 40, 0, 0, 0 });
            // 
            // nudDeviationPercent
            // 
            nudDeviationPercent.DecimalPlaces = 1;
            nudDeviationPercent.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudDeviationPercent.Location = new Point(178, 52);
            nudDeviationPercent.Name = "nudDeviationPercent";
            nudDeviationPercent.Size = new Size(76, 23);
            nudDeviationPercent.TabIndex = 11;
            nudDeviationPercent.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // btnSetDefaults
            // 
            btnSetDefaults.Location = new Point(140, 375);
            btnSetDefaults.Name = "btnSetDefaults";
            btnSetDefaults.Size = new Size(120, 30);
            btnSetDefaults.TabIndex = 12;
            btnSetDefaults.Text = "Set defaults";
            btnSetDefaults.UseVisualStyleBackColor = true;
            btnSetDefaults.Click += btnSetDefaults_Click;
            // 
            // panelClusteringOptions
            // 
            panelClusteringOptions.AccessibleDescription = "";
            panelClusteringOptions.AccessibleName = "";
            panelClusteringOptions.Controls.Add(nudSecondParameter);
            panelClusteringOptions.Controls.Add(lblSecondParameter);
            panelClusteringOptions.Controls.Add(lblFirstParameter);
            panelClusteringOptions.Controls.Add(cmbClusterAlgorithm);
            panelClusteringOptions.Controls.Add(nudFirstParameter);
            panelClusteringOptions.Location = new Point(10, 228);
            panelClusteringOptions.Name = "panelClusteringOptions";
            panelClusteringOptions.Size = new Size(250, 141);
            panelClusteringOptions.TabIndex = 10;
            // 
            // nudSecondParameter
            // 
            nudSecondParameter.DecimalPlaces = 1;
            nudSecondParameter.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudSecondParameter.Location = new Point(164, 96);
            nudSecondParameter.Name = "nudSecondParameter";
            nudSecondParameter.Size = new Size(80, 23);
            nudSecondParameter.TabIndex = 13;
            // 
            // lblSecondParameter
            // 
            lblSecondParameter.AutoSize = true;
            lblSecondParameter.Location = new Point(12, 98);
            lblSecondParameter.Name = "lblSecondParameter";
            lblSecondParameter.Size = new Size(44, 15);
            lblSecondParameter.TabIndex = 14;
            lblSecondParameter.Text = "Param:";
            // 
            // lblFirstParameter
            // 
            lblFirstParameter.AutoSize = true;
            lblFirstParameter.Location = new Point(12, 55);
            lblFirstParameter.Name = "lblFirstParameter";
            lblFirstParameter.Size = new Size(44, 15);
            lblFirstParameter.TabIndex = 5;
            lblFirstParameter.Text = "Param:";
            // 
            // cmbClusterAlgorithm
            // 
            cmbClusterAlgorithm.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbClusterAlgorithm.FormattingEnabled = true;
            cmbClusterAlgorithm.Location = new Point(12, 12);
            cmbClusterAlgorithm.Name = "cmbClusterAlgorithm";
            cmbClusterAlgorithm.Size = new Size(232, 23);
            cmbClusterAlgorithm.TabIndex = 4;
            cmbClusterAlgorithm.SelectedIndexChanged += cmbClusterAlgorithm_SelectedIndexChanged;
            // 
            // nudFirstParameter
            // 
            nudFirstParameter.DecimalPlaces = 1;
            nudFirstParameter.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudFirstParameter.Location = new Point(164, 52);
            nudFirstParameter.Name = "nudFirstParameter";
            nudFirstParameter.Size = new Size(80, 23);
            nudFirstParameter.TabIndex = 4;
            // 
            // chkbxUseClustering
            // 
            chkbxUseClustering.AutoSize = true;
            chkbxUseClustering.Location = new Point(22, 97);
            chkbxUseClustering.Name = "chkbxUseClustering";
            chkbxUseClustering.Size = new Size(102, 19);
            chkbxUseClustering.TabIndex = 1;
            chkbxUseClustering.Text = "Use Clustering";
            chkbxUseClustering.UseVisualStyleBackColor = true;
            chkbxUseClustering.CheckedChanged += chkbxUseClustering_CheckedChanged;
            // 
            // chkbxWithEdges
            // 
            chkbxWithEdges.AutoSize = true;
            chkbxWithEdges.Location = new Point(144, 97);
            chkbxWithEdges.Name = "chkbxWithEdges";
            chkbxWithEdges.Size = new Size(110, 19);
            chkbxWithEdges.TabIndex = 13;
            chkbxWithEdges.Text = "Clustered graph";
            chkbxWithEdges.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(267, 413);
            Controls.Add(chkbxWithEdges);
            Controls.Add(panelClusteringOptions);
            Controls.Add(chkbxUseClustering);
            Controls.Add(btnSetDefaults);
            Controls.Add(nudDeviationPercent);
            Controls.Add(nudMinSimilarityPercentage);
            Controls.Add(panelParameterStates);
            Controls.Add(btnSubmit);
            Controls.Add(lblDeviationPercent);
            Controls.Add(lblMinSimilarityPercentage);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "SettingsForm";
            Text = "Settings";
            panelParameterStates.ResumeLayout(false);
            panelParameterStates.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMinSimilarityPercentage).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDeviationPercent).EndInit();
            panelClusteringOptions.ResumeLayout(false);
            panelClusteringOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudSecondParameter).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudFirstParameter).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblMinSimilarityPercentage;
        private Label lblDeviationPercent;
        private Button btnSubmit;
        private Panel panelParameterStates;
        private Label lblWeight;
        private NumericUpDown nudWeight;
        private CheckBox chkbxIsActive;
        private ComboBox cmbNames;
        private NumericUpDown nudMinSimilarityPercentage;
        private NumericUpDown nudDeviationPercent;
        private Button btnSetDefaults;
        private Panel panelClusteringOptions;
        private ComboBox cmbClusterAlgorithm;
        private NumericUpDown nudSecondParameter;
        private Label lblSecondParameter;
        private Label lblFirstParameter;
        private NumericUpDown nudFirstParameter;
        private CheckBox chkbxUseClustering;
        private CheckBox chkbxWithEdges;
    }
}