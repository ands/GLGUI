using System.Drawing;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections;
using GLGUI.Advanced;

namespace GLGUI.DataView
{
	public class MainForm : Form
	{
		OpenGLControl glcontrol;
		GLGui glgui;
        GLLabel fpsLabel;
        GLLabel console;
        LineWriter consoleWriter;
		DataControl dataViewer;

		public MainForm()
		{
			this.Size = new Size(1024, 600);

            consoleWriter = new LineWriter();
            Console.SetOut(consoleWriter);
            Console.SetError(consoleWriter);

			glcontrol = new OpenGLControl();
			glcontrol.Dock = DockStyle.Fill;
			glcontrol.OpenGLLoad += OpenGLLoad;
			glcontrol.OpenGLResize += OpenGLResize;
			glcontrol.OpenGLFrame += OpenGLFrame;
            glcontrol.OpenGLFPS += fps => fpsLabel.Text = "Application: " + fps.ToString("0") + "FPS. GLGUI: " + (glgui.RenderDuration * 1000.0f).ToString("0.0") + "ms (" + (1.0 / glgui.RenderDuration).ToString("0") + "FPS)";
		    Controls.Add(glcontrol);
		}

		private void OpenGLLoad()
		{
			glgui = new GLGui(glcontrol);

			var splitterH = glgui.Add(new GLSplitLayout(glgui) { Orientation = GLSplitterOrientation.Horizontal, SplitterPosition = 0.8f, Outer = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height), Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom });
			var splitterV = splitterH.Add(new GLSplitLayout(glgui) { Orientation = GLSplitterOrientation.Vertical, SplitterPosition = 0.6f });

			// console
			var consoleScrollControl = splitterH.Add(new GLScrollableControl(glgui));
			console = consoleScrollControl.Add(new GLLabel(glgui) { AutoSize = true });
			var skin = console.SkinEnabled;
			skin.Font = new GLFont(new Font(FontFamily.GenericMonospace, 8.0f));
			skin.Color = Color.FromArgb(96, 96, 96);
			skin.BackgroundColor = Color.FromArgb(240, 240, 240);
			console.SkinEnabled = skin;
            
			// viewport
			var viewport = splitterV.Add(new WorldRenderer(glgui));
			fpsLabel = viewport.Add(new GLLabel(glgui) { Location = new Point(4, 4), AutoSize = true });
            fpsLabel.SkinEnabled = skin;

            // flowLayout on the right
            var rightFlow = splitterV.Add(new GLFlowLayout(glgui) { FlowDirection = FlowDirection.TopDown });
            var rightFlowSkin = rightFlow.Skin;
            rightFlowSkin.BackgroundColor = Color.FromArgb(240, 240, 240);
            rightFlow.Skin = rightFlowSkin;

            // options
            var options = rightFlow.Add(new GLOptions(glgui) { FlowDirection = FlowDirection.TopDown, AutoSize = true });
            options.Add(new GLCheckBox(glgui) { Text = "First", AutoSize = true });
            options.Add(new GLCheckBox(glgui) { Text = "Second", AutoSize = true });
            options.Add(new GLCheckBox(glgui) { Text = "Third", AutoSize = true });

			// dataviewer
            dataViewer = rightFlow.Add(new DataControl(glgui) { Outer = new Rectangle(0, 0, rightFlow.InnerWidth, rightFlow.InnerHeight - options.Height - rightFlow.Skin.Space), Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom });
			dataViewer.Hidden.Add(typeof(IEnumerable), new string[] {
                "_items", "_size", "_version", "_syncRoot",
                "m_buckets", "m_slots", "m_count", "m_lastIndex", "m_freeList", "m_comparer",
                "m_version", "m_siInfo", "m_collection", "m_boundedCapacity", "m_freeNodes",
                "m_occupiedNodes", "m_isDisposed", "m_ConsumersCancellationTokenSource",
                "m_ProducersCancellationTokenSource", "m_currentAdders",
                "buckets", "entries", "count", "version", "freeList", "freeCount", "comparer", "keys", "values",
                "IsFixedSize", "IsReadOnly", "IsSynchronized", "SyncRoot" });
            dataViewer.Hidden.Add(typeof(Array), new string[] { "LongLength", "Rank", "Count" });
            dataViewer.Hidden.Add(typeof(KeyValuePair<,>), new string[] { "key", "value" });
            dataViewer.Hidden.Add(typeof(Dictionary<,>), new string[] { "Keys", "Values" });
            dataViewer.SetData(glgui);
		}

		private void OpenGLResize(int width, int height)
		{
            GL.Viewport(ClientSize);
		}

		private void OpenGLFrame(double time, double delta)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);

            if (consoleWriter.Changed)
            {
                console.Text = string.Join("\n", consoleWriter.Lines);
                consoleWriter.Changed = false;
            }

			glgui.Render();
			glcontrol.SwapBuffers();
		}
	}
}

