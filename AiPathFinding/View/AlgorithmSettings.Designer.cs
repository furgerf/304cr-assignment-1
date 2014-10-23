namespace AiPathFinding.View
{
    partial class AlgorithmSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpKnownAreaAlgorithm = new System.Windows.Forms.GroupBox();
            this.comKnownAreaAlgorithm = new System.Windows.Forms.ComboBox();
            this.butStart = new System.Windows.Forms.Button();
            this.grpKnownAreaAlgorithm.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpKnownAreaAlgorithm
            // 
            this.grpKnownAreaAlgorithm.Controls.Add(this.comKnownAreaAlgorithm);
            this.grpKnownAreaAlgorithm.Location = new System.Drawing.Point(3, 3);
            this.grpKnownAreaAlgorithm.Name = "grpKnownAreaAlgorithm";
            this.grpKnownAreaAlgorithm.Size = new System.Drawing.Size(200, 46);
            this.grpKnownAreaAlgorithm.TabIndex = 0;
            this.grpKnownAreaAlgorithm.TabStop = false;
            this.grpKnownAreaAlgorithm.Text = "Choose algorithm for known area";
            // 
            // comKnownAreaAlgorithm
            // 
            this.comKnownAreaAlgorithm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comKnownAreaAlgorithm.FormattingEnabled = true;
            this.comKnownAreaAlgorithm.Location = new System.Drawing.Point(6, 19);
            this.comKnownAreaAlgorithm.Name = "comKnownAreaAlgorithm";
            this.comKnownAreaAlgorithm.Size = new System.Drawing.Size(188, 21);
            this.comKnownAreaAlgorithm.TabIndex = 0;
            // 
            // butStart
            // 
            this.butStart.Location = new System.Drawing.Point(53, 55);
            this.butStart.Name = "butStart";
            this.butStart.Size = new System.Drawing.Size(75, 23);
            this.butStart.TabIndex = 1;
            this.butStart.Text = "Start!";
            this.butStart.UseVisualStyleBackColor = true;
            this.butStart.Click += new System.EventHandler(this.butStart_Click);
            // 
            // AlgorithmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.butStart);
            this.Controls.Add(this.grpKnownAreaAlgorithm);
            this.Name = "AlgorithmSettings";
            this.Size = new System.Drawing.Size(206, 200);
            this.grpKnownAreaAlgorithm.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpKnownAreaAlgorithm;
        private System.Windows.Forms.ComboBox comKnownAreaAlgorithm;
        private System.Windows.Forms.Button butStart;
    }
}
