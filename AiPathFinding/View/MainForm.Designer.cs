namespace AiPathFinding.View
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._canvas = new System.Windows.Forms.Panel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.butAlgorithmSettings = new System.Windows.Forms.ToolStripButton();
            this.butMapSettings = new System.Windows.Forms.ToolStripButton();
            this.butStatus = new System.Windows.Forms.ToolStripButton();
            this.status = new AiPathFinding.View.Status();
            this.algorithmSettings = new AiPathFinding.View.AlgorithmSettings();
            this.mapSettings = new AiPathFinding.View.MapSettings();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _canvas
            // 
            this._canvas.BackColor = System.Drawing.Color.White;
            this._canvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._canvas.Location = new System.Drawing.Point(243, 12);
            this._canvas.Name = "_canvas";
            this._canvas.Size = new System.Drawing.Size(200, 100);
            this._canvas.TabIndex = 0;
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.butAlgorithmSettings,
            this.butMapSettings,
            this.butStatus});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(32, 885);
            this.toolStrip.TabIndex = 4;
            this.toolStrip.Text = "toolStrip1";
            this.toolStrip.TextDirection = System.Windows.Forms.ToolStripTextDirection.Vertical270;
            // 
            // butAlgorithmSettings
            // 
            this.butAlgorithmSettings.Checked = true;
            this.butAlgorithmSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.butAlgorithmSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.butAlgorithmSettings.Image = ((System.Drawing.Image)(resources.GetObject("butAlgorithmSettings.Image")));
            this.butAlgorithmSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.butAlgorithmSettings.Name = "butAlgorithmSettings";
            this.butAlgorithmSettings.Size = new System.Drawing.Size(29, 98);
            this.butAlgorithmSettings.Text = "Algorithm Settings";
            this.butAlgorithmSettings.Click += new System.EventHandler(this.butAlgorithmSettings_Click);
            // 
            // butMapSettings
            // 
            this.butMapSettings.Checked = true;
            this.butMapSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.butMapSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.butMapSettings.Image = ((System.Drawing.Image)(resources.GetObject("butMapSettings.Image")));
            this.butMapSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.butMapSettings.Name = "butMapSettings";
            this.butMapSettings.Size = new System.Drawing.Size(29, 73);
            this.butMapSettings.Text = "Map Settings";
            this.butMapSettings.Click += new System.EventHandler(this.butMapSettings_Click);
            // 
            // butStatus
            // 
            this.butStatus.Checked = true;
            this.butStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.butStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.butStatus.Image = ((System.Drawing.Image)(resources.GetObject("butStatus.Image")));
            this.butStatus.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.butStatus.Name = "butStatus";
            this.butStatus.Size = new System.Drawing.Size(29, 65);
            this.butStatus.Text = "Status Info";
            this.butStatus.Click += new System.EventHandler(this.butStatus_Click);
            // 
            // status
            // 
            this.status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.status.Location = new System.Drawing.Point(29, 694);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(208, 340);
            this.status.TabIndex = 3;
            // 
            // algorithmSettings
            // 
            this.algorithmSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.algorithmSettings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.algorithmSettings.Location = new System.Drawing.Point(29, 12);
            this.algorithmSettings.Name = "algorithmSettings";
            this.algorithmSettings.Size = new System.Drawing.Size(208, 300);
            this.algorithmSettings.TabIndex = 1;
            // 
            // mapSettings
            // 
            this.mapSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.mapSettings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapSettings.Location = new System.Drawing.Point(29, 318);
            this.mapSettings.Name = "mapSettings";
            this.mapSettings.Size = new System.Drawing.Size(208, 370);
            this.mapSettings.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1039, 885);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.status);
            this.Controls.Add(this.algorithmSettings);
            this.Controls.Add(this.mapSettings);
            this.Controls.Add(this._canvas);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "AI Path Finding";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel _canvas;
        private MapSettings mapSettings;
        private AlgorithmSettings algorithmSettings;
        private Status status;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton butAlgorithmSettings;
        private System.Windows.Forms.ToolStripButton butMapSettings;
        private System.Windows.Forms.ToolStripButton butStatus;








    }
}

