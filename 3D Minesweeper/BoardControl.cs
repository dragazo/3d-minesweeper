using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3D_Minesweeper
{
    public partial class BoardControl : UserControl
    {
        public Board Board
        {
            get
            {
                return Display.Board;
            }
            set
            {
                Display.Board = value;

                if (value != null) FocusBar.Maximum = value.Z - 1;
                FocusBar.Value = 0;
            }
        }

        public BoardControl()
        {
            InitializeComponent();
        }

        private void FocusBar_ValueChanged(object sender, EventArgs e)
        {
            Display.FocusLayer = FocusBar.Value;
        }

        private void BoardControl_Resize(object sender, EventArgs e)
        {
            Display.Size = new Size(Width - Display.Left, Height - Display.Top);
        }
    }
}
