using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace GLGUI
{
	public abstract class GLControl
	{
		public virtual string Name { get; set; }
		public GLGui Gui { get; internal set; }
		public GLControl Parent { get; internal set; }
		public IEnumerable<GLControl> Controls { get { return controls; } }

        public Rectangle Outer { get { return outer; } set { if (outer != value) { outer = value; Invalidate(); } } }
        public Rectangle Inner { get { return inner; } protected set { if (inner != value) { lastInner = inner; inner = value; DoResize(); /*Invalidate();*/ } } }
		public Size SizeMin { get { return sizeMin; } set { sizeMin = value; Invalidate(); } }
        public Size SizeMax { get { return sizeMax; } set { sizeMax = value; Invalidate(); } }
		public bool HasFocus { get; private set; }
		public bool IsDragged { get { return isDragged || controls.Any(c => c.IsDragged); } }
        public AnchorStyles Anchor { get { return anchor; } set { anchor = value; Invalidate(); } }
        public bool AutoSize { get { return autoSize; } set { autoSize = value; Invalidate(); } }
		public GLContextMenu ContextMenu { get { return contextMenu; } set { contextMenu = value; } }

        // derived from above properties:
        public Point Location { get { return outer.Location; } set { Outer = new Rectangle(value, outer.Size); } }
        public Size Size { get { return outer.Size; } set { Outer = new Rectangle(outer.Location, value); } }
        public int X { get { return outer.X; } }
        public int Y { get { return outer.Y; } }
        public int Width { get { return outer.Width; } }
        public int Height { get { return outer.Height; } }
        public Point InnerOffset { get { return inner.Location; } }
        public Size InnerSize { get { return inner.Size; } }
        public int InnerWidth { get { return inner.Width; } }
        public int InnerHeight { get { return inner.Height; } }

        public delegate void RenderEventHandler(Rectangle scissorRect, double timeDelta);

		public event RenderEventHandler Render;
		public event MouseEventHandler MouseMove;
		public event MouseEventHandler MouseDown;
		public event MouseEventHandler MouseUp;
		public event MouseEventHandler MouseClick;
		public event MouseEventHandler MouseDoubleClick;
		public event MouseEventHandler MouseWheel;
		public event EventHandler MouseEnter;
		public event EventHandler MouseLeave;
		public event KeyEventHandler KeyDown;
		public event KeyEventHandler KeyUp;
		public event KeyPressEventHandler KeyPress;
		public event EventHandler Focus;
		public event EventHandler FocusLost;
		public event EventHandler Resize;

		protected Rectangle outer;
		protected Size sizeMin, sizeMax;
		protected bool isDragged = false;
		protected AnchorStyles anchor = AnchorStyles.Left | AnchorStyles.Top;

		private Rectangle inner;
		private Rectangle lastInner;
		private bool autoSize = false;
		private List<GLControl> controls = new List<GLControl>();
		private static int idCounter = 0;
        private bool visited = false;
		private GLContextMenu contextMenu;

		private GLControl hoverChild;
		private GLControl focusedChild;

		public GLControl(GLGui gui)
		{
			Gui = gui;
			Name = GetType().Name + (idCounter++);

			outer = new Rectangle(0, 0, 0, 0);
			inner = new Rectangle(0, 0, 0, 0);
			sizeMin = new Size(0, 0);
			sizeMax = new Size(int.MaxValue, int.MaxValue);
		}

        public void Invalidate()
        {
			if (visited || Gui == null || Gui.LayoutSuspended)
                return;
            visited = true;

            UpdateLayout();

            if (Parent != null && Parent.autoSize)
                Parent.Invalidate();

            visited = false;
        }

		protected virtual void UpdateLayout()
		{
			if (autoSize)
			{
				if (Controls.Count() > 0)
				{
					outer.Width = Controls.Max(c => c.Outer.Right);
					outer.Height = Controls.Max(c => c.Outer.Bottom);
				}
				else
				{
					outer.Width = 0;
					outer.Height = 0;
				}
			}
			outer.Width = Math.Min(Math.Max(outer.Width, sizeMin.Width), sizeMax.Width);
			outer.Height = Math.Min(Math.Max(outer.Height, sizeMin.Height), sizeMax.Height);
			Inner = new Rectangle(0, 0, outer.Width, outer.Height);
		}

		public virtual T Add<T>(T control) where T : GLControl
		{
			if (control.Parent != null)
			{
				string message = string.Format("{0} {1} is already a child of {2} {3}.",
					control.GetType().Name, control.Name,
					control.Parent.GetType().Name, control.Parent.Name);
				throw new ArgumentException(message);
			}
			if (control is GLForm || control is GLContextMenu)
                controls.Insert(0, control);
            else
			    controls.Add(control);
			control.Gui = Gui;
			control.Parent = this;
            control.Invalidate();
			return control;
		}

		public virtual void Remove(GLControl control)
		{
			if (control.Parent == null)
				return;

			if (control.Parent != this)
			{
				string message = string.Format("{0} {1} is a child of {2} {3}.",
					control.GetType().Name, control.Name,
					control.Parent.GetType().Name, control.Parent.Name);
				throw new ArgumentException(message);
			}

			controls.Remove(control);
			control.Gui = null;
			control.Parent = null;
		}

		public virtual void Clear()
		{
			Gui.SuspendLayout();
			foreach(GLControl control in controls.ToArray())
				Remove(control);
			Gui.ResumeLayout();
		}

		protected Point ToControl(Point p)
		{
			p.X -= outer.X;
			p.Y -= outer.Y;
			GLControl c = Parent;
			while (c != c.Parent)
			{
				p.X -= c.inner.X + c.outer.X;
				p.Y -= c.inner.Y + c.outer.Y;
				c = c.Parent;
			}
			return p;
		}

		protected Point ToViewport(Point p)
		{
			p.X += outer.X;
			p.Y += outer.Y;
			GLControl c = Parent;
			while (c != c.Parent)
			{
				p.X += c.inner.X + c.outer.X;
				p.Y += c.inner.Y + c.outer.Y;
				c = c.Parent;
			}
			return p;
		}

		protected Rectangle ToViewport(Rectangle r)
		{
			r.X += outer.X;
			r.Y += outer.Y;
			GLControl c = Parent;
			while (c != c.Parent)
			{
				r.X += c.inner.X + c.outer.X;
				r.Y += c.inner.Y + c.outer.Y;
				c = c.Parent;
			}
			return r;
		}

		public void Scissor(Rectangle scissorRect, Rectangle renderRect)
		{
			scissorRect.Intersect(ToViewport(renderRect));
			GL.Scissor(scissorRect.X, Gui.Outer.Height - scissorRect.Bottom, scissorRect.Width, scissorRect.Height);
		}

		internal void DoRender(Point absolutePosition, Rectangle scissorRect, double timeDelta)
		{
			bool pushed = false;
			if (outer.X != 0 || outer.Y != 0)
            {
                pushed = true;
                GL.PushMatrix();
                GL.Translate(outer.X, outer.Y, 0.0f);
                absolutePosition.X += outer.X;
                absolutePosition.Y += outer.Y;
            }

			if (Render != null)
			{
                scissorRect.Intersect(new Rectangle(absolutePosition, outer.Size));
				GL.Scissor(scissorRect.X, Gui.Outer.Height - scissorRect.Bottom, scissorRect.Width, scissorRect.Height);
                Render(scissorRect, timeDelta);
			}

			if (inner.X != 0 || inner.Y != 0)
            {
				if (!pushed)
				{
					pushed = true;
					GL.PushMatrix();
				}
                GL.Translate(inner.X, inner.Y, 0.0f);
                absolutePosition.X += inner.X;
                absolutePosition.Y += inner.Y;
				scissorRect.Intersect(new Rectangle(absolutePosition, inner.Size));
            }
			else if (Render == null || inner.Size.Width != outer.Size.Width || inner.Size.Height != outer.Size.Height)
			    scissorRect.Intersect(new Rectangle(absolutePosition, inner.Size));

			for (int i = controls.Count - 1; i >= 0; i--)
                controls[i].DoRender(absolutePosition, scissorRect, timeDelta);

            if(pushed)
			    GL.PopMatrix();
		}

		internal void DoMouseMove(MouseEventArgs e)
		{
			if (Parent == null)
				return;

			if (!isDragged)
			{
				Point im = new Point(e.X - inner.X, e.Y - inner.Y);

				if (hoverChild != null && hoverChild.IsDragged)
				{
					hoverChild.DoMouseMove(new MouseEventArgs(e.Button, e.Clicks, im.X - hoverChild.Outer.X, im.Y - hoverChild.Outer.Y, e.Delta));
					return;
				}

				if(inner.Contains(e.Location))
				{
					foreach (GLControl control in controls)
					{
						if (control.Outer.Contains(im))
						{
							if (hoverChild != control)
							{
								if (hoverChild != null)
									hoverChild.DoMouseLeave();
								hoverChild = control;
								hoverChild.DoMouseEnter();
							}
							control.DoMouseMove(new MouseEventArgs(e.Button, e.Clicks, im.X - control.Outer.X, im.Y - control.Outer.Y, e.Delta));
							return;
						}
					}
				}

				if (hoverChild != null)
				{
					hoverChild.DoMouseLeave();
					hoverChild = null;
				}
			}

			if (MouseMove != null)
				MouseMove(this, e);
		}

		internal void DoMouseDown(MouseEventArgs e)
		{
			if (Parent == null)
				return;

			if (!isDragged)
			{
				Point im = new Point(e.X - inner.X, e.Y - inner.Y);

				if (!HasFocus)
				{
					HasFocus = true;
					if (Focus != null)
						Focus(this, EventArgs.Empty);
				}

				if (inner.Contains(e.Location))
				{
					int i = 0;
					foreach(GLControl control in controls)
					{
						if (control.Outer.Contains(im))
						{
							if (control != focusedChild)
							{
								if (focusedChild != null)
									focusedChild.DoFocusLost();
								focusedChild = control;
							}
							control.DoMouseDown(new MouseEventArgs(e.Button, e.Clicks, im.X - control.Outer.X, im.Y - control.Outer.Y, e.Delta));
							if (control is GLForm)
							{
								GLControl tmp = controls[i]; // move to front
								controls.RemoveAt(i);
								controls.Insert(0, tmp);
							}
							return;
						}
						i++;
					}
				}
			}

			if (MouseDown != null)
				MouseDown(this, e);

			if (e.Button == MouseButtons.Right && contextMenu != null)
				Gui.OpenContextMenu(contextMenu, ToViewport(e.Location));
		}

		internal void DoMouseUp(MouseEventArgs e)
		{
			if (Parent == null)
				return;

			if (!isDragged)
			{
				Point im = new Point(e.X - inner.X, e.Y - inner.Y);

				if (hoverChild != null && hoverChild.IsDragged)
				{
					hoverChild.DoMouseUp(new MouseEventArgs(e.Button, e.Clicks, im.X - hoverChild.Outer.X, im.Y - hoverChild.Outer.Y, e.Delta));
					return;
				}

				if (inner.Contains(e.Location))
				{
					foreach(GLControl control in controls)
					{
						if (control.Outer.Contains(im))
						{
							control.DoMouseUp(new MouseEventArgs(e.Button, e.Clicks, im.X - control.Outer.X, im.Y - control.Outer.Y, e.Delta));
							return;
						}
					}
				}
			}

			if (MouseUp != null)
				MouseUp(this, e);
		}

		internal void DoMouseClick(MouseEventArgs e)
		{
			if (Parent == null)
				return;

			if (!isDragged)
			{
				Point im = new Point(e.X - inner.X, e.Y - inner.Y);

				if (inner.Contains(e.Location))
				{
					foreach(GLControl control in controls)
					{
						if (control.Outer.Contains(im))
						{
							control.DoMouseClick(new MouseEventArgs(e.Button, e.Clicks, im.X - control.Outer.X, im.Y - control.Outer.Y, e.Delta));
							return;
						}
					}
				}
			}

			if (MouseClick != null)
				MouseClick(this, e);
		}

		internal void DoMouseDoubleClick(MouseEventArgs e)
		{
			if (Parent == null)
				return;

			if (!isDragged)
			{
				Point im = new Point(e.X - inner.X, e.Y - inner.Y);

				if (inner.Contains(e.Location))
				{
					foreach(GLControl control in controls)
					{
						if (control.Outer.Contains(im))
						{
							control.DoMouseDoubleClick(new MouseEventArgs(e.Button, e.Clicks, im.X - control.Outer.X, im.Y - control.Outer.Y, e.Delta));
							return;
						}
					}
				}
			}

			if (MouseDoubleClick != null)
				MouseDoubleClick(this, e);
		}

		internal bool DoMouseWheel(MouseEventArgs e)
		{
			if (Parent == null)
				return false;

			if (!isDragged)
			{
				Point im = new Point(e.X - inner.X, e.Y - inner.Y);

				if (inner.Contains(e.Location))
				{
					foreach(GLControl control in controls)
					{
						if (control.Outer.Contains(im))
						{
							if (control.DoMouseWheel(new MouseEventArgs(e.Button, e.Clicks, im.X - control.Outer.X, im.Y - control.Outer.Y, e.Delta)))
								return true;
						}
					}
				}
			}

            if (MouseWheel != null)
            {
                MouseWheel(this, e);
                return true;
            }
            return false;
		}

		internal void DoMouseEnter()
		{
			if (Parent == null)
				return;

			Gui.Parent.Cursor = Cursors.Default;

			if (MouseEnter != null)
				MouseEnter(this, EventArgs.Empty);
		}

		internal void DoMouseLeave()
		{
			if (Parent == null)
				return;

			if (hoverChild != null)
			{
				hoverChild.DoMouseLeave();
				hoverChild = null;
			}

			if (MouseLeave != null)
				MouseLeave(this, EventArgs.Empty);
		}

		internal void DoKeyUp(KeyEventArgs e)
		{
			if (Parent == null)
				return;

			foreach (GLControl control in controls)
			{
				if (control.HasFocus)
				{
					control.DoKeyUp(e);
					if(e.Handled)
						return;
				}
			}

			if (KeyUp != null)
				KeyUp(this, e);
		}

		internal void DoKeyDown(KeyEventArgs e)
		{
			if (Parent == null)
				return;

			foreach (GLControl control in controls)
			{
				if (control.HasFocus)
				{
					control.DoKeyDown(e);
					if(e.Handled)
						return;
				}
			}

			if (KeyDown != null)
				KeyDown(this, e);
		}

		internal void DoKeyPress(KeyPressEventArgs e)
		{
			if (Parent == null)
				return;

			foreach (GLControl control in controls)
			{
				if (control.HasFocus)
				{
					control.DoKeyPress(e);
					if(e.Handled)
						return;
				}
			}

			if (KeyPress != null)
				KeyPress(this, e);
		}

		internal void DoFocusLost()
		{
			if (Parent == null)
				return;

			if (HasFocus)
			{
				HasFocus = false;
				if (FocusLost != null)
					FocusLost(this, EventArgs.Empty);
			}
			foreach (GLControl control in controls)
				if (control.HasFocus)
					control.DoFocusLost();
		}

		private int subPixelDx = 0, subPixelDy = 0;
		internal void DoResize()
		{
			if (Parent == null)
				return;

			if (Resize != null)
				Resize(this, EventArgs.Empty);

			int dx = inner.Width - lastInner.Width;
			int dy = inner.Height - lastInner.Height;

			foreach (GLControl control in controls)
			{
				var a = control.Anchor;
				var o = control.Outer;
				int l = o.Left, r = o.Right, t = o.Top, b = o.Bottom;

				if ((a & (AnchorStyles.Left | AnchorStyles.Right)) == 0)
				{
					dx += control.subPixelDx; control.subPixelDx = Math.Sign(dx) * (dx & 1);
					l += dx / 2;
					r += dx / 2;
				}
				else
				{
					if ((a & AnchorStyles.Left) == 0)
						l += dx;
					if ((a & AnchorStyles.Right) != 0)
						r += dx;
				}

				if ((a & (AnchorStyles.Top | AnchorStyles.Bottom)) == 0)
				{
					dy += control.subPixelDy; control.subPixelDy = Math.Sign(dy) * (dy & 1);
					t += dy / 2;
					b += dy / 2;
				}
				else
				{
					if ((a & AnchorStyles.Top) == 0)
						t += dy;
					if ((a & AnchorStyles.Bottom) != 0)
						b += dy;
				}

				control.outer = new Rectangle(l, t, r - l, b - t);
                control.Invalidate();
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}
}

