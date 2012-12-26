namespace ShockRouter
{
    partial class MainWindow
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
            this.studioButton = new System.Windows.Forms.Button();
            this.chartButton = new System.Windows.Forms.Button();
            this.obButton = new System.Windows.Forms.Button();
            this.emergencyButton = new System.Windows.Forms.Button();
            this.studioInputLabel = new System.Windows.Forms.Label();
            this.studioInputComboBox = new System.Windows.Forms.ComboBox();
            this.ChartUrlLabel = new System.Windows.Forms.Label();
            this.chartUrlTextBox = new System.Windows.Forms.TextBox();
            this.obUrlLabel = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.emergencyFileLabel = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.fileLabel = new System.Windows.Forms.Label();
            this.processorLabel = new System.Windows.Forms.Label();
            this.processorComboBox = new System.Windows.Forms.ComboBox();
            this.processorConfigButton = new System.Windows.Forms.Button();
            this.sourceLevelLabel = new System.Windows.Forms.Label();
            this.outputLevelLabel = new System.Windows.Forms.Label();
            this.scheduleListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.detectorLabel = new System.Windows.Forms.Label();
            this.secondsLabel = new System.Windows.Forms.Label();
            this.scheduleSeperator = new ShockCast.Seperator();
            this.outputRightMeter = new ShockCast.VolumeMeter();
            this.outputLeftMeter = new ShockCast.VolumeMeter();
            this.sourceRightMeter = new ShockCast.VolumeMeter();
            this.sourceLeftMeter = new ShockCast.VolumeMeter();
            this.LevelsSeperator = new ShockCast.Seperator();
            this.settingsSeperator = new ShockCast.Seperator();
            this.sourceSeperator = new ShockCast.Seperator();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // studioButton
            // 
            this.studioButton.CausesValidation = false;
            this.studioButton.Location = new System.Drawing.Point(12, 33);
            this.studioButton.Name = "studioButton";
            this.studioButton.Size = new System.Drawing.Size(150, 100);
            this.studioButton.TabIndex = 1;
            this.studioButton.Tag = "";
            this.studioButton.Text = "Studio";
            this.studioButton.UseVisualStyleBackColor = true;
            this.studioButton.Click += new System.EventHandler(this.SourceSwitch);
            // 
            // chartButton
            // 
            this.chartButton.CausesValidation = false;
            this.chartButton.Location = new System.Drawing.Point(168, 33);
            this.chartButton.Name = "chartButton";
            this.chartButton.Size = new System.Drawing.Size(150, 100);
            this.chartButton.TabIndex = 2;
            this.chartButton.Tag = "";
            this.chartButton.Text = "SRA Chart Show";
            this.chartButton.UseVisualStyleBackColor = true;
            this.chartButton.Click += new System.EventHandler(this.SourceSwitch);
            // 
            // obButton
            // 
            this.obButton.CausesValidation = false;
            this.obButton.Location = new System.Drawing.Point(324, 33);
            this.obButton.Name = "obButton";
            this.obButton.Size = new System.Drawing.Size(150, 100);
            this.obButton.TabIndex = 3;
            this.obButton.Tag = "";
            this.obButton.Text = "Outside Broadcast";
            this.obButton.UseVisualStyleBackColor = true;
            this.obButton.Click += new System.EventHandler(this.SourceSwitch);
            // 
            // emergencyButton
            // 
            this.emergencyButton.CausesValidation = false;
            this.emergencyButton.Location = new System.Drawing.Point(480, 33);
            this.emergencyButton.Name = "emergencyButton";
            this.emergencyButton.Size = new System.Drawing.Size(150, 100);
            this.emergencyButton.TabIndex = 4;
            this.emergencyButton.Tag = "";
            this.emergencyButton.Text = "Emergency Output";
            this.emergencyButton.UseVisualStyleBackColor = true;
            this.emergencyButton.Click += new System.EventHandler(this.SourceSwitch);
            // 
            // studioInputLabel
            // 
            this.studioInputLabel.AutoSize = true;
            this.studioInputLabel.Location = new System.Drawing.Point(12, 157);
            this.studioInputLabel.Name = "studioInputLabel";
            this.studioInputLabel.Size = new System.Drawing.Size(97, 13);
            this.studioInputLabel.TabIndex = 6;
            this.studioInputLabel.Text = "Studio Audio Input:";
            // 
            // studioInputComboBox
            // 
            this.studioInputComboBox.CausesValidation = false;
            this.studioInputComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.studioInputComboBox.FormattingEnabled = true;
            this.studioInputComboBox.Location = new System.Drawing.Point(15, 173);
            this.studioInputComboBox.Name = "studioInputComboBox";
            this.studioInputComboBox.Size = new System.Drawing.Size(303, 21);
            this.studioInputComboBox.TabIndex = 7;
            this.studioInputComboBox.SelectedIndexChanged += new System.EventHandler(this.studioInputComboBox_SelectedIndexChanged);
            // 
            // ChartUrlLabel
            // 
            this.ChartUrlLabel.AutoSize = true;
            this.ChartUrlLabel.Location = new System.Drawing.Point(12, 197);
            this.ChartUrlLabel.Name = "ChartUrlLabel";
            this.ChartUrlLabel.Size = new System.Drawing.Size(126, 13);
            this.ChartUrlLabel.TabIndex = 8;
            this.ChartUrlLabel.Text = "Chart Show Stream URL:";
            // 
            // chartUrlTextBox
            // 
            this.chartUrlTextBox.Location = new System.Drawing.Point(15, 213);
            this.chartUrlTextBox.Name = "chartUrlTextBox";
            this.chartUrlTextBox.Size = new System.Drawing.Size(303, 20);
            this.chartUrlTextBox.TabIndex = 9;
            // 
            // obUrlLabel
            // 
            this.obUrlLabel.AutoSize = true;
            this.obUrlLabel.Location = new System.Drawing.Point(12, 236);
            this.obUrlLabel.Name = "obUrlLabel";
            this.obUrlLabel.Size = new System.Drawing.Size(158, 13);
            this.obUrlLabel.TabIndex = 10;
            this.obUrlLabel.Text = "Outside Broadcast Stream URL:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(15, 252);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(303, 20);
            this.textBox1.TabIndex = 11;
            // 
            // emergencyFileLabel
            // 
            this.emergencyFileLabel.AutoSize = true;
            this.emergencyFileLabel.Location = new System.Drawing.Point(12, 275);
            this.emergencyFileLabel.Name = "emergencyFileLabel";
            this.emergencyFileLabel.Size = new System.Drawing.Size(117, 13);
            this.emergencyFileLabel.TabIndex = 12;
            this.emergencyFileLabel.Text = "Emergency Output File:";
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(243, 291);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 14;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            // 
            // fileLabel
            // 
            this.fileLabel.AutoEllipsis = true;
            this.fileLabel.Location = new System.Drawing.Point(13, 296);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(224, 18);
            this.fileLabel.TabIndex = 13;
            // 
            // processorLabel
            // 
            this.processorLabel.AutoSize = true;
            this.processorLabel.Location = new System.Drawing.Point(13, 340);
            this.processorLabel.Name = "processorLabel";
            this.processorLabel.Size = new System.Drawing.Size(124, 13);
            this.processorLabel.TabIndex = 15;
            this.processorLabel.Text = "Audio Processing Plugin:";
            // 
            // processorComboBox
            // 
            this.processorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.processorComboBox.FormattingEnabled = true;
            this.processorComboBox.Location = new System.Drawing.Point(16, 356);
            this.processorComboBox.Name = "processorComboBox";
            this.processorComboBox.Size = new System.Drawing.Size(222, 21);
            this.processorComboBox.TabIndex = 16;
            // 
            // processorConfigButton
            // 
            this.processorConfigButton.Location = new System.Drawing.Point(244, 355);
            this.processorConfigButton.Name = "processorConfigButton";
            this.processorConfigButton.Size = new System.Drawing.Size(75, 23);
            this.processorConfigButton.TabIndex = 17;
            this.processorConfigButton.Text = "Configure";
            this.processorConfigButton.UseVisualStyleBackColor = true;
            // 
            // sourceLevelLabel
            // 
            this.sourceLevelLabel.AutoSize = true;
            this.sourceLevelLabel.Location = new System.Drawing.Point(324, 157);
            this.sourceLevelLabel.Name = "sourceLevelLabel";
            this.sourceLevelLabel.Size = new System.Drawing.Size(73, 13);
            this.sourceLevelLabel.TabIndex = 19;
            this.sourceLevelLabel.Text = "Source Level:";
            // 
            // outputLevelLabel
            // 
            this.outputLevelLabel.AutoSize = true;
            this.outputLevelLabel.Location = new System.Drawing.Point(324, 213);
            this.outputLevelLabel.Name = "outputLevelLabel";
            this.outputLevelLabel.Size = new System.Drawing.Size(71, 13);
            this.outputLevelLabel.TabIndex = 22;
            this.outputLevelLabel.Text = "Output Level:";
            // 
            // scheduleListView
            // 
            this.scheduleListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.scheduleListView.Location = new System.Drawing.Point(327, 291);
            this.scheduleListView.Name = "scheduleListView";
            this.scheduleListView.Size = new System.Drawing.Size(273, 86);
            this.scheduleListView.TabIndex = 26;
            this.scheduleListView.UseCompatibleStateImageBehavior = false;
            this.scheduleListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Source";
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(606, 290);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(24, 23);
            this.addButton.TabIndex = 27;
            this.addButton.Text = "+";
            this.addButton.UseVisualStyleBackColor = true;
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(606, 319);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(24, 23);
            this.removeButton.TabIndex = 28;
            this.removeButton.Text = "-";
            this.removeButton.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(144, 317);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(62, 20);
            this.numericUpDown1.TabIndex = 29;
            // 
            // detectorLabel
            // 
            this.detectorLabel.AutoSize = true;
            this.detectorLabel.Location = new System.Drawing.Point(12, 319);
            this.detectorLabel.Name = "detectorLabel";
            this.detectorLabel.Size = new System.Drawing.Size(115, 13);
            this.detectorLabel.TabIndex = 30;
            this.detectorLabel.Text = "Silence Detector Time:";
            // 
            // secondsLabel
            // 
            this.secondsLabel.AutoSize = true;
            this.secondsLabel.Location = new System.Drawing.Point(212, 319);
            this.secondsLabel.Name = "secondsLabel";
            this.secondsLabel.Size = new System.Drawing.Size(49, 13);
            this.secondsLabel.TabIndex = 31;
            this.secondsLabel.Text = "Seconds";
            // 
            // scheduleSeperator
            // 
            this.scheduleSeperator.Label = "Scheduled Changes";
            this.scheduleSeperator.Location = new System.Drawing.Point(324, 272);
            this.scheduleSeperator.Name = "scheduleSeperator";
            this.scheduleSeperator.Size = new System.Drawing.Size(306, 15);
            this.scheduleSeperator.TabIndex = 25;
            // 
            // outputRightMeter
            // 
            this.outputRightMeter.Amplitude = -60F;
            this.outputRightMeter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.outputRightMeter.Location = new System.Drawing.Point(327, 250);
            this.outputRightMeter.MaxDb = 0F;
            this.outputRightMeter.MinDb = -60F;
            this.outputRightMeter.Name = "outputRightMeter";
            this.outputRightMeter.Size = new System.Drawing.Size(303, 16);
            this.outputRightMeter.TabIndex = 24;
            this.outputRightMeter.Text = "volumeMeter1";
            // 
            // outputLeftMeter
            // 
            this.outputLeftMeter.Amplitude = -60F;
            this.outputLeftMeter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.outputLeftMeter.Location = new System.Drawing.Point(327, 229);
            this.outputLeftMeter.MaxDb = 0F;
            this.outputLeftMeter.MinDb = -60F;
            this.outputLeftMeter.Name = "outputLeftMeter";
            this.outputLeftMeter.Size = new System.Drawing.Size(303, 16);
            this.outputLeftMeter.TabIndex = 23;
            this.outputLeftMeter.Text = "volumeMeter1";
            // 
            // sourceRightMeter
            // 
            this.sourceRightMeter.Amplitude = -60F;
            this.sourceRightMeter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.sourceRightMeter.Location = new System.Drawing.Point(327, 194);
            this.sourceRightMeter.MaxDb = 0F;
            this.sourceRightMeter.MinDb = -60F;
            this.sourceRightMeter.Name = "sourceRightMeter";
            this.sourceRightMeter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.sourceRightMeter.Size = new System.Drawing.Size(303, 16);
            this.sourceRightMeter.TabIndex = 21;
            // 
            // sourceLeftMeter
            // 
            this.sourceLeftMeter.Amplitude = -60F;
            this.sourceLeftMeter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.sourceLeftMeter.Location = new System.Drawing.Point(327, 173);
            this.sourceLeftMeter.MaxDb = 0F;
            this.sourceLeftMeter.MinDb = -60F;
            this.sourceLeftMeter.Name = "sourceLeftMeter";
            this.sourceLeftMeter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.sourceLeftMeter.Size = new System.Drawing.Size(303, 16);
            this.sourceLeftMeter.TabIndex = 20;
            // 
            // LevelsSeperator
            // 
            this.LevelsSeperator.Label = "Audio Levels";
            this.LevelsSeperator.Location = new System.Drawing.Point(324, 139);
            this.LevelsSeperator.Name = "LevelsSeperator";
            this.LevelsSeperator.Size = new System.Drawing.Size(306, 15);
            this.LevelsSeperator.TabIndex = 18;
            // 
            // settingsSeperator
            // 
            this.settingsSeperator.Label = "Settings";
            this.settingsSeperator.Location = new System.Drawing.Point(12, 139);
            this.settingsSeperator.Name = "settingsSeperator";
            this.settingsSeperator.Size = new System.Drawing.Size(306, 15);
            this.settingsSeperator.TabIndex = 5;
            // 
            // sourceSeperator
            // 
            this.sourceSeperator.Label = "Source Selection";
            this.sourceSeperator.Location = new System.Drawing.Point(12, 12);
            this.sourceSeperator.Name = "sourceSeperator";
            this.sourceSeperator.Size = new System.Drawing.Size(618, 15);
            this.sourceSeperator.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(642, 393);
            this.Controls.Add(this.secondsLabel);
            this.Controls.Add(this.detectorLabel);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.scheduleListView);
            this.Controls.Add(this.scheduleSeperator);
            this.Controls.Add(this.outputRightMeter);
            this.Controls.Add(this.outputLeftMeter);
            this.Controls.Add(this.outputLevelLabel);
            this.Controls.Add(this.sourceRightMeter);
            this.Controls.Add(this.sourceLeftMeter);
            this.Controls.Add(this.sourceLevelLabel);
            this.Controls.Add(this.LevelsSeperator);
            this.Controls.Add(this.processorConfigButton);
            this.Controls.Add(this.processorComboBox);
            this.Controls.Add(this.processorLabel);
            this.Controls.Add(this.fileLabel);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.emergencyFileLabel);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.obUrlLabel);
            this.Controls.Add(this.chartUrlTextBox);
            this.Controls.Add(this.ChartUrlLabel);
            this.Controls.Add(this.studioInputComboBox);
            this.Controls.Add(this.studioInputLabel);
            this.Controls.Add(this.settingsSeperator);
            this.Controls.Add(this.sourceSeperator);
            this.Controls.Add(this.emergencyButton);
            this.Controls.Add(this.obButton);
            this.Controls.Add(this.chartButton);
            this.Controls.Add(this.studioButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "ShockRouter";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button studioButton;
        private System.Windows.Forms.Button chartButton;
        private System.Windows.Forms.Button obButton;
        private System.Windows.Forms.Button emergencyButton;
        private ShockCast.Seperator sourceSeperator;
        private ShockCast.Seperator settingsSeperator;
        private System.Windows.Forms.Label studioInputLabel;
        private System.Windows.Forms.ComboBox studioInputComboBox;
        private System.Windows.Forms.Label ChartUrlLabel;
        private System.Windows.Forms.TextBox chartUrlTextBox;
        private System.Windows.Forms.Label obUrlLabel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label emergencyFileLabel;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label fileLabel;
        private System.Windows.Forms.Label processorLabel;
        private System.Windows.Forms.ComboBox processorComboBox;
        private System.Windows.Forms.Button processorConfigButton;
        private ShockCast.Seperator LevelsSeperator;
        private System.Windows.Forms.Label sourceLevelLabel;
        private ShockCast.VolumeMeter sourceLeftMeter;
        private ShockCast.VolumeMeter sourceRightMeter;
        private System.Windows.Forms.Label outputLevelLabel;
        private ShockCast.VolumeMeter outputRightMeter;
        private ShockCast.VolumeMeter outputLeftMeter;
        private ShockCast.Seperator scheduleSeperator;
        private System.Windows.Forms.ListView scheduleListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label detectorLabel;
        private System.Windows.Forms.Label secondsLabel;
    }
}

