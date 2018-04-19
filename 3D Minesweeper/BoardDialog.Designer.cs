namespace _3D_Minesweeper
{
    partial class BoardDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.WidthUpDown = new System.Windows.Forms.NumericUpDown();
            this.HeightUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.DepthUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.MinesUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.DifficultyCombo = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.WidthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeightUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DepthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinesUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Width";
            // 
            // WidthUpDown
            // 
            this.WidthUpDown.Location = new System.Drawing.Point(12, 25);
            this.WidthUpDown.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.WidthUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WidthUpDown.Name = "WidthUpDown";
            this.WidthUpDown.Size = new System.Drawing.Size(120, 20);
            this.WidthUpDown.TabIndex = 1;
            this.WidthUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WidthUpDown.ValueChanged += new System.EventHandler(this.DimensionChanged);
            // 
            // HeightUpDown
            // 
            this.HeightUpDown.Location = new System.Drawing.Point(138, 25);
            this.HeightUpDown.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.HeightUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HeightUpDown.Name = "HeightUpDown";
            this.HeightUpDown.Size = new System.Drawing.Size(120, 20);
            this.HeightUpDown.TabIndex = 3;
            this.HeightUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.HeightUpDown.ValueChanged += new System.EventHandler(this.DimensionChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(138, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Height";
            // 
            // DepthUpDown
            // 
            this.DepthUpDown.Location = new System.Drawing.Point(264, 25);
            this.DepthUpDown.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.DepthUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DepthUpDown.Name = "DepthUpDown";
            this.DepthUpDown.Size = new System.Drawing.Size(120, 20);
            this.DepthUpDown.TabIndex = 5;
            this.DepthUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DepthUpDown.ValueChanged += new System.EventHandler(this.DimensionChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(264, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Depth";
            // 
            // MinesUpDown
            // 
            this.MinesUpDown.Location = new System.Drawing.Point(435, 25);
            this.MinesUpDown.Name = "MinesUpDown";
            this.MinesUpDown.Size = new System.Drawing.Size(120, 20);
            this.MinesUpDown.TabIndex = 7;
            this.MinesUpDown.ValueChanged += new System.EventHandler(this.MinesUpDown_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(435, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Mines";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(341, 86);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // DifficultyCombo
            // 
            this.DifficultyCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DifficultyCombo.FormattingEnabled = true;
            this.DifficultyCombo.Items.AddRange(new object[] {
            "Custom",
            "Easy",
            "Medium",
            "Hard",
            "Insane"});
            this.DifficultyCombo.Location = new System.Drawing.Point(12, 88);
            this.DifficultyCombo.Name = "DifficultyCombo";
            this.DifficultyCombo.Size = new System.Drawing.Size(120, 21);
            this.DifficultyCombo.TabIndex = 8;
            this.DifficultyCombo.SelectedIndexChanged += new System.EventHandler(this.DifficultyCombo_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Difficulty";
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(451, 86);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(104, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // BoardDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 117);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.DifficultyCombo);
            this.Controls.Add(this.MinesUpDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.DepthUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.HeightUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.WidthUpDown);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "BoardDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BoardDialog";
            ((System.ComponentModel.ISupportInitialize)(this.WidthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeightUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DepthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinesUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown WidthUpDown;
        private System.Windows.Forms.NumericUpDown HeightUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown DepthUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown MinesUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox DifficultyCombo;
        private System.Windows.Forms.Button button2;
    }
}