namespace RouterClient
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.sourceSeperator = new ShockCast.Seperator();
            this.emergencyButton = new System.Windows.Forms.Button();
            this.obButton = new System.Windows.Forms.Button();
            this.chartButton = new System.Windows.Forms.Button();
            this.studioButton = new System.Windows.Forms.Button();
            this.settingsSeperator = new ShockCast.Seperator();
            this.ipTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // sourceSeperator
            // 
            this.sourceSeperator.Label = "Source Selection";
            this.sourceSeperator.Location = new System.Drawing.Point(12, 12);
            this.sourceSeperator.Name = "sourceSeperator";
            this.sourceSeperator.Size = new System.Drawing.Size(618, 15);
            this.sourceSeperator.TabIndex = 5;
            // 
            // emergencyButton
            // 
            this.emergencyButton.CausesValidation = false;
            this.emergencyButton.Location = new System.Drawing.Point(480, 33);
            this.emergencyButton.Name = "emergencyButton";
            this.emergencyButton.Size = new System.Drawing.Size(150, 100);
            this.emergencyButton.TabIndex = 9;
            this.emergencyButton.Tag = "";
            this.emergencyButton.Text = "Emergency Output";
            this.emergencyButton.UseVisualStyleBackColor = true;
            this.emergencyButton.Click += new System.EventHandler(this.emergencyButton_Click);
            // 
            // obButton
            // 
            this.obButton.CausesValidation = false;
            this.obButton.Location = new System.Drawing.Point(324, 33);
            this.obButton.Name = "obButton";
            this.obButton.Size = new System.Drawing.Size(150, 100);
            this.obButton.TabIndex = 8;
            this.obButton.Tag = "";
            this.obButton.Text = "Outside Broadcast";
            this.obButton.UseVisualStyleBackColor = true;
            this.obButton.Click += new System.EventHandler(this.obButton_Click);
            // 
            // chartButton
            // 
            this.chartButton.CausesValidation = false;
            this.chartButton.Location = new System.Drawing.Point(168, 33);
            this.chartButton.Name = "chartButton";
            this.chartButton.Size = new System.Drawing.Size(150, 100);
            this.chartButton.TabIndex = 7;
            this.chartButton.Tag = "";
            this.chartButton.Text = "SRA Chart Show";
            this.chartButton.UseVisualStyleBackColor = true;
            this.chartButton.Click += new System.EventHandler(this.chartButton_Click);
            // 
            // studioButton
            // 
            this.studioButton.CausesValidation = false;
            this.studioButton.Location = new System.Drawing.Point(12, 33);
            this.studioButton.Name = "studioButton";
            this.studioButton.Size = new System.Drawing.Size(150, 100);
            this.studioButton.TabIndex = 6;
            this.studioButton.Tag = "";
            this.studioButton.Text = "Studio";
            this.studioButton.UseVisualStyleBackColor = true;
            this.studioButton.Click += new System.EventHandler(this.studioButton_Click);
            // 
            // settingsSeperator
            // 
            this.settingsSeperator.Label = "Server IP";
            this.settingsSeperator.Location = new System.Drawing.Point(12, 139);
            this.settingsSeperator.Name = "settingsSeperator";
            this.settingsSeperator.Size = new System.Drawing.Size(618, 15);
            this.settingsSeperator.TabIndex = 10;
            // 
            // ipTextBox
            // 
            this.ipTextBox.Location = new System.Drawing.Point(12, 160);
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.Size = new System.Drawing.Size(618, 20);
            this.ipTextBox.TabIndex = 11;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(642, 194);
            this.Controls.Add(this.ipTextBox);
            this.Controls.Add(this.settingsSeperator);
            this.Controls.Add(this.sourceSeperator);
            this.Controls.Add(this.emergencyButton);
            this.Controls.Add(this.obButton);
            this.Controls.Add(this.chartButton);
            this.Controls.Add(this.studioButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "ShockRouter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ShockCast.Seperator sourceSeperator;
        private System.Windows.Forms.Button emergencyButton;
        private System.Windows.Forms.Button obButton;
        private System.Windows.Forms.Button chartButton;
        private System.Windows.Forms.Button studioButton;
        private ShockCast.Seperator settingsSeperator;
        private System.Windows.Forms.TextBox ipTextBox;


    }
}

