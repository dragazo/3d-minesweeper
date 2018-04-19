using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3D_Minesweeper
{
    public partial class BoardDialog : Form
    {
        private int BoardWidth
        {
            get
            {
                return (int)WidthUpDown.Value;
            }
            set
            {
                WidthUpDown.Value = value;
            }
        }
        private int BoardHeight
        {
            get
            {
                return (int)HeightUpDown.Value;
            }
            set
            {
                HeightUpDown.Value = value;
            }
        }
        private int BoardDepth
        {
            get
            {
                return (int)DepthUpDown.Value;
            }
            set
            {
                DepthUpDown.Value = value;
            }
        }

        private int Mines
        {
            get
            {
                return (int)MinesUpDown.Value;
            }
            set
            {
                MinesUpDown.Value = value;
            }
        }

        public BoardSettings Settings
        {
            get
            {
                switch (DifficultyCombo.SelectedIndex)
                {
                    case 1:
                        return Board.Easy;
                    case 2:
                        return Board.Medium;
                    case 3:
                        return Board.Hard;
                    case 4:
                        return Board.Insane;
                    default:
                        return new BoardSettings(BoardWidth, BoardHeight, BoardDepth, Mines);
                }
            }
            set
            {
                BoardWidth = value.Width;
                BoardHeight = value.Height;
                BoardDepth = value.Depth;

                Mines = value.Mines;
            }
        }

        public BoardDialog()
        {
            InitializeComponent();

            DifficultyCombo.SelectedIndex = 1;
            Settings = Settings;
        }

        private bool SuspendDifficulty = false;

        private void DimensionChanged(object sender, EventArgs e)
        {
            MinesUpDown.Maximum = BoardWidth * BoardHeight * BoardDepth;
            if (!SuspendDifficulty) DifficultyCombo.SelectedIndex = 0;
        }
        private void MinesUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!SuspendDifficulty) DifficultyCombo.SelectedIndex = 0;
        }

        private void DifficultyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DifficultyCombo.SelectedIndex != 0)
            {
                SuspendDifficulty = true;
                Settings = Settings;
                SuspendDifficulty = false;
            }
        }
    }
}
