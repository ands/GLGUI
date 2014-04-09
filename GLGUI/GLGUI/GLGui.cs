using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GLGUI
{
	public class GLGui : GLControl
	{
		public new GameWindow Parent;
		public GLSkin Skin = new GLSkin();
		public double RenderDuration { get { return renderDuration; } }
		public bool LayoutSuspended { get { return suspendCounter > 0; } }
		public GLCursor Cursor { get { return cursor; } set { cursor = value; Parent.CursorHandle = cursor.Handle; } }

		internal static List<IDisposable> toDispose = new List<IDisposable>();
		internal static int usedTextures = 0;
		internal static int usedVertexArrays = 0;
		private static int lastUsedTextures = 0;
		private static int lastUsedVertexArrays = 0;

		private GLContextMenu currentContextMenu;
        private Stopwatch stopwatch;
		private double renderDuration;
		private int suspendCounter = 0;
		private GLCursor cursor;

		public GLGui(GameWindow parent) : base(null)
		{
			GLCursor.LoadCursors(parent);

			Gui = this;
			base.Parent = this;
			Parent = parent;
			Outer = parent.ClientRectangle;
			Anchor = GLAnchorStyles.All;

			parent.Mouse.Move += (s, e) => DoMouseMove(e);
			parent.Mouse.ButtonDown += OnMouseDown;
			parent.Mouse.ButtonUp += OnMouseUp;
			parent.Mouse.WheelChanged += (s, e) => DoMouseWheel(e);
			parent.MouseEnter += (s, e) => DoMouseEnter();
			parent.MouseLeave += (s, e) => DoMouseLeave();
			parent.KeyDown += (s, e) => DoKeyDown(e);
			parent.KeyUp += (s, e) => DoKeyUp(e);
			parent.KeyPress += (s, e) => DoKeyPress(e);
			parent.Resize += (s, e) => Outer = parent.ClientRectangle;
		}
		
		public void SuspendLayout()
		{
			suspendCounter++;
		}

		public void ResumeLayout()
		{
			suspendCounter--;
			if (suspendCounter < 0)
				suspendCounter = 0;
		}

		public new void Render()
        {
            if (stopwatch == null)
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }
            stopwatch.Stop();
			double delta = stopwatch.Elapsed.TotalMilliseconds * 0.001;
            stopwatch.Restart();

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Texture2D);

            int[] vp = new int[4];
            GL.GetInteger(GetPName.Viewport, vp);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(0, vp[2], vp[3], 0, -1.0, 1.0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.ScissorTest);

            var prevGui = GLDraw.CurrentGui;
            GLDraw.CurrentGui = this;
            GLDraw.ControlRect = outer;
            GLDraw.ScissorRect = outer;

            DoRender(new Point(), delta);

            GLDraw.CurrentGui = prevGui;

            GL.Disable(EnableCap.ScissorTest);
            GL.Disable(EnableCap.Blend);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

			lock(toDispose)
			{
				foreach(var d in toDispose)
					d.Dispose();
				toDispose.Clear();
			}
			if(usedVertexArrays != lastUsedVertexArrays)
			{
				lastUsedVertexArrays = usedVertexArrays;
				if(usedVertexArrays > 2048)
					Console.WriteLine("Warning: Used vertex arrays by GLGUI: {0}", usedVertexArrays);
				GC.Collect();
			}
			if(usedTextures != lastUsedTextures)
			{
				lastUsedTextures = usedTextures;
				if(usedTextures > 32)
					Console.WriteLine("Warning: Used textures by GLGUI: {0}", usedTextures);
				GC.Collect();
			}

			renderDuration = stopwatch.Elapsed.TotalMilliseconds;
        }

		internal void OpenContextMenu(GLContextMenu contextMenu, Point position)
		{
			if (currentContextMenu != null)
				Remove(currentContextMenu);

			currentContextMenu = contextMenu;

			if (currentContextMenu != null)
			{
				Add(currentContextMenu);
				currentContextMenu.Location = position;
			}
		}

		internal void CloseContextMenu()
		{
			OpenContextMenu(null, Point.Empty);
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (currentContextMenu != null)
			{
				if (currentContextMenu.Outer.Contains(e.Position))
				{
					currentContextMenu.DoMouseDown(new MouseButtonEventArgs(e.X - currentContextMenu.Outer.X, e.Y - currentContextMenu.Outer.Y, e.Button, e.IsPressed));
					return;
				}
				else
					CloseContextMenu();
			}
			    
			DoMouseDown(e);
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (currentContextMenu != null && currentContextMenu.Outer.Contains(e.Position))
			{
				currentContextMenu.DoMouseUp(new MouseButtonEventArgs(e.X - currentContextMenu.Outer.X, e.Y - currentContextMenu.Outer.Y, e.Button, e.IsPressed));
				return;
			}

			DoMouseUp(e);
		}
	}
}