namespace AiPathFinding.View
{
    partial class Status
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
            this.grpEntities = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPlayerPosition = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTargetPosition = new System.Windows.Forms.TextBox();
            this.txtMinimalDistance = new System.Windows.Forms.TextBox();
            this.txtManhattenDistance = new System.Windows.Forms.TextBox();
            this.grpEntities.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpEntities
            // 
            this.grpEntities.Controls.Add(this.txtManhattenDistance);
            this.grpEntities.Controls.Add(this.txtMinimalDistance);
            this.grpEntities.Controls.Add(this.txtTargetPosition);
            this.grpEntities.Controls.Add(this.label4);
            this.grpEntities.Controls.Add(this.label3);
            this.grpEntities.Controls.Add(this.label2);
            this.grpEntities.Controls.Add(this.txtPlayerPosition);
            this.grpEntities.Controls.Add(this.label1);
            this.grpEntities.Location = new System.Drawing.Point(3, 3);
            this.grpEntities.Name = "grpEntities";
            this.grpEntities.Size = new System.Drawing.Size(200, 194);
            this.grpEntities.TabIndex = 0;
            this.grpEntities.TabStop = false;
            this.grpEntities.Text = "Entitites";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Player Position";
            // 
            // txtPlayerPosition
            // 
            this.txtPlayerPosition.Enabled = false;
            this.txtPlayerPosition.Location = new System.Drawing.Point(115, 19);
            this.txtPlayerPosition.Name = "txtPlayerPosition";
            this.txtPlayerPosition.Size = new System.Drawing.Size(79, 20);
            this.txtPlayerPosition.TabIndex = 1;
            this.txtPlayerPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Target Position";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Minimal Distance";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Manhatten Distance";
            // 
            // txtTargetPosition
            // 
            this.txtTargetPosition.Enabled = false;
            this.txtTargetPosition.Location = new System.Drawing.Point(115, 45);
            this.txtTargetPosition.Name = "txtTargetPosition";
            this.txtTargetPosition.Size = new System.Drawing.Size(79, 20);
            this.txtTargetPosition.TabIndex = 7;
            this.txtTargetPosition.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtMinimalDistance
            // 
            this.txtMinimalDistance.Enabled = false;
            this.txtMinimalDistance.Location = new System.Drawing.Point(115, 71);
            this.txtMinimalDistance.Name = "txtMinimalDistance";
            this.txtMinimalDistance.Size = new System.Drawing.Size(79, 20);
            this.txtMinimalDistance.TabIndex = 8;
            this.txtMinimalDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtManhattenDistance
            // 
            this.txtManhattenDistance.Enabled = false;
            this.txtManhattenDistance.Location = new System.Drawing.Point(115, 97);
            this.txtManhattenDistance.Name = "txtManhattenDistance";
            this.txtManhattenDistance.Size = new System.Drawing.Size(79, 20);
            this.txtManhattenDistance.TabIndex = 9;
            this.txtManhattenDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Status
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.grpEntities);
            this.Name = "Status";
            this.Size = new System.Drawing.Size(206, 242);
            this.grpEntities.ResumeLayout(false);
            this.grpEntities.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpEntities;
        private System.Windows.Forms.TextBox txtPlayerPosition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtManhattenDistance;
        private System.Windows.Forms.TextBox txtMinimalDistance;
        private System.Windows.Forms.TextBox txtTargetPosition;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}
