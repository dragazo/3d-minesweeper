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
    public partial class BoardDisplay : UserControl
    {
        private Board _Board = null;
        public Board Board
        {
            get
            {
                return _Board;
            }
            set
            {
                if (_Board != null) _Board.OnUpdate -= Invalidate;

                _Board = value;
                _FocusLayer = 0;

                if (value != null) _Board.OnUpdate += Invalidate;

                Invalidate();
            }
        }

        private static readonly Point Start = new Point(5, 5);

        private int _FocusLayer = 0;
        public int FocusLayer
        {
            get
            {
                return _FocusLayer;
            }
            set
            {
                _FocusLayer = value;
                Invalidate();
            }
        }

        private Position Hover = Position.Null;

        public BoardDisplay()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Board == null)
                e.Graphics.Clear(Color.LightGray);
            else
                Board.Paint(e, FocusLayer, Hover, Start);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Board == null) return;

            Position now = Hovering();
            if (now == Hover) return;

            Hover = now;
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (Board == null) return;

            ActionResult r = Board.Interact(Hover, e.Button);
            if (r == ActionResult.GameOver || r == ActionResult.Victory)
                Board.Reveal();
        }

        private Position Hovering()
        {
            return Board.Hovering(PointToClient(MousePosition), Start, FocusLayer);
        }

        private void BoardDisplay_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
