using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace GLGUI
{
	public class GLViewport : GLControl
	{
        public float AspectRatio { get { return (float)Inner.Width / (float)Inner.Height; } }

        public delegate void ViewportRenderEventHandler(GLViewport source, double timeDelta);
		public new event ViewportRenderEventHandler Render;

		public GLViewport(GLGui gui) : base(gui)
		{
            base.Render += OnRender;

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

        public void OnRender(double timeDelta)
        {
            if(Render == null)
                return;

            GL.Scissor(GLDraw.ScissorRect.X, Gui.Outer.Height - GLDraw.ScissorRect.Bottom, GLDraw.ScissorRect.Width, GLDraw.ScissorRect.Height);

            // save
            int[] mainViewport = new int[4];
            GL.GetInteger(GetPName.Viewport, mainViewport);
            var vp = ToViewport(Inner);
            GL.Viewport(vp.X, mainViewport[3] - vp.Y - vp.Height, vp.Width, vp.Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            //GL.LoadIdentity();
            //GL.Ortho(0, vp.Width, vp.Height, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            //GL.LoadIdentity();

            //GL.PushAttrib(AttribMask.AllAttribBits);

            // render
            Render(this, timeDelta);

            // restore
            //GL.PopAttrib();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

            GL.Viewport(mainViewport[0], mainViewport[1], mainViewport[2], mainViewport[3]);
        }
	}
}

