namespace _3D_Minesweeper
{
    partial class BoardControl
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
            this.FocusBar = new System.Windows.Forms.TrackBar();
            this.Display = new _3D_Minesweeper.BoardDisplay();
            ((System.ComponentModel.ISupportInitialize)(this.FocusBar)).BeginInit();
            this.SuspendLayout();
            // 
            // FocusBar
            // 
            this.FocusBar.AutoSize = false;
            this.FocusBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.FocusBar.Location = new System.Drawing.Point(0, 0);
            this.FocusBar.Name = "FocusBar";
            this.FocusBar.Size = new System.Drawing.Size(494, 31);
            this.FocusBar.TabIndex = 1;
            this.FocusBar.ValueChanged += new System.EventHandler(this.FocusBar_ValueChanged);
            // 
            // Display
            // 
            this.Display.Board = null;
            this.Display.FocusLayer = 0;
            this.Display.Location = new System.Drawing.Point(0, 28);
            this.Display.Name = "Display";
            this.Display.Size = new System.Drawing.Size(494, 255);
            this.Display.TabIndex = 0;
            // 
            // BoardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Display);
            this.Controls.Add(this.FocusBar);
            this.Name = "BoardControl";
            this.Size = new System.Drawing.Size(494, 283);
            this.Resize += new System.EventHandler(this.BoardControl_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.FocusBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BoardDisplay Display;
        private System.Windows.Forms.TrackBar FocusBar;
    }
}
