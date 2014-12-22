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
            this.label3 = new System.Windows.Forms.Label();
            this.numCellSize = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numMapHeight = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numMapWidth = new System.Windows.Forms.NumericUpDown();
            this.grpSaveLoad = new System.Windows.Forms.GroupBox();
            this.butLoadMap = new System.Windows.Forms.Button();
            this.butSaveMap = new System.Windows.Forms.Button();
            this.grpGenerate = new System.Windows.Forms.GroupBox();
            this.butGenerate = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.numFog = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numMountain = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numHill = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numForest = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numPlains = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numStreet = new System.Windows.Forms.NumericUpDown();
            this.grpMapSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCellSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMapHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMapWidth)).BeginInit();
            this.grpSaveLoad.SuspendLayout();
            this.grpGenerate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMountain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numForest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlains)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStreet)).BeginInit();
            this.SuspendLayout();
            // 
            // grpMapSize
            // 
            this.grpMapSize.Controls.Add(this.label3);
            this.grpMapSize.Controls.Add(this.numCellSize);
            this.grpMapSize.Controls.Add(this.label2);
            this.grpMapSize.Controls.Add(this.numMapHeight);
            this.grpMapSize.Controls.Add(this.label1);
            this.grpMapSize.Controls.Add(this.numMapWidth);
            this.grpMapSize.Location = new System.Drawing.Point(3, 3);
            this.grpMapSize.Name = "grpMapSize";
            this.grpMapSize.Size = new System.Drawing.Size(200, 97);
            this.grpMapSize.TabIndex = 0;
            this.grpMapSize.TabStop = false;
            this.grpMapSize.Text = "Map Size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Cell Size";
            // 
            // numCellSize
            // 
            this.numCellSize.Location = new System.Drawing.Point(122, 71);
            this.numCellSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCellSize.Name = "numCellSize";
            this.numCellSize.Size = new System.Drawing.Size(72, 20);
            this.numCellSize.TabIndex = 4;
            this.numCellSize.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
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
            9,
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
            12,
            0,
            0,
            0});
            // 
            // grpSaveLoad
            // 
            this.grpSaveLoad.Controls.Add(this.butLoadMap);
            this.grpSaveLoad.Controls.Add(this.butSaveMap);
            this.grpSaveLoad.Location = new System.Drawing.Point(3, 106);
            this.grpSaveLoad.Name = "grpSaveLoad";
            this.grpSaveLoad.Size = new System.Drawing.Size(200, 49);
            this.grpSaveLoad.TabIndex = 1;
            this.grpSaveLoad.TabStop = false;
            this.grpSaveLoad.Text = "Save/Load";
            // 
            // butLoadMap
            // 
            this.butLoadMap.Location = new System.Drawing.Point(103, 19);
            this.butLoadMap.Name = "butLoadMap";
            this.butLoadMap.Size = new System.Drawing.Size(91, 23);
            this.butLoadMap.TabIndex = 1;
            this.butLoadMap.Text = "Load Map";
            this.butLoadMap.UseVisualStyleBackColor = true;
            // 
            // butSaveMap
            // 
            this.butSaveMap.Location = new System.Drawing.Point(6, 19);
            this.butSaveMap.Name = "butSaveMap";
            this.butSaveMap.Size = new System.Drawing.Size(91, 23);
            this.butSaveMap.TabIndex = 0;
            this.butSaveMap.Text = "Save Map";
            this.butSaveMap.UseVisualStyleBackColor = true;
            // 
            // grpGenerate
            // 
            this.grpGenerate.Controls.Add(this.butGenerate);
            this.grpGenerate.Controls.Add(this.label9);
            this.grpGenerate.Controls.Add(this.numFog);
            this.grpGenerate.Controls.Add(this.label8);
            this.grpGenerate.Controls.Add(this.numMountain);
            this.grpGenerate.Controls.Add(this.label7);
            this.grpGenerate.Controls.Add(this.numHill);
            this.grpGenerate.Controls.Add(this.label6);
            this.grpGenerate.Controls.Add(this.numForest);
            this.grpGenerate.Controls.Add(this.label5);
            this.grpGenerate.Controls.Add(this.numPlains);
            this.grpGenerate.Controls.Add(this.label4);
            this.grpGenerate.Controls.Add(this.numStreet);
            this.grpGenerate.Location = new System.Drawing.Point(3, 161);
            this.grpGenerate.Name = "grpGenerate";
            this.grpGenerate.Size = new System.Drawing.Size(200, 204);
            this.grpGenerate.TabIndex = 2;
            this.grpGenerate.TabStop = false;
            this.grpGenerate.Text = "Regenerate Map";
            // 
            // butGenerate
            // 
            this.butGenerate.Location = new System.Drawing.Point(6, 175);
            this.butGenerate.Name = "butGenerate";
            this.butGenerate.Size = new System.Drawing.Size(188, 23);
            this.butGenerate.TabIndex = 12;
            this.butGenerate.Text = "Generate!";
            this.butGenerate.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 151);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(25, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Fog";
            // 
            // numFog
            // 
            this.numFog.DecimalPlaces = 1;
            this.numFog.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numFog.Location = new System.Drawing.Point(122, 149);
            this.numFog.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numFog.Name = "numFog";
            this.numFog.Size = new System.Drawing.Size(72, 20);
            this.numFog.TabIndex = 10;
            this.numFog.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.numFog.ValueChanged += new System.EventHandler(this.numFog_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 125);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Mountain";
            // 
            // numMountain
            // 
            this.numMountain.Location = new System.Drawing.Point(122, 123);
            this.numMountain.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numMountain.Name = "numMountain";
            this.numMountain.Size = new System.Drawing.Size(72, 20);
            this.numMountain.TabIndex = 8;
            this.numMountain.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMountain.ValueChanged += new System.EventHandler(this.numMountain_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Hill";
            // 
            // numHill
            // 
            this.numHill.Location = new System.Drawing.Point(122, 97);
            this.numHill.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numHill.Name = "numHill";
            this.numHill.Size = new System.Drawing.Size(72, 20);
            this.numHill.TabIndex = 6;
            this.numHill.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numHill.ValueChanged += new System.EventHandler(this.numHill_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Forest";
            // 
            // numForest
            // 
            this.numForest.Location = new System.Drawing.Point(122, 71);
            this.numForest.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numForest.Name = "numForest";
            this.numForest.Size = new System.Drawing.Size(72, 20);
            this.numForest.TabIndex = 4;
            this.numForest.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numForest.ValueChanged += new System.EventHandler(this.numForest_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Plains";
            // 
            // numPlains
            // 
            this.numPlains.Location = new System.Drawing.Point(122, 45);
            this.numPlains.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numPlains.Name = "numPlains";
            this.numPlains.Size = new System.Drawing.Size(72, 20);
            this.numPlains.TabIndex = 2;
            this.numPlains.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numPlains.ValueChanged += new System.EventHandler(this.numPlains_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Street";
            // 
            // numStreet
            // 
            this.numStreet.Location = new System.Drawing.Point(122, 19);
            this.numStreet.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numStreet.Name = "numStreet";
            this.numStreet.Size = new System.Drawing.Size(72, 20);
            this.numStreet.TabIndex = 0;
            this.numStreet.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numStreet.ValueChanged += new System.EventHandler(this.numStreet_ValueChanged);
            // 
            // MapSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.grpGenerate);
            this.Controls.Add(this.grpSaveLoad);
            this.Controls.Add(this.grpMapSize);
            this.Name = "MapSettings";
            this.Size = new System.Drawing.Size(206, 369);
            this.grpMapSize.ResumeLayout(false);
            this.grpMapSize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCellSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMapHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMapWidth)).EndInit();
            this.grpSaveLoad.ResumeLayout(false);
            this.grpGenerate.ResumeLayout(false);
            this.grpGenerate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMountain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numForest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlains)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStreet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpMapSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numMapHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numMapWidth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numCellSize;
        private System.Windows.Forms.GroupBox grpSaveLoad;
        public System.Windows.Forms.Button butLoadMap;
        public System.Windows.Forms.Button butSaveMap;
        private System.Windows.Forms.GroupBox grpGenerate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numForest;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numPlains;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numStreet;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numFog;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numMountain;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numHill;
        public System.Windows.Forms.Button butGenerate;
    }
}
