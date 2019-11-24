using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.CompilerServices;

namespace _3D_Minesweeper
{
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
			return obj is Position && (Position)obj == this;
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

	public enum ActionResult
	{
		None,
		Success,
		GameOver,
		Victory,
	}

	[Serializable]
	public class Tile
	{
		public bool Unknown;
		public bool IsMine;
		public bool IsMarked;
		public int Count;
	}

	//Core
	[Serializable]
	public partial class Board
	{
		private static Random R = new Random();

		private Tile[,,] Tokens;
		public readonly int X, Y, Z;
		private ActionResult State;
		private int Mines, Marked;

		private Tile this[Position pos]
		{
			get => Tokens[pos.X, pos.Y, pos.Z];
			set => Tokens[pos.X, pos.Y, pos.Z] = value;
		}

		private IEnumerable<Tile> Adjacent(Position pos)
		{
			return Adjacent(pos.X, pos.Y, pos.Z);
		}
		private IEnumerable<Tile> Adjacent(int x, int y, int z)
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
			if (x <= 0 || y <= 0 || z <= 0) throw new ArgumentException("Invalid dimensions");
			if (mines < 0 || mines > x * y * z) throw new ArgumentException("Invalid number of mines");

			Tokens = new Tile[x, y, z];

			X = x;
			Y = y;
			Z = z;

			State = ActionResult.None;

			Mines = mines;
			Marked = 0;

			for (int i = 0; i < X; ++i)
				for (int j = 0; j < Y; ++j)
					for (int k = 0; k < Z; ++k)
						Tokens[i, j, k] = new Tile() { Unknown = true, IsMine = false, IsMarked = false, Count = 0 };

			for (; mines > 0; mines--)
			{
				do
				{
					x = R.Next(0, X);
					y = R.Next(0, Y);
					z = R.Next(0, Z);
				} while (Tokens[x, y, z].IsMine);

				Tile t = Tokens[x, y, z];
				t.IsMine = true;
				foreach (Tile adj in Adjacent(x, y, z))adj.Count++;
			}

			Update();
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
			if (focusLayer < 0 || focusLayer >= Z) throw new ArgumentException("Focus layer out of range");
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

				using (Graphics _g = Graphics.FromImage(PrevRender))
				{
					_g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

					for (int z = focusLayer - LayerShowRadius; z < focusLayer + LayerShowRadius + 1; z++)
						if (z != focusLayer && z > -1 && z < Z)
							for (int x = 0; x < X; x++)
								for (int y = 0; y < Y; y++)
									PaintToken(_g, x, y, z, start, false, false);
					for (int x = 0; x < X; x++)
						for (int y = 0; y < Y; y++)
							PaintToken(_g, x, y, focusLayer, start, true, false);
				}
			}

			if (!Paused)
			{
				g.DrawImage(PrevRender, Point.Empty);

				if (State == ActionResult.None
					&& hover.X > -1 && hover.X < X
					&& hover.Y > -1 && hover.Y < Y
					&& hover.Z > -1 && hover.Z < Z)
					PaintToken(g, hover.X, hover.Y, hover.Z, start, true, true);

				g.DrawString($"{Marked}/{Mines} Mines Marked", GUIFont, GUIBrush, X * (Width + Spacing) + GUIOffset, 0);
			}
			else g.DrawString("Paused", GUIFont, GUIBrush, X * (Width + Spacing) + GUIOffset, 0);

