using System;
using System.Drawing;
using System.Linq;
using OpenTK.Input;

namespace GLGUI
{
	public class GLForm : GLControl
	{
        public string Title { get { return title; } set { if (value != title) { title = value; Invalidate(); } } }
        public bool Maximized { get { return maximized; } set { Maximize(value); } }
        public GLSkin.GLFormSkin SkinActive { get { return skinActive; } set { skinActive = value; Invalidate(); } }
        public GLSkin.GLFormSkin SkinInactive { get { return skinInactive; } set { skinInactive = value; Invalidate(); } }

		private string title = "";
        private GLFontText titleProcessed = new GLFontText();
		private GLSkin.GLFormSkin skinActive, skinInactive;
		private GLSkin.GLFormSkin skin;
		private enum DragOperation { Move = 0, ResizeNW, ResizeN, ResizeNE, ResizeE, ResizeSE, ResizeS, ResizeSW, ResizeW }
		private DragOperation dragOp;
		private Rectangle moveClickRegion;
		private Point mouseOffset;
		private SizeF titleSize;
		private int minHeight;
		private DateTime lastClick;

		private static GLCursor[] dragOpCursors = new GLCursor[]
		{
			GLCursor.SizeAll,
			GLCursor.SizeNWSE, GLCursor.SizeNS, GLCursor.SizeNESW, GLCursor.SizeWE,
			GLCursor.SizeNWSE, GLCursor.SizeNS, GLCursor.SizeNESW, GLCursor.SizeWE
		};

		public GLForm(GLGui gui) : base(gui)
		{
			Render += OnRender;
			MouseDown += OnMouseDown;
			MouseUp += OnMouseUp;
			MouseMove += OnMouseMove;
			MouseLeave += OnMouseLeave;
			//MouseDoubleClick += OnMouseDoubleClick;
			Focus += (s, e) => Invalidate();
            FocusLost += (s, e) => Invalidate();

			skinActive = Gui.Skin.FormActive;
			skinInactive = Gui.Skin.FormInactive;

			outer = new Rectangle(0, 0, 100, 100);
			sizeMin = new Size(64, 32);
			sizeMax = new Size(int.MaxValue, int.MaxValue);
		}

        protected override void UpdateLayout()
		{
			skin = HasFocus ? skinActive : skinInactive;

			if (AutoSize)
			{
				if (Controls.Count() > 0)
				{
					outer.Width = Controls.Max(c => c.Outer.Right) + skin.Border.Horizontal;
                    outer.Height = Controls.Max(c => c.Outer.Bottom) + skin.Border.Vertical + (int)titleSize.Height + skin.Border.Top;
				}
				else
				{
					outer.Width = 0;
					outer.Height = 0;
				}
			}

			outer.Width = Math.Min(Math.Max(outer.Width, sizeMin.Width), sizeMax.Width);

			int innerWidth = outer.Width - skin.Border.Horizontal;
            titleSize = skin.Font.ProcessText(titleProcessed, title, GLFontAlignment.Left);
            minHeight = Math.Max(sizeMin.Height, (int)titleSize.Height + skin.Border.Vertical + skin.Border.Top);

			outer.Height = Math.Min(Math.Max(outer.Height, minHeight), sizeMax.Height);
			if (Parent != null)
			{
				outer.X = Math.Max(Math.Min(outer.X, Parent.Inner.Width - outer.Width), 0);
				outer.Y = Math.Max(Math.Min(outer.Y, Parent.Inner.Height - outer.Height), 0);
                outer.Width = Math.Min(outer.Right, Parent.Inner.Width) - outer.X;
                outer.Height = Math.Min(outer.Bottom, Parent.Inner.Height) - outer.Y;
			}

			Inner = new Rectangle(
                skin.Border.Left, skin.Border.Top + (int)titleSize.Height + skin.Border.Top,
                innerWidth, outer.Height - skin.Border.Vertical - (int)titleSize.Height - skin.Border.Top);
            moveClickRegion = new Rectangle(skin.Border.Left, skin.Border.Top, Inner.Width, (int)titleSize.Height);
		}

