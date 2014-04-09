using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace GLGUI
{
	public class GLViewport : GLControl
	{
        public float AspectRatio { get { return (float)Inner.Width / (float)Inner.Height; } }
		public event RenderEventHandler RenderViewport;

		public GLViewport(GLGui gui) : base(gui)
		{
            Render += OnRender;

			outer = new Rectangle(0, 0, 256, 256);
			sizeMin = new Size(1, 1);
			sizeMax = new Size(int.MaxValue, int.MaxValue);
		}

        protected override void UpdateLayout()
		{
			outer.Width = Math.Min(Math.Max(outer.Width, sizeMin.Width), sizeMax.Width);
            outer.Height = Math.Min(Math.Max(outer.Height, sizeMin.Height), sizeMax.Height);
			Inner = new Rectangle(0, 0, outer.Width, outer.Height);
		}

        private void OnRender(object sender, double timeDelta)
        {
            if (RenderViewport == null)
                return;

            // save
            int[] mainViewport = new int[4];
            GL.GetInteger(GetPName.Viewport, mainViewport);
            // TODO: get rid of these:
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            
            // render
            var vp = ToViewport(Inner);
            GL.Scissor(GLDraw.ScissorRect.X, Gui.Outer.Height - GLDraw.ScissorRect.Bottom, GLDraw.ScissorRect.Width, GLDraw.ScissorRect.Height);
            GL.Viewport(vp.X, mainViewport[3] - vp.Y - vp.Height, vp.Width, vp.Height);
            RenderViewport(this, timeDelta);

            // restore
            GL.Viewport(mainViewport[0], mainViewport[1], mainViewport[2], mainViewport[3]);
            // TODO: get rid of these:
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
        }
	}
}

