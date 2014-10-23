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
            this.grpPlayback = new System.Windows.Forms.GroupBox();
            this.butFirst = new System.Windows.Forms.Button();
            this.butPrevious = new System.Windows.Forms.Button();
            this.butNext = new System.Windows.Forms.Button();
            this.butLast = new System.Windows.Forms.Button();
            this.grpKnownAreaAlgorithm.SuspendLayout();
            this.grpPlayback.SuspendLayout();
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
            // grpPlayback
            // 
            this.grpPlayback.Controls.Add(this.butLast);
            this.grpPlayback.Controls.Add(this.butNext);
            this.grpPlayback.Controls.Add(this.butPrevious);
            this.grpPlayback.Controls.Add(this.butFirst);
            this.grpPlayback.Enabled = false;
            this.grpPlayback.Location = new System.Drawing.Point(3, 84);
            this.grpPlayback.Name = "grpPlayback";
            this.grpPlayback.Size = new System.Drawing.Size(200, 100);
            this.grpPlayback.TabIndex = 2;
            this.grpPlayback.TabStop = false;
            this.grpPlayback.Text = "Algorithm Playback";
            // 
            // butFirst
            // 
            this.butFirst.Location = new System.Drawing.Point(6, 19);
            this.butFirst.Name = "butFirst";
            this.butFirst.Size = new System.Drawing.Size(40, 23);
            this.butFirst.TabIndex = 0;
            this.butFirst.Text = "First";
            this.butFirst.UseVisualStyleBackColor = true;
            this.butFirst.Click += new System.EventHandler(this.butFirst_Click);
            // 
            // butPrevious
            // 
            this.butPrevious.Location = new System.Drawing.Point(52, 19);
            this.butPrevious.Name = "butPrevious";
            this.butPrevious.Size = new System.Drawing.Size(40, 23);
            this.butPrevious.TabIndex = 1;
            this.butPrevious.Text = "Prev.";
            this.butPrevious.UseVisualStyleBackColor = true;
            this.butPrevious.Click += new System.EventHandler(this.butPrevious_Click);
            // 
            // butNext
            // 
            this.butNext.Location = new System.Drawing.Point(98, 19);
            this.butNext.Name = "butNext";
            this.butNext.Size = new System.Drawing.Size(40, 23);
            this.butNext.TabIndex = 2;
            this.butNext.Text = "Next";
            this.butNext.UseVisualStyleBackColor = true;
            this.butNext.Click += new System.EventHandler(this.butNext_Click);
            // 
            // butLast
            // 
            this.butLast.Location = new System.Drawing.Point(144, 19);
            this.butLast.Name = "butLast";
            this.butLast.Size = new System.Drawing.Size(40, 23);
            this.butLast.TabIndex = 3;
            this.butLast.Text = "Last";
            this.butLast.UseVisualStyleBackColor = true;
            this.butLast.Click += new System.EventHandler(this.butLast_Click);
            // 
            // AlgorithmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.grpPlayback);
            this.Controls.Add(this.butStart);
            this.Controls.Add(this.grpKnownAreaAlgorithm);
            this.Name = "AlgorithmSettings";
            this.Size = new System.Drawing.Size(206, 200);
            this.grpKnownAreaAlgorithm.ResumeLayout(false);
            this.grpPlayback.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpKnownAreaAlgorithm;
        private System.Windows.Forms.ComboBox comKnownAreaAlgorithm;
        private System.Windows.Forms.Button butStart;
        private System.Windows.Forms.GroupBox grpPlayback;
        private System.Windows.Forms.Button butFirst;
        private System.Windows.Forms.Button butLast;
        private System.Windows.Forms.Button butNext;
        private System.Windows.Forms.Button butPrevious;
    }
}
