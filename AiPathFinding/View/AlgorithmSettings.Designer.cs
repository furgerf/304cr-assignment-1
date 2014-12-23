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
            this.grpPrimaryAlgorithm = new System.Windows.Forms.GroupBox();
            this.comPrimaryAlgorithm = new System.Windows.Forms.ComboBox();
            this.butStart = new System.Windows.Forms.Button();
            this.grpPlayback = new System.Windows.Forms.GroupBox();
            this.labExplored = new System.Windows.Forms.Label();
            this.progressSteps = new System.Windows.Forms.ProgressBar();
            this.labStep = new System.Windows.Forms.Label();
            this.butLast = new System.Windows.Forms.Button();
            this.butNext = new System.Windows.Forms.Button();
            this.butPrevious = new System.Windows.Forms.Button();
            this.butFirst = new System.Windows.Forms.Button();
            this.butClear = new System.Windows.Forms.Button();
            this.grpSecondaryAlgorithm = new System.Windows.Forms.GroupBox();
            this.comSecondaryAlgorithm = new System.Windows.Forms.ComboBox();
            this.butRestart = new System.Windows.Forms.Button();
            this.grpFogSelection = new System.Windows.Forms.GroupBox();
            this.comFogMethod = new System.Windows.Forms.ComboBox();
            this.grpPrimaryAlgorithm.SuspendLayout();
            this.grpPlayback.SuspendLayout();
            this.grpSecondaryAlgorithm.SuspendLayout();
            this.grpFogSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPrimaryAlgorithm
            // 
            this.grpPrimaryAlgorithm.Controls.Add(this.comPrimaryAlgorithm);
            this.grpPrimaryAlgorithm.Location = new System.Drawing.Point(3, 3);
            this.grpPrimaryAlgorithm.Name = "grpPrimaryAlgorithm";
            this.grpPrimaryAlgorithm.Size = new System.Drawing.Size(200, 46);
            this.grpPrimaryAlgorithm.TabIndex = 0;
            this.grpPrimaryAlgorithm.TabStop = false;
            this.grpPrimaryAlgorithm.Text = "Select primary algorithm";
            // 
            // comPrimaryAlgorithm
            // 
            this.comPrimaryAlgorithm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comPrimaryAlgorithm.FormattingEnabled = true;
            this.comPrimaryAlgorithm.Location = new System.Drawing.Point(6, 19);
            this.comPrimaryAlgorithm.Name = "comPrimaryAlgorithm";
            this.comPrimaryAlgorithm.Size = new System.Drawing.Size(188, 21);
            this.comPrimaryAlgorithm.TabIndex = 0;
            this.comPrimaryAlgorithm.SelectedIndexChanged += new System.EventHandler(this.comPrimaryAlgorithm_SelectedIndexChanged);
            // 
            // butStart
            // 
            this.butStart.Location = new System.Drawing.Point(3, 259);
            this.butStart.Name = "butStart";
            this.butStart.Size = new System.Drawing.Size(62, 23);
            this.butStart.TabIndex = 1;
            this.butStart.Text = "Start";
            this.butStart.UseVisualStyleBackColor = true;
            this.butStart.Click += new System.EventHandler(this.butStart_Click);
            // 
            // grpPlayback
            // 
            this.grpPlayback.Controls.Add(this.labExplored);
            this.grpPlayback.Controls.Add(this.progressSteps);
            this.grpPlayback.Controls.Add(this.labStep);
            this.grpPlayback.Controls.Add(this.butLast);
            this.grpPlayback.Controls.Add(this.butNext);
            this.grpPlayback.Controls.Add(this.butPrevious);
            this.grpPlayback.Controls.Add(this.butFirst);
            this.grpPlayback.Enabled = false;
            this.grpPlayback.Location = new System.Drawing.Point(3, 159);
            this.grpPlayback.Name = "grpPlayback";
            this.grpPlayback.Size = new System.Drawing.Size(200, 94);
            this.grpPlayback.TabIndex = 2;
            this.grpPlayback.TabStop = false;
            this.grpPlayback.Text = "Algorithm Playback";
            // 
            // labExplored
            // 
            this.labExplored.AutoSize = true;
            this.labExplored.Location = new System.Drawing.Point(6, 76);
            this.labExplored.Name = "labExplored";
            this.labExplored.Size = new System.Drawing.Size(98, 13);
            this.labExplored.TabIndex = 6;
            this.labExplored.Text = "(No exploration yet)";
            // 
            // progressSteps
            // 
            this.progressSteps.Location = new System.Drawing.Point(6, 32);
            this.progressSteps.Name = "progressSteps";
            this.progressSteps.Size = new System.Drawing.Size(188, 12);
            this.progressSteps.TabIndex = 5;
            // 
            // labStep
            // 
            this.labStep.AutoSize = true;
            this.labStep.Location = new System.Drawing.Point(6, 16);
            this.labStep.Name = "labStep";
            this.labStep.Size = new System.Drawing.Size(95, 13);
            this.labStep.TabIndex = 4;
            this.labStep.Text = "(No steps to show)";
            // 
            // butLast
            // 
            this.butLast.Location = new System.Drawing.Point(154, 50);
            this.butLast.Name = "butLast";
            this.butLast.Size = new System.Drawing.Size(40, 23);
            this.butLast.TabIndex = 3;
            this.butLast.Text = "Last";
            this.butLast.UseVisualStyleBackColor = true;
            this.butLast.Click += new System.EventHandler(this.butLast_Click);
            // 
            // butNext
            // 
            this.butNext.Location = new System.Drawing.Point(108, 50);
            this.butNext.Name = "butNext";
            this.butNext.Size = new System.Drawing.Size(40, 23);
            this.butNext.TabIndex = 2;
            this.butNext.Text = "Next";
            this.butNext.UseVisualStyleBackColor = true;
            this.butNext.Click += new System.EventHandler(this.butNext_Click);
            // 
            // butPrevious
            // 
            this.butPrevious.Location = new System.Drawing.Point(52, 50);
            this.butPrevious.Name = "butPrevious";
            this.butPrevious.Size = new System.Drawing.Size(40, 23);
            this.butPrevious.TabIndex = 1;
            this.butPrevious.Text = "Prev.";
            this.butPrevious.UseVisualStyleBackColor = true;
            this.butPrevious.Click += new System.EventHandler(this.butPrevious_Click);
            // 
            // butFirst
            // 
            this.butFirst.Location = new System.Drawing.Point(6, 50);
            this.butFirst.Name = "butFirst";
            this.butFirst.Size = new System.Drawing.Size(40, 23);
            this.butFirst.TabIndex = 0;
            this.butFirst.Text = "First";
            this.butFirst.UseVisualStyleBackColor = true;
            this.butFirst.Click += new System.EventHandler(this.butFirst_Click);
            // 
            // butClear
            // 
            this.butClear.Enabled = false;
            this.butClear.Location = new System.Drawing.Point(139, 259);
            this.butClear.Name = "butClear";
            this.butClear.Size = new System.Drawing.Size(62, 23);
            this.butClear.TabIndex = 3;
            this.butClear.Text = "Clear";
            this.butClear.UseVisualStyleBackColor = true;
            this.butClear.Click += new System.EventHandler(this.butClear_Click);
            // 
            // grpSecondaryAlgorithm
            // 
            this.grpSecondaryAlgorithm.Controls.Add(this.comSecondaryAlgorithm);
            this.grpSecondaryAlgorithm.Location = new System.Drawing.Point(3, 55);
            this.grpSecondaryAlgorithm.Name = "grpSecondaryAlgorithm";
            this.grpSecondaryAlgorithm.Size = new System.Drawing.Size(200, 46);
            this.grpSecondaryAlgorithm.TabIndex = 4;
            this.grpSecondaryAlgorithm.TabStop = false;
            this.grpSecondaryAlgorithm.Text = "Select secondary algorithm";
            // 
            // comSecondaryAlgorithm
            // 
            this.comSecondaryAlgorithm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comSecondaryAlgorithm.FormattingEnabled = true;
            this.comSecondaryAlgorithm.Location = new System.Drawing.Point(6, 19);
            this.comSecondaryAlgorithm.Name = "comSecondaryAlgorithm";
            this.comSecondaryAlgorithm.Size = new System.Drawing.Size(188, 21);
            this.comSecondaryAlgorithm.TabIndex = 0;
            this.comSecondaryAlgorithm.SelectedIndexChanged += new System.EventHandler(this.comSecondaryAlgorithm_SelectedIndexChanged);
            // 
            // butRestart
            // 
            this.butRestart.Enabled = false;
            this.butRestart.Location = new System.Drawing.Point(71, 259);
            this.butRestart.Name = "butRestart";
            this.butRestart.Size = new System.Drawing.Size(62, 23);
            this.butRestart.TabIndex = 5;
            this.butRestart.Text = "Restart";
            this.butRestart.UseVisualStyleBackColor = true;
            this.butRestart.Click += new System.EventHandler(this.butRestart_Click);
            // 
            // grpFogSelection
            // 
            this.grpFogSelection.Controls.Add(this.comFogMethod);
            this.grpFogSelection.Location = new System.Drawing.Point(3, 107);
            this.grpFogSelection.Name = "grpFogSelection";
            this.grpFogSelection.Size = new System.Drawing.Size(200, 46);
            this.grpFogSelection.TabIndex = 6;
            this.grpFogSelection.TabStop = false;
            this.grpFogSelection.Text = "Select method to pick fog";
            // 
            // comFogMethod
            // 
            this.comFogMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comFogMethod.FormattingEnabled = true;
            this.comFogMethod.Location = new System.Drawing.Point(6, 19);
            this.comFogMethod.Name = "comFogMethod";
            this.comFogMethod.Size = new System.Drawing.Size(188, 21);
            this.comFogMethod.TabIndex = 0;
            this.comFogMethod.SelectedIndexChanged += new System.EventHandler(this.comFogMethod_SelectedIndexChanged);
            // 
            // AlgorithmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.grpFogSelection);
            this.Controls.Add(this.butRestart);
            this.Controls.Add(this.grpSecondaryAlgorithm);
            this.Controls.Add(this.butClear);
            this.Controls.Add(this.grpPlayback);
            this.Controls.Add(this.butStart);
            this.Controls.Add(this.grpPrimaryAlgorithm);
            this.Name = "AlgorithmSettings";
            this.Size = new System.Drawing.Size(206, 287);
            this.grpPrimaryAlgorithm.ResumeLayout(false);
            this.grpPlayback.ResumeLayout(false);
            this.grpPlayback.PerformLayout();
            this.grpSecondaryAlgorithm.ResumeLayout(false);
            this.grpFogSelection.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpPrimaryAlgorithm;
        private System.Windows.Forms.ComboBox comPrimaryAlgorithm;
        private System.Windows.Forms.Button butStart;
        private System.Windows.Forms.GroupBox grpPlayback;
        private System.Windows.Forms.Button butFirst;
        private System.Windows.Forms.Button butLast;
        private System.Windows.Forms.Button butNext;
        private System.Windows.Forms.Button butPrevious;
        private System.Windows.Forms.ProgressBar progressSteps;
        private System.Windows.Forms.Label labStep;
        private System.Windows.Forms.Button butClear;
        private System.Windows.Forms.GroupBox grpSecondaryAlgorithm;
        private System.Windows.Forms.ComboBox comSecondaryAlgorithm;
        private System.Windows.Forms.Button butRestart;
        private System.Windows.Forms.Label labExplored;
        private System.Windows.Forms.GroupBox grpFogSelection;
        private System.Windows.Forms.ComboBox comFogMethod;
    }
}
