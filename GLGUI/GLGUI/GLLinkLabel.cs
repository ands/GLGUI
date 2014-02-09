using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GLGUI;

namespace GLGUI
{
	public class GLLinkLabel : GLControl
	{
        public string Text { get { return text; } set { if (value != text) { text = value; Invalidate(); } } }
        public bool Enabled { get { return enabled; } set { enabled = value; Invalidate(); } }
		public GLSkin.GLLinkLabelSkin SkinEnabled { get { return skinEnabled; } set { skinEnabled = value; Invalidate(); } }
		public GLSkin.GLLinkLabelSkin SkinDisabled { get { return skinDisabled; } set { skinDisabled = value; Invalidate(); } }

		public event EventHandler Click;

		private string text = "";
		private GLFontText textProcessed = new GLFontText();
		private SizeF textSize;
		private GLSkin.GLLinkLabelSkin skinEnabled, skinDisabled;
		private GLSkin.GLLinkLabelSkin skin;
		private bool enabled = true;

		public GLLinkLabel(GLGui gui) : base(gui)
		{
			Render += OnRender;
			MouseClick += OnMouseClick;
			MouseEnter += OnMouseEnter;
			MouseLeave += OnMouseLeave;

			skinEnabled = Gui.Skin.LinkLabelEnabled;
			skinDisabled = Gui.Skin.LinkLabelDisabled;

			outer = new Rectangle(0, 0, 0, 0);
			sizeMin = new Size(1, 1);
			sizeMax = new Size(int.MaxValue, int.MaxValue);

			ContextMenu = new GLContextMenu(gui);
			ContextMenu.Add(new GLContextMenuEntry(gui) { Text = "Copy" }).Click += (s, e) => Clipboard.SetText(text);
		}

        protected override void UpdateLayout()
		{
			skin = Enabled ? skinEnabled : skinDisabled;

            textSize = skin.Font.ProcessText(textProcessed, text, GLFontAlignment.Left);

			if (AutoSize)
			{
				outer.Width = (int)textSize.Width + skin.Padding.Horizontal;
                outer.Height = (int)textSize.Height + skin.Padding.Vertical;
			}

            outer.Width = Math.Min(Math.Max(outer.Width, sizeMin.Width), sizeMax.Width);
            outer.Height = Math.Min(Math.Max(outer.Height, sizeMin.Height), sizeMax.Height);

			Inner = new Rectangle(skin.Padding.Left, skin.Padding.Top, outer.Width - skin.Padding.Horizontal, outer.Height - skin.Padding.Vertical);
		}

        private void OnRender(Rectangle scissorRect, double timeDelta)
		{
			GLDraw.FilledRectangle(outer.Size, skin.BackgroundColor);
			Scissor(scissorRect, Inner);
			skin.Font.Print(textProcessed, new Vector2(Inner.Left, Inner.Top), skin.Color);
			//GLDraw.Line(Inner.Left, Inner.Bottom, Inner.Right, Inner.Bottom, skin.Color); // very ugly on windows : /
		}

		private void OnMouseClick(object sender, MouseEventArgs e)
		{
			Gui.Parent.Cursor = Cursors.Default;
			if (Click != null)
				Click(this, EventArgs.Empty);
		}

		private void OnMouseEnter(object sender, EventArgs e)
		{
			Gui.Parent.Cursor = Cursors.Hand;
		}

		private void OnMouseLeave(object sender, EventArgs e)
		{
			Gui.Parent.Cursor = Cursors.Default;
		}
	}
}

