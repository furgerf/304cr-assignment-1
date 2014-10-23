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
            this._canvas = new System.Windows.Forms.Panel();
            this.algorithmSettings1 = new AiPathFinding.View.AlgorithmSettings();
            this.mapSettings = new AiPathFinding.View.MapSettings();
            this.status = new AiPathFinding.View.Status();
            this.SuspendLayout();
            // 
            // _canvas
            // 
            this._canvas.BackColor = System.Drawing.Color.White;
            this._canvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._canvas.Location = new System.Drawing.Point(238, 12);
            this._canvas.Name = "_canvas";
            this._canvas.Size = new System.Drawing.Size(200, 100);
            this._canvas.TabIndex = 0;
            // 
            // algorithmSettings1
            // 
            this.algorithmSettings1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.algorithmSettings1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.algorithmSettings1.Location = new System.Drawing.Point(12, 235);
            this.algorithmSettings1.Name = "algorithmSettings1";
            this.algorithmSettings1.Size = new System.Drawing.Size(208, 150);
            this.algorithmSettings1.TabIndex = 2;
            // 
            // mapSettings
            // 
            this.mapSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.mapSettings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapSettings.Location = new System.Drawing.Point(12, 12);
            this.mapSettings.Name = "mapSettings";
            this.mapSettings.Size = new System.Drawing.Size(208, 217);
            this.mapSettings.TabIndex = 1;
            // 
            // status
            // 
            this.status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.status.Location = new System.Drawing.Point(12, 391);
            this.status.Name = "status";
            this.status.PlayerPosition = null;
            this.status.Size = new System.Drawing.Size(208, 181);
            this.status.TabIndex = 3;
            this.status.TargetPosition = null;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1039, 584);
            this.Controls.Add(this.status);
            this.Controls.Add(this.algorithmSettings1);
            this.Controls.Add(this.mapSettings);
            this.Controls.Add(this._canvas);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _canvas;
        private MapSettings mapSettings;
        private AlgorithmSettings algorithmSettings1;
        private Status status;








    }
}