        private void OnRender(object sender, double timeDelta)
		{
			GLDraw.Fill(ref skin.BorderColor);
			GLDraw.FillRect(Inner, ref skin.BackgroundColor);
            GLDraw.Text(titleProcessed, ref moveClickRegion, ref skin.Color);
		}

		private void StartDragOperation(DragOperation op, Point p)
		{
            if ((AutoSize || maximized) && op != DragOperation.Move)
				return;

			mouseOffset = p;
			isDragged = true;
			dragOp = op;
			Gui.Cursor = dragOpCursors[(int)op];
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.Button == MouseButton.Left)
			{
				if (moveClickRegion.Contains(e.Position))
					StartDragOperation (DragOperation.Move, e.Position);
				else if(e.X < skin.Border.Left)
				{
					if (e.Y < skin.Border.Top)
						StartDragOperation (DragOperation.ResizeNW, e.Position);
					else if (e.Y >= outer.Height - skin.Border.Bottom)
						StartDragOperation (DragOperation.ResizeSW, e.Position);
					else
						StartDragOperation (DragOperation.ResizeW, e.Position);
				}
				else if (e.X >= outer.Width - skin.Border.Right)
				{
					if (e.Y < skin.Border.Top)
						StartDragOperation (DragOperation.ResizeNE, e.Position);
					else if (e.Y >= outer.Height - skin.Border.Bottom)
						StartDragOperation (DragOperation.ResizeSE, e.Position);
					else
						StartDragOperation (DragOperation.ResizeE, e.Position);
				}
				else if (e.Y < skin.Border.Top)
					StartDragOperation (DragOperation.ResizeN, e.Position);
				else if (e.Y >= outer.Height - skin.Border.Bottom)
					StartDragOperation (DragOperation.ResizeS, e.Position);
			}

