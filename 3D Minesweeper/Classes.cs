using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.CompilerServices;

namespace _3D_Minesweeper
{
    #region Board
    //Core
    [Serializable]
    public partial class Board
    {
        private Token[,,] Tokens;
        public readonly int X, Y, Z;
        private bool Enabled = true;

        public int Mines
        {
            get
            {
                int r = 0;
                foreach (Token t in Tokens)
                    if (t is Mine) r++;

                return r;
            }
        }
        public int Marked
        {
            get
            {
                int r = 0;
                foreach (Token t in Tokens)
                    if (t.Marked) r++;

                return r;
            }
        }

        private Token this[Position pos]
        {
            get
            {
                return Tokens[pos.X, pos.Y, pos.Z];
            }
            set
            {
                Tokens[pos.X, pos.Y, pos.Z] = value;
            }
        }

        private IEnumerable<Token> Adjacent(Position pos)
        {
            return Adjacent(pos.X, pos.Y, pos.Z);
        }
        private IEnumerable<Token> Adjacent(int x, int y, int z)
        {
            for (int a = x - 1; a <= x + 1; a++)
                for (int b = y - 1; b <= y + 1; b++)
                    for (int c = z - 1; c <= z + 1; c++)
                        if (a >= 0 && a < X
                            && b >= 0 && b < Y
                            && c >= 0 && c < Z
                            && (a != x || b != y || c != z)) yield return Tokens[a, b, c];
        }

        private IEnumerable<Position> AdjacentPos(Position pos)
        {
            return AdjacentPos(pos.X, pos.Y, pos.Z);
        }
        private IEnumerable<Position> AdjacentPos(int x, int y, int z)
        {
            for (int a = x - 1; a <= x + 1; a++)
                for (int b = y - 1; b <= y + 1; b++)
                    for (int c = z - 1; c <= z + 1; c++)
                        if (a >= 0 && a < X
                            && b >= 0 && b < Y
                            && c >= 0 && c < Z
                            && (a != x || b != y || c != z)) yield return new Position(a, b, c);
        }

        public Board(int size, int mines) : this(size, size, size, mines) { }
        public Board(BoardSettings settings) : this(settings.Width, settings.Height, settings.Depth, settings.Mines) { }
        public Board(int x, int y, int z, int mines)
        {
            X = x;
            Y = y;
            Z = z;

            Tokens = new Token[x, y, z];

            for (int a = 0; a < x; a++)
                for (int b = 0; b < y; b++)
                    for (int c = 0; c < z; c++)
                        Tokens[a, b, c] = new Marker();

            AddMines(mines);
        }

        private static Random R = new Random();

        public void AddMines(int count)
        {
            if (count + Mines > X * Y * Z) throw new ArgumentException("Cannot add this many mines");

            for (; count > 0; count--)
            {
                int x, y, z;
                RandomMarker(out x, out y, out z);

                Tokens[x, y, z] = new Mine();

                foreach (Token t in Adjacent(x, y, z))
                    if (t is Marker) ((Marker)t).Count++;
            }

            Update();
        }
        public void RemoveMines(int count)
        {
            if (count > Mines) throw new ArgumentException("Cannot remove this many mines");

            for (; count > 0; count--)
            {
                int x, y, z;
                RandomMine(out x, out y, out z);

                Tokens[x, y, z] = new Marker();

                foreach (Token t in Adjacent(x, y, z))
                    if (t is Marker) ((Marker)t).Count--;
            }

            Update();
        }

        public void Random(out int x, out int y, out int z)
        {
            x = R.Next(0, X);
            y = R.Next(0, Y);
            z = R.Next(0, Z);
        }
        public void RandomMarker(out int x, out int y, out int z)
        {
            do
            {
                x = R.Next(0, X);
                y = R.Next(0, Y);
                z = R.Next(0, Z);
            } while (Tokens[x, y, z] is Mine);
        }
        public void RandomMine(out int x, out int y, out int z)
        {
            do
            {
                x = R.Next(0, X);
                y = R.Next(0, Y);
                z = R.Next(0, Z);
            } while (Tokens[x, y, z] is Marker);
        }
    }
    //Drawing
    public partial class Board
    {
        private static readonly Font
            BigFont = new Font(FontFamily.GenericSansSerif, 25f, GraphicsUnit.Pixel),
            SmallFont = new Font(FontFamily.GenericSansSerif, 20f, GraphicsUnit.Pixel),
            GUIFont = new Font(FontFamily.GenericSansSerif, 30f, GraphicsUnit.Pixel);

