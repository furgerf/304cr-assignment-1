namespace AiPathFinding.View
{
    partial class MapSettings
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
            this.grpMapSize = new System.Windows.Forms.GroupBox();
            this.numMapWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numMapHeight = new System.Windows.Forms.NumericUpDown();
            this.grpMapSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMapWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMapHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // grpMapSize
            // 
            this.grpMapSize.Controls.Add(this.label2);
            this.grpMapSize.Controls.Add(this.numMapHeight);
            this.grpMapSize.Controls.Add(this.label1);
            this.grpMapSize.Controls.Add(this.numMapWidth);
            this.grpMapSize.Location = new System.Drawing.Point(3, 3);
            this.grpMapSize.Name = "grpMapSize";
            this.grpMapSize.Size = new System.Drawing.Size(200, 75);
            this.grpMapSize.TabIndex = 0;
            this.grpMapSize.TabStop = false;
            this.grpMapSize.Text = "Map Size";
            // 
            // numMapWidth
            // 
            this.numMapWidth.Location = new System.Drawing.Point(122, 19);
            this.numMapWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMapWidth.Name = "numMapWidth";
            this.numMapWidth.Size = new System.Drawing.Size(72, 20);
            this.numMapWidth.TabIndex = 0;
            this.numMapWidth.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Map Width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Map Height";
            // 
            // numMapHeight
            // 
            this.numMapHeight.Location = new System.Drawing.Point(122, 45);
            this.numMapHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMapHeight.Name = "numMapHeight";
            this.numMapHeight.Size = new System.Drawing.Size(72, 20);
            this.numMapHeight.TabIndex = 2;
            this.numMapHeight.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // MapSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.grpMapSize);
            this.Name = "MapSettings";
            this.Size = new System.Drawing.Size(206, 363);
            this.grpMapSize.ResumeLayout(false);
            this.grpMapSize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMapWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMapHeight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpMapSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numMapHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numMapWidth;
    }
}
