using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace _3D_Minesweeper
{
    public partial class GameSpace : Form
    {
        private FileInfo _WorkingFile = null;
        private FileInfo WorkingFile
        {
            get
            {
                return _WorkingFile;
            }
            set
            {
                _WorkingFile = value;

                ReText();
            }
        }

        private Board Board
        {
            get
            {
                return Controller.Board;
            }
            set
            {
                if (Controller.Board != null) Controller.Board.OnUpdate -= ReText;

                Controller.Board = value;
            }
        }

        public GameSpace()
        {
            InitializeComponent();

            WorkingFile = null;
        }

        private void ReText()
        {
            if (WorkingFile == null)
                Text = "3D Minesweeper";
            else Text = string.Format("3D Minesweeper - {0}", WorkingFile.FullName);
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Board != null) Board.Paused = true;
            BoardDialog d = new BoardDialog();

            if (d.ShowDialog() == DialogResult.OK)
                Board = new Board(d.Settings);

            d.Dispose();
        }

        private bool SaveAs()
        {
            if (Board == null)
            {
                MessageBox.Show("Nothing to save");
                return false;
            }
            Board.Paused = true;

            SaveFileDialog d = new SaveFileDialog();
            d.Filter = Board.Filter;
            bool r = false;

            if (d.ShowDialog() == DialogResult.OK)
            {
                WorkingFile = new FileInfo(d.FileName);
                r = Save();
            }

            d.Dispose();
            return r;
        }
        private bool Save()
        {
            if (Board == null)
            {
                MessageBox.Show("Nothing to save");
                return false;
            }

            if (WorkingFile == null) return SaveAs();

            try
            {
                Board.Save(WorkingFile);
                return true;
            }
            catch (Exception ex)
            {
                WorkingFile.Delete();
                MessageBox.Show(string.Format("Uhoh, an error occured while saving your file!\n\n{0}", ex));
            }

            return false;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Board != null) Board.Paused = true;
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = Board.Filter;

            if (d.ShowDialog() == DialogResult.OK)
            {
                WorkingFile = new FileInfo(d.FileName);

                try
                {
                    Board b = Board.Load(WorkingFile);
                    if (b == null) throw new FormatException("File is not formatted correctly");

                    Board = b;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Uhoh, an error occured while opening your file!\n\n{0}", ex));
                }
            }

            d.Dispose();
        }

        private void GameSpace_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Board == null) return;
            Board.Paused = true;

            DialogResult r = MessageBox.Show("Wold you like to save before closing? Your progress may be lost.", "Save Before Closing", MessageBoxButtons.YesNoCancel);
            if (r == DialogResult.Yes)
            {
                if (!Save()) e.Cancel = true;
            }
            else if (r == DialogResult.Cancel)
                e.Cancel = true;
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Board != null) Board.Paused = !Board.Paused;
        }

        private void gameToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            if (Board != null) Board.Paused = true;
        }
    }
}