        private static readonly SolidBrush
            FocusKnown = new SolidBrush(Color.FromArgb(255, 200, 200, 200)),
            FocusUnknown = new SolidBrush(Color.FromArgb(255, 51, 153, 255)),

            UnfocusKnown = new SolidBrush(Color.FromArgb(20, 200, 200, 200)),
            UnfocusUnknown = new SolidBrush(Color.FromArgb(20, 51, 153, 255)),

            FocusText = new SolidBrush(Color.FromArgb(255, 0, 0, 0)),
            UnfocusText = new SolidBrush(Color.FromArgb(40, 0, 0, 0)),

            FocusMarked = new SolidBrush(Color.FromArgb(255, 255, 128, 0)),
            UnfocusMarked = new SolidBrush(Color.FromArgb(20, 255, 128, 0)),

            FocusBomb = new SolidBrush(Color.FromArgb(255, 153, 0, 0)),
            UnfocusBomb = new SolidBrush(Color.FromArgb(20, 153, 0, 0)),
            
            GUIBrush = new SolidBrush(Color.FromArgb(255, 51, 153, 255));

        private const int Width = 25, Spacing = 5, Phase = 10, HoverPadding = 3, MarkPadding = 5, GUIOffset = 30, TimeOffset = 25;
        private const int SmallOffset = 4, LargeOffset = -3;
        private const int LayerShowRadius = 3;