			g.DrawString(Time.ToString(@"hh\:mm\:ss"), GUIFont, GUIBrush, X * (Width + Spacing) + GUIOffset + TimeOffset, TimeOffset);
		}

		private void PaintToken(Graphics g, int x, int y, int z, Point start, bool focus, bool big)
		{
			SolidBrush textBrush = focus ? FocusText : UnfocusText;
			SolidBrush markBrush = focus ? FocusMarked : UnfocusMarked;

			Tile t = Tokens[x, y, z];

			Rectangle r = big ?
				new Rectangle(
					start.X + (Width + Spacing) * x + Phase * z - HoverPadding,
					start.Y + (Width + Spacing) * y + Phase * z - HoverPadding,
					Width + 2 * HoverPadding, Width + 2 * HoverPadding) :
				new Rectangle(
					start.X + (Width + Spacing) * x + Phase * z,
					start.Y + (Width + Spacing) * y + Phase * z,
					Width, Width);

			SolidBrush b = focus ?
				(t.Unknown ? FocusUnknown : (t.IsMine ? FocusBomb : FocusKnown)) :
				(t.Unknown ? UnfocusUnknown : (t.IsMine ? UnfocusBomb : UnfocusKnown));

			g.FillRectangle(b, r);

			if (t.IsMarked)
			{
				Rectangle m = new Rectangle(
					r.Left + MarkPadding, r.Top + MarkPadding,
					r.Width - 2 * MarkPadding - 1, r.Height - 2 * MarkPadding - 1);

				g.FillEllipse(markBrush, m);
			}
			else if (!t.Unknown && !t.IsMine)
			{
				int c = t.Count;
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
		private void GameOver()
		{
			foreach (Tile t in Tokens) t.Unknown = false;
			State = ActionResult.GameOver;
			Update();
		}
		private bool CheckCompleted()
		{
			foreach (Tile t in Tokens) if (t.IsMine && !t.IsMarked || !t.IsMine && (t.IsMarked || t.Unknown)) return false;
			State = ActionResult.Victory;
			Update();
			return true;
		}
		private bool Correct(Position pos)
		{
			foreach (Tile adj in Adjacent(pos)) if (adj.IsMine ^ adj.IsMarked) return false;
			return true;
		}

		public ActionResult Interact(Position pos, MouseButtons b)
		{
			if (State != ActionResult.None || Paused) return ActionResult.None;
			if (pos.X < 0 || pos.X >= X || pos.Y < 0 || pos.Y >= Y || pos.Z < 0 || pos.Z > Z) return ActionResult.None;
			Tile t = this[pos];

			switch (b)
			{
			case MouseButtons.Left:
				if (t.IsMine)
				{
					GameOver();
					return ActionResult.GameOver;
				}
				if (!t.Unknown) return ActionResult.None;

				t.Unknown = false;
				if (t.IsMarked)
				{
					t.IsMarked = false;
					Marked--;
				}

				break;

			case MouseButtons.Right:
				if (!t.Unknown) return ActionResult.None;

				if (t.IsMarked)
				{
					t.IsMarked = false;
					Marked--;
				}
				else
				{
					t.IsMarked = true;
					Marked++;
				}

				break;

			case MouseButtons.Middle:
				if (t.IsMine)
				{
					GameOver();
					return ActionResult.GameOver;
				}

				if (t.Unknown)
				{
					if (t.Count == 0) FloodRevealRecursive(pos);
					else t.Unknown = false;
				}
				else
				{
					int adjmarks = 0;
					foreach (Tile adj in Adjacent(pos)) if (adj.IsMarked) ++adjmarks;
					if (adjmarks != t.Count) return ActionResult.None;

					if (Correct(pos)) FloodRevealRecursive(pos);
					else
					{
						GameOver();
						return ActionResult.GameOver;
					}
				}

				break;

			default: return ActionResult.None;
			}

			if (CheckCompleted()) return ActionResult.Victory;

			Update();
			return ActionResult.Success;
		}

		private void FloodRevealRecursive(Position pos, bool _first = true)
		{
			Tile t = this[pos];
			t.Unknown = false;

			if (t.Count == 0 || _first)
				foreach (Position p in AdjacentPos(pos))
				{
					Tile adj = this[p];
					if (adj.Unknown && !adj.IsMine) FloodRevealRecursive(p, false);
				}
		}

		[NonSerialized] private Action _OnUpdate = null;
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
			_OnUpdate?.Invoke();
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
			if (State == ActionResult.None && !Paused) Time += now - Last;
			Last = now;
		}
	}
}
