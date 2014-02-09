using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GLGUI.DataView
{
	public class OpenGLControl : OpenTK.GLControl
	{
		public delegate void LoadHandler();
		public delegate void ResizeHandler(int width, int height);
		public delegate void FrameHandler(double time, double delta);
		public delegate void FPSHandler(uint fps);

		public event LoadHandler OpenGLLoad;
		public event ResizeHandler OpenGLResize;
		public event FrameHandler OpenGLFrame;
		public event FPSHandler OpenGLFPS;

		private Stopwatch stopwatch;
		private bool loaded = false;
		private double time = 0.0;
		private uint fpsSecond = 1;
		private uint fpsCounter = 0;

		public OpenGLControl() : base(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 0, 4))
		{
			Load += glcontrol_Load;
			Resize += glcontrol_Resize;
			Paint += glcontrol_Paint;

			stopwatch = new Stopwatch();
		}

		private void glcontrol_Load(object sender, EventArgs e)
		{
			VSync = false;
			if(OpenGLResize != null)
				OpenGLResize(ClientSize.Width, ClientSize.Height);
			if(OpenGLLoad != null)
				OpenGLLoad();
			loaded = true;
			Application.Idle += Application_Idle;
			stopwatch.Start();
		}

		private void glcontrol_Resize(object sender, EventArgs e)
		{
			if (!loaded) return;
			if(OpenGLResize != null)
				OpenGLResize(ClientSize.Width, ClientSize.Height);
		}

		private void glcontrol_Paint(object sender, PaintEventArgs e)
		{
			if (!loaded) return;

			stopwatch.Stop();
			double delta = stopwatch.Elapsed.TotalMilliseconds * 0.001;
			stopwatch.Restart();
			time += delta;

			if (time >= fpsSecond)
			{
				if(OpenGLFPS != null)
					OpenGLFPS(fpsCounter);
				fpsCounter = 0;
				fpsSecond++;
			}

			if(OpenGLFrame != null)
				OpenGLFrame(time, delta);

			fpsCounter++;
		}

		void Application_Idle(object sender, EventArgs e)
		{
			Invalidate();
		}

		protected override bool IsInputKey(Keys key)
		{
			switch (key)
			{
			case Keys.Up:
			case Keys.Down:
			case Keys.Right:
			case Keys.Left:
            case Keys.Tab:
				return true;
			}
			return base.IsInputKey(key);
		}
	}
}