        [NonSerialized] private Bitmap PrevRender = null;
        [NonSerialized] private int PrevFocus = -1;
        [NonSerialized] private bool PrevUptodate = false;
        public void Paint(PaintEventArgs e, int focusLayer, Position hover, Point start)
        {
            if (focusLayer < 0 || focusLayer > Z) throw new ArgumentException("Focus layer out of range");
            UpdateTime();

            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            if (!PrevUptodate || PrevFocus != focusLayer)
            {
                PrevFocus = focusLayer;
                PrevUptodate = true;

                if (PrevRender != null) PrevRender.Dispose();
                PrevRender = new Bitmap(
                    X * (Width + Spacing) + (Z - 1) * Phase + MarkPadding,
                    Y * (Width + Spacing) + (Z - 1) * Phase + MarkPadding);
                Graphics _g = Graphics.FromImage(PrevRender);
                _g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                for (int z = focusLayer - LayerShowRadius; z < focusLayer + LayerShowRadius + 1; z++)
                    if (z != focusLayer && z > -1 && z < Z)
                        for (int x = 0; x < X; x++)
                            for (int y = 0; y < Y; y++)
                                PaintToken(_g, x, y, z, start, false, false);
                for (int x = 0; x < X; x++)
                    for (int y = 0; y < Y; y++)
                        PaintToken(_g, x, y, focusLayer, start, true, false);

                _g.Dispose();
            }

            if (!Paused)
            {
                g.DrawImage(PrevRender, Point.Empty);

                if (Enabled
                    && hover.X > -1 && hover.X < X
                    && hover.Y > -1 && hover.Y < Y
                    && hover.Z > -1 && hover.Z < Z)
                    PaintToken(g, hover.X, hover.Y, hover.Z, start, true, true);

                g.DrawString(string.Format("{0}/{1} Mines Marked", Marked, Mines), GUIFont, GUIBrush, X * (Width + Spacing) + GUIOffset, 0);
            }
            else g.DrawString("Paused", GUIFont, GUIBrush, X * (Width + Spacing) + GUIOffset, 0);

            
            g.DrawString(Time.ToString(@"hh\:mm\:ss"), GUIFont, GUIBrush, X * (Width + Spacing) + GUIOffset + TimeOffset, TimeOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PaintToken(Graphics g, int x, int y, int z, Point start, bool focus, bool big)
        {
            SolidBrush textBrush = focus ? FocusText : UnfocusText;
            SolidBrush markBrush = focus ? FocusMarked : UnfocusMarked;

            Token t = Tokens[x, y, z];

            Rectangle r =
                big ?
                    new Rectangle(
                        start.X + (Width + Spacing) * x + Phase * z - HoverPadding,
                        start.Y + (Width + Spacing) * y + Phase * z - HoverPadding,
                        Width + 2 * HoverPadding, Width + 2 * HoverPadding) :
                    new Rectangle(
                        start.X + (Width + Spacing) * x + Phase * z,
                        start.Y + (Width + Spacing) * y + Phase * z,
                        Width, Width);

            SolidBrush b = focus ?
                (t.Unknown ? FocusUnknown : (t is Mine ? FocusBomb : FocusKnown)) :
                (t.Unknown ? UnfocusUnknown : (t is Mine ? UnfocusBomb : UnfocusKnown));

            g.FillRectangle(b, r);

            if (t.Marked)
            {
                Rectangle m = new Rectangle(
                    r.Left + MarkPadding, r.Top + MarkPadding,
                    r.Width - 2 * MarkPadding - 1, r.Height - 2 * MarkPadding - 1);

                g.FillEllipse(markBrush, m);
            }
            else if (!t.Unknown && t is Marker)
            {
                int c = ((Marker)t).Count;
                if (c > 0) g.DrawString(
                     c.ToString(),
                     SmallFont,
                     textBrush,
                     new Point(r.Location.X + (c < 10 ? SmallOffset : LargeOffset), r.Location.Y));
            }
        }

        public Position Hovering(Point p, Point start, int focusLayer)
        {
            if (focusLayer < 0 || focusLayer > Z) throw new ArgumentException("Focus layer out of range");

            int x = p.X - Phase * focusLayer - start.X;
            int y = p.Y - Phase * focusLayer - start.Y;

            int a = x / (Width + Spacing);
            int b = y / (Width + Spacing);

            return new Position(
                x % (Width + Spacing) < Width ? (x >= 0 && a < X ? a : -1) : -1,
                y % (Width + Spacing) < Width ? (y >= 0 && a < Y ? b : -1) : -1,
                focusLayer);
        }
    }
    //interaction
    public partial class Board
    {
        public void Reveal()
        {
            Enabled = false;

            foreach (Token t in Tokens)
                t.Unknown = false;

            Update();
        }

        public ActionResult Interact(Position pos, MouseButtons b)
        {
            if (!Enabled || Paused) return ActionResult.None;
            if (pos.X < 0 || pos.X >= X || pos.Y < 0 || pos.Y >= Y || pos.Z < 0 || pos.Z > Z) return ActionResult.None;
            Token t = this[pos];

            switch (b)
            {
                case MouseButtons.Left:
                    if (t is Mine) return ActionResult.GameOver;
                    if (!t.Unknown) return ActionResult.None;

                    t.Marked = false;
                    t.Unknown = false;

                    Update();

                    return Complete ? ActionResult.Victory : ActionResult.Success;
                case MouseButtons.Right:
                    if (!t.Unknown) return ActionResult.None;
                    t.Marked = !t.Marked;

                    Update();

                    return Complete ? ActionResult.Victory : ActionResult.Success;
                case MouseButtons.Middle:
                    if (t is Mine) return ActionResult.GameOver;

                    if (t.Unknown)
                    {
                        if (((Marker)t).Count == 0) FloodRevealRecursive(pos);
                        else t.Unknown = false;
                    }
                    else
                    {
                        if (Correct(pos)) FloodRevealRecursive(pos);
                        else return ActionResult.GameOver;
                    }

                    Update();

                    return Complete ? ActionResult.Victory : ActionResult.Success;
            }

            return ActionResult.None;
        }

        [Obsolete("Recursion has better performance")]
        public void FloodRevealLoop(Position pos)
        {
            byte[,,] map = new byte[X, Y, Z];
            map[pos.X, pos.Y, pos.Z] = 1;

            while (true)
            {
                bool d = true;

                for (int x = 0; x < X; x++)
                    for (int y = 0; y < Y; y++)
                        for (int z = 0; z < Z; z++)
                        {
                            if (map[x, y, z] != 1) continue;
                            map[x, y, z] = 2;
                            d = false;

                            Token t = Tokens[x, y, z];
                            t.Marked = false;
                            t.Unknown = false;

                            if (t is Marker && ((Marker)t).Count == 0)
                                for (int a = x - 1; a <= x + 1; a++)
                                    for (int b = y - 1; b <= y + 1; b++)
                                        for (int c = z - 1; c <= z + 1; c++)
                                            if (a > -1 && a < X
                                                && b > -1 && b < Y
                                                && c > -1 && c < Z
                                                && map[a, b, c] == 0) map[a, b, c] = 1;

                        }

                if (d) break;
            }
        }

        public void FloodRevealRecursive(Position pos, bool _first = true)
        {
            Token t = this[pos];
            t.Unknown = false;

            if (((Marker)t).Count == 0 || _first)
                foreach (Position p in AdjacentPos(pos))
                {
                    Token o = this[p];
                    if (o.Unknown && o is Marker) FloodRevealRecursive(p, false);
                }
        }

        private bool Correct(Position pos)
        {
            foreach(Token t in Adjacent(pos))
                if ((t is Mine && !t.Marked) || (t is Marker && t.Marked)) return false;

            return true;
        }

        public bool Complete
        {
            get
            {
                foreach (Token t in Tokens)
                    if (t.Unknown || (t is Mine && !t.Marked) || (t is Marker && t.Marked)) return false;

                return true;
            }
        }

        [NonSerialized]
        private Action _OnUpdate = null;
        public event Action OnUpdate
        {
            add
            {
                _OnUpdate += value;
            }
            remove
            {
                _OnUpdate -= value;
            }
        }
        private void Update()
        {
            PrevUptodate = false;
            if (_OnUpdate != null) _OnUpdate();
        }
    }
    //settings
    public partial class Board
    {
        public static readonly BoardSettings
            Easy = new BoardSettings(5, 5, 5, 5),
            Medium = new BoardSettings(10, 10, 10, 80),
            Hard = new BoardSettings(15, 15, 15, 300),
            Insane = new BoardSettings(20, 20, 20, 666);
    }
    //IO
    public partial class Board
    {
        private static BinaryFormatter Bin = new BinaryFormatter();
        public const string
            BinExt = ".3dm",
            Filter = "3D Minesweeper File (.3dm)|*.3dm";

        public void Save(FileInfo file)
        {
            UpdateTime();
            FileStream s = null;

            try
            {
                s = file.OpenWrite();
                Bin.Serialize(s, this);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                    s.Dispose();
                }
            }
        }

        public static Board Load(FileInfo file)
        {
            FileStream s = null;

            try
            {
                s = file.OpenRead();
                return Bin.Deserialize(s) as Board;
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                    s.Dispose();
                }
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext c)
        {
            Last = DateTime.Now;
        }
    }
    //Time
    public partial class Board
    {
        [NonSerialized] private bool _Paused = false;
        public bool Paused
        {
            get
            {
                return _Paused;
            }
            set
            {
                UpdateTime();
                _Paused = value;

                Update();
            }
        }

        private TimeSpan Time = TimeSpan.Zero;

        [NonSerialized] private DateTime Last = DateTime.Now;
        private void UpdateTime()
        {
            DateTime now = DateTime.Now;
            if(Enabled && !Paused) Time += now - Last;
            Last = now;
        }
    }
    #endregion

    public enum ActionResult
    {
        Success,
        GameOver,
        Victory,
        None
    }

    [Serializable]
    public class Token
    {
        public bool Marked = false, Unknown = true;
    }

    [Serializable]
    public sealed class Mine : Token { }
    [Serializable]
    public sealed class Marker : Token
    {
        public int Count = 0;
    }

    public struct Position
    {
        public int X, Y, Z;

        public Position(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static bool operator ==(Position a, Position b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }
        public static bool operator !=(Position a, Position b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static readonly Position Null = new Position(-1, -1, -1);
    }

    public struct BoardSettings
    {
        public int Width, Height, Depth, Mines;

        public BoardSettings(int width, int height, int depth, int mines)
        {
            Width = width;
            Height = height;
            Depth = depth;

            Mines = mines;
        }
    }
}
