using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Linq;

namespace GLGUI
{
	public class GLGui : GLControl
	{
		public new Control Parent;
		public GLSkin Skin = new GLSkin();
		public double RenderDuration { get { return renderDuration; } }
		public bool LayoutSuspended { get { return suspendCounter > 0; } }

		private GLContextMenu currentContextMenu;
        private Stopwatch stopwatch;
		private double renderDuration;
		private int suspendCounter = 0;

		public GLGui(Control parent) : base(null)
		{
			Gui = this;
			base.Parent = this;
			Parent = parent;
			Outer = parent.ClientRectangle;
			Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            parent.MouseMove += (s, e) => DoMouseMove(e);
			parent.MouseDown += OnMouseDown;
            parent.MouseUp += (s, e) => DoMouseUp(e);
			parent.MouseClick += (s, e) => DoMouseClick(e);
            parent.MouseDoubleClick += (s, e) => DoMouseDoubleClick(e);
            parent.MouseWheel += (s, e) => DoMouseWheel(e);
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

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            DoRender(new Point(), outer, delta);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);

            GL.Disable(EnableCap.ScissorTest);
            GL.Disable(EnableCap.Blend);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

			renderDuration = stopwatch.Elapsed.TotalMilliseconds * 0.001;
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

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (currentContextMenu != null && !currentContextMenu.Outer.Contains(e.Location))
				CloseContextMenu();
			DoMouseDown(e);
		}
	}
}