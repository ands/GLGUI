using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GLGUI.Example
{
    public class LineWriter : TextWriter
    {
        public List<string> Lines;
        public bool Changed = false;
        private StringBuilder currentLine;

        public LineWriter()
        {
            this.Lines = new List<string>(1024);
            this.currentLine = new StringBuilder(256);
        }

        public override void Write(char value)
        {
            if (value == '\n')
                Flush();
            else
                currentLine.Append(value);
        }

        public override void Flush()
        {
            Lines.Add(currentLine.ToString());
            if (Lines.Count > 1024)
                Lines.RemoveAt(0);
            currentLine.Clear();
            Changed = true;
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }

        public void Clear()
        {
            Lines.Clear();
            currentLine.Clear();
            Changed = true;
        }
    }

	public class MainForm : GameWindow
	{
		GLGui glgui;
        GLLabel fpsLabel;
        GLLabel console;
        LineWriter consoleWriter;

		public MainForm() : base(1024, 600, new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 0, 4), "GLGUI Example")
		{
            consoleWriter = new LineWriter();
            Console.SetOut(consoleWriter);
            Console.SetError(consoleWriter);

			this.Load += OpenGLLoad;
			this.Resize += (s, e) => GL.Viewport(ClientSize);
			this.RenderFrame += OpenGLFrame;
		}

		private void OpenGLLoad(object src, EventArgs ev)
		{
			VSync = VSyncMode.Off;

			glgui = new GLGui(this);
            
            var mainAreaControl = glgui.Add(new GLGroupLayout(glgui) { Outer = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height - 200), Anchor = GLAnchorStyles.Left | GLAnchorStyles.Right | GLAnchorStyles.Top | GLAnchorStyles.Bottom });
            var mainSkin = mainAreaControl.Skin;
			mainSkin.BackgroundColor = glgui.Skin.FormActive.BackgroundColor;
			mainSkin.BorderColor = glgui.Skin.FormActive.BorderColor;
            mainAreaControl.Skin = mainSkin;

            var consoleScrollControl = glgui.Add(new GLScrollableControl(glgui) { Outer = new Rectangle(0, ClientSize.Height - 200, ClientSize.Width, 200), Anchor = GLAnchorStyles.Left | GLAnchorStyles.Right | GLAnchorStyles.Bottom });
            console = consoleScrollControl.Add(new GLLabel(glgui) { AutoSize = true });
            var skin = console.SkinEnabled;
			skin.Font = new GLFont(new Font("Arial", 8.0f));
			skin.BackgroundColor = glgui.Skin.TextBoxActive.BackgroundColor;
            console.SkinEnabled = skin;

            fpsLabel = mainAreaControl.Add(new GLLabel(glgui) { Location = new Point(10, 10), AutoSize = true });
            fpsLabel.SkinEnabled = skin;

            var a = mainAreaControl.Add(new GLForm(glgui) { Title = "Hello World", Outer = new Rectangle(100, 100, 200, 150), AutoSize = false });
            var c = a.Add(new GLForm(glgui) { Title = "Hello Form", Outer = new Rectangle(10, 10, 100, 100), AutoSize = false });
			c.MouseMove += (s, e) => Console.WriteLine(e.Position.ToString());

            var flow = a.Add(new GLFlowLayout(glgui) { FlowDirection = GLFlowDirection.BottomUp, Outer = new Rectangle(0, 0, a.Inner.Width, a.Inner.Height), AutoSize = true });
            for (int i = 0; i < 5; i++)
                flow.Add(new GLButton(glgui) { Text = "Button" + i, Outer = new Rectangle(10, 32, 150, 0) }).Click += (s, e) => Console.WriteLine(s + " pressed.");
			flow.Add(new GLButton(glgui) { Text = "Hide Cursor", Outer = new Rectangle(10, 32, 150, 0) }).Click += (s, e) => glgui.Cursor = GLCursor.None;

			for (int i = 0; i < 1; i++)
			{
                var b = mainAreaControl.Add(new GLForm(glgui) { Title = "Cube", Size = new Size(200, 200) });
                //var c = b.Add(new GLScrollableControl(glgui) { Outer = new Rectangle(0, 0, b.Inner.Width, b.Inner.Height), ContentSize = new Size(400, 200), Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom });
                var d = b.Add(new GLViewport(glgui) { Size = b.InnerSize, Anchor = GLAnchorStyles.Left | GLAnchorStyles.Top | GLAnchorStyles.Right | GLAnchorStyles.Bottom });
                double t = 0.0f;
                d.RenderViewport += (vp, delta) =>
                {
                    GL.Enable(EnableCap.DepthTest);
                    GL.ClearColor(0, 0, 0, 1);
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    GL.MatrixMode(MatrixMode.Projection);
                    var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), d.AspectRatio, 1.0f, 100.0f);
                    GL.LoadMatrix(ref proj);
                    GL.MatrixMode(MatrixMode.Modelview);
                    GL.LoadIdentity();

                    GL.Translate(0, 0, -2.0f);
                    GL.Rotate(t * 100.0f, 1, 0, 0);
                    GL.Rotate(t * 42.0f, 0, 1, 0);
                    t += delta;

					GL.Begin(PrimitiveType.Quads);
                    GL.Color3(1.0, 0.0, 0.0);
                    GL.Vertex3(0.5, -0.5, -0.5);
                    GL.Color3(0.0, 1.0, 0.0);
                    GL.Vertex3(0.5, 0.5, -0.5);
                    GL.Color3(0.0, 0.0, 1.0);
                    GL.Vertex3(-0.5, 0.5, -0.5);
                    GL.Color3(1.0, 0.0, 1.0);
                    GL.Vertex3(-0.5, -0.5, -0.5);

                    GL.Color3(1.0, 1.0, 1.0);
                    GL.Vertex3(0.5, -0.5, 0.5);
                    GL.Vertex3(0.5, 0.5, 0.5);
                    GL.Vertex3(-0.5, 0.5, 0.5);
                    GL.Vertex3(-0.5, -0.5, 0.5);

                    GL.Color3(1.0, 0.0, 1.0);
                    GL.Vertex3(0.5, -0.5, -0.5);
                    GL.Vertex3(0.5, 0.5, -0.5);
                    GL.Vertex3(0.5, 0.5, 0.5);
                    GL.Vertex3(0.5, -0.5, 0.5);

                    GL.Color3(0.0, 1.0, 0.0);
                    GL.Vertex3(-0.5, -0.5, 0.5);
                    GL.Vertex3(-0.5, 0.5, 0.5);
                    GL.Vertex3(-0.5, 0.5, -0.5);
                    GL.Vertex3(-0.5, -0.5, -0.5);

                    GL.Color3(0.0, 0.0, 1.0);
                    GL.Vertex3(0.5, 0.5, 0.5);
                    GL.Vertex3(0.5, 0.5, -0.5);
                    GL.Vertex3(-0.5, 0.5, -0.5);
                    GL.Vertex3(-0.5, 0.5, 0.5);

                    GL.Color3(1.0, 0.0, 0.0);
                    GL.Vertex3(0.5, -0.5, -0.5);
                    GL.Vertex3(0.5, -0.5, 0.5);
                    GL.Vertex3(-0.5, -0.5, 0.5);
                    GL.Vertex3(-0.5, -0.5, -0.5);
                    GL.End();

                    GL.Disable(EnableCap.DepthTest);
                };

                b = mainAreaControl.Add(new GLForm(glgui) { Title = "Lorem Ipsum", Size = new Size(300, 200) });
				var x = b.Add(new GLTextBox(glgui) { Text = "Lorem ipsum dolor sit amet,\nconsetetur sadipscing elitr,\nsed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat,\nsed diam voluptua.\n\nAt vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.", Multiline = true, WordWrap = true, Outer = new Rectangle(4, 4, b.Inner.Width - 8, b.Inner.Height - 8), Anchor = GLAnchorStyles.Left | GLAnchorStyles.Top | GLAnchorStyles.Right | GLAnchorStyles.Bottom });
                x.Changed += (z, e) => Console.WriteLine(z + " text length: " + ((GLTextBox)z).Text.Length);

                b = mainAreaControl.Add(new GLForm(glgui) { Title = "Foo Bar", Size = new Size(100, 200), AutoSize = true });
				//b.Add(new GLButton(glgui) { Text = "Foo Bar", Outer = new Rectangle(32, 24, 75, 50), Anchor = AnchorStyles.Right, AutoSize = true });
                flow = b.Add(new GLFlowLayout(glgui) { FlowDirection = GLFlowDirection.TopDown, AutoSize = true });
				/*var s = flow.Skin;
				s.BackgroundColor = Color.FromArgb(255, 255, 255);
				flow.Skin = s;*/
                flow.Add(new GLCheckBox(glgui) { Text = "Trololo lolo", AutoSize = true }).Changed += (z, e) => Console.WriteLine(z + ": " + ((GLCheckBox)z).Checked);
                flow.Add(new GLCheckBox(glgui) { Text = "Troll", AutoSize = true }).Changed += (z, e) => Console.WriteLine(z + ": " + ((GLCheckBox)z).Checked);
                flow.Add(new GLCheckBox(glgui) { Text = "lolololo", AutoSize = true }).Changed += (z, e) => Console.WriteLine(z + ": " + ((GLCheckBox)z).Checked);
				flow.Add(new GLCheckBox(glgui) { Text = "Trololo lolo", AutoSize = true }).Changed += (z, e) => Console.WriteLine(z + ": " + ((GLCheckBox)z).Checked);
			}
		}

		int fpsCounter = 0;
		int fpsSecond = 1;
		double time = 0.0;

		private void OpenGLFrame(object s, FrameEventArgs e)
		{
			time += e.Time;

			if (time >= fpsSecond)
			{
				fpsLabel.Text = "Application: " + fpsCounter.ToString("0") +
				                "FPS. GLGUI: " + glgui.RenderDuration.ToString("0.0") +
				                "ms (" + (1000.0 / glgui.RenderDuration).ToString("0") + "FPS)";
				fpsCounter = 0;
				fpsSecond++;
			}

			if (consoleWriter.Changed)
			{
				console.Text = string.Join("\n", consoleWriter.Lines);
				consoleWriter.Changed = false;
			}

			glgui.Render();
			SwapBuffers();

			fpsCounter++;
		}
	}
}