            justDoubleClicked = false;
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.Button == MouseButton.Left)
			{
				if (isDragged)
				{
					isDragged = false;
					Gui.Cursor = GLCursor.Default;
				}
				var now = DateTime.Now;
				if ((now - lastClick).TotalMilliseconds < 500.0)
					OnMouseDoubleClick(this, e);
				lastClick = now;
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (isDragged)
			{
				Point p = e.Position;
				if (Parent != null)
				{
					p.X = Math.Min(Math.Max(p.X + outer.X, 0), Parent.Inner.Width) - outer.X;
					p.Y = Math.Min(Math.Max(p.Y + outer.Y, 0), Parent.Inner.Height) - outer.Y;
				}
				int dx = p.X - mouseOffset.X;
				int dy = p.Y - mouseOffset.Y;
				switch (dragOp)
				{
					case DragOperation.Move:
                        if (maximized && !justDoubleClicked)
                        {
                            if (Math.Max(dx, dy) > 16)
                            {
                                mouseOffset.X = restoreOuter.Width / 2;
                                Maximize(false);
                            }
                        }
                        else
						    Outer = new Rectangle(outer.X + dx, outer.Y + dy, outer.Width, outer.Height);
						return;
					case DragOperation.ResizeNW:
						dx = outer.Width - Math.Min(Math.Max(outer.Width - dx, sizeMin.Width), sizeMax.Width);
						dy = outer.Height - Math.Min(Math.Max(outer.Height - dy, minHeight), sizeMax.Height);
						Outer = new Rectangle(outer.X + dx, outer.Y + dy, outer.Width - dx, outer.Height - dy);
						return;
					case DragOperation.ResizeN:
						dy = outer.Height - Math.Min(Math.Max(outer.Height - dy, minHeight), sizeMax.Height);
						Outer = new Rectangle(outer.X, outer.Y + dy, outer.Width, outer.Height - dy);
						return;
					case DragOperation.ResizeNE:
						dx = Math.Min(Math.Max(p.X, sizeMin.Width), sizeMax.Width);
						dy = outer.Height - Math.Min(Math.Max(outer.Height - dy, minHeight), sizeMax.Height);
						Outer = new Rectangle(outer.X, outer.Y + dy, dx, outer.Height - dy);
						return;
					case DragOperation.ResizeE:
						dx = Math.Min(Math.Max(p.X, sizeMin.Width), sizeMax.Width);
						Outer = new Rectangle(outer.X, outer.Y, dx, outer.Height);
						return;
					case DragOperation.ResizeSE:
						dx = Math.Min(Math.Max(p.X, sizeMin.Width), sizeMax.Width);
						dy = Math.Min(Math.Max(p.Y, minHeight), sizeMax.Height);
						Outer = new Rectangle(outer.X, outer.Y, dx, dy);
						return;
					case DragOperation.ResizeS:
						dy = Math.Min(Math.Max(p.Y, minHeight), sizeMax.Height);
						Outer = new Rectangle(outer.X, outer.Y, outer.Width, dy);
						return;
					case DragOperation.ResizeSW:
						dx = outer.Width - Math.Min(Math.Max(outer.Width - dx, sizeMin.Width), sizeMax.Width);
						dy = Math.Min(Math.Max(p.Y, minHeight), sizeMax.Height);
						Outer = new Rectangle(outer.X + dx, outer.Y, outer.Width - dx, dy);
						return;
					case DragOperation.ResizeW:
						dx = outer.Width - Math.Min(Math.Max(outer.Width - dx, sizeMin.Width), sizeMax.Width);
						Outer = new Rectangle(outer.X + dx, outer.Y, outer.Width - dx, outer.Height);
						return;
				}
			}

			if (!AutoSize && !maximized)
			{
				if (e.X < skin.Border.Left)
				{
					if (e.Y < skin.Border.Top)
						Gui.Cursor = GLCursor.SizeNWSE;
					else if (e.Y >= outer.Height - skin.Border.Bottom)
						Gui.Cursor = GLCursor.SizeNESW;
					else
						Gui.Cursor = GLCursor.SizeWE;
				}
				else if (e.X >= Outer.Width - skin.Border.Right)
				{
					if (e.Y < skin.Border.Top)
						Gui.Cursor = GLCursor.SizeNESW;
					else if (e.Y >= outer.Height - skin.Border.Bottom)
						Gui.Cursor = GLCursor.SizeNWSE;
					else
						Gui.Cursor = GLCursor.SizeWE;
				}
				else if (e.Y < skin.Border.Top || e.Y >= outer.Height - skin.Border.Bottom)
					Gui.Cursor = GLCursor.SizeNS;
				else
					Gui.Cursor = GLCursor.Default;
			}
		}

		private void OnMouseLeave(object sender, EventArgs e)
		{
			Gui.Cursor = GLCursor.Default;
		}

        private bool justDoubleClicked = false;
        private bool maximized = false;
        private Rectangle restoreOuter;
        private GLAnchorStyles restoreAnchor;
        private void Maximize(bool maximize)
        {
            if (maximized == maximize)
                return;
            if (!maximize) // restore
            {
                maximized = false;
                anchor = restoreAnchor;
                outer = restoreOuter;
                Invalidate();
            }
            else // maximize
            {
                maximized = true;
                restoreOuter = outer;
                restoreAnchor = anchor;
                outer = Parent.Inner;
                anchor = GLAnchorStyles.Left | GLAnchorStyles.Top | GLAnchorStyles.Right | GLAnchorStyles.Bottom;
                Invalidate();
            }
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
			if (!AutoSize && Parent != null && moveClickRegion.Contains(e.Position))
                Maximize(!maximized);

			Gui.Cursor = GLCursor.Default; // hack to avoid move operation cursor
            justDoubleClicked = true;
        }
	}
}

