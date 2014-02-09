using System;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.Drawing;
using GLGUI;
using OpenTK;

namespace GLGUI
{
	public class GLCheckBox : GLControl
	{
        public string Text { get { return text; } set { if (value != text) { text = value; Invalidate(); } } }
        public bool Enabled { get { return enabled; } set { enabled = value; Invalidate(); } }
        public bool Checked { get { return _checked; } set { _checked = value; Invalidate(); } }
        public GLSkin.GLCheckBoxSkin SkinEnabled { get { return skinEnabled; } set { skinEnabled = value; Invalidate(); } }
        public GLSkin.GLCheckBoxSkin SkinPressed { get { return skinPressed; } set { skinPressed = value; Invalidate(); } }
        public GLSkin.GLCheckBoxSkin SkinHover { get { return skinHover; } set { skinHover = value; Invalidate(); } }
        public GLSkin.GLCheckBoxSkin SkinDisabled { get { return skinDisabled; } set { skinDisabled = value; Invalidate(); } }

		public event EventHandler Changed;

		private string text = "";
        private GLFontText textProcessed = new GLFontText();
		private SizeF textSize;
		private GLSkin.GLCheckBoxSkin skinEnabled, skinPressed, skinHover, skinDisabled;
		private GLSkin.GLCheckBoxSkin skin;
		private bool enabled = true;
		private bool _checked = false;
		private bool down = false;
		private bool over = false;
		private Rectangle outerBox = new Rectangle(0, 0, 10, 10);
		private Rectangle innerBox;

		public GLCheckBox(GLGui gui) : base(gui)
		{
			Render += OnRender;
			MouseDown += OnMouseDown;
			MouseUp += OnMouseUp;
			MouseEnter += OnMouseEnter;
			MouseLeave += OnMouseLeave;

			skinEnabled = Gui.Skin.CheckBoxEnabled;
			skinPressed = Gui.Skin.CheckBoxPressed;
			skinHover = Gui.Skin.CheckBoxHover;
			skinDisabled = Gui.Skin.CheckBoxDisabled;

			outer = outerBox;
			sizeMin = outerBox.Size;
			sizeMax = new Size(int.MaxValue, int.MaxValue);
		}

        protected override void UpdateLayout()
		{
			skin = Enabled ? (down ? skinPressed : (over ? skinHover : skinEnabled)) : skinDisabled;

			innerBox = new Rectangle(skin.Border.Left, skin.Border.Top,
				outerBox.Width - skin.Border.Horizontal, outerBox.Height - skin.Border.Vertical);

			outer.Width = Math.Min(Math.Max(outer.Width, sizeMin.Width), sizeMax.Width);

			int innerWidth = outer.Width - skin.Border.Horizontal - outerBox.Width;
            textSize = skin.Font.ProcessText(textProcessed, text, GLFontAlignment.Left);
			int minHeight = Math.Max(sizeMin.Height, (int)textSize.Height + skin.Border.Vertical);

			if (AutoSize)
			{
				innerWidth = (int)textSize.Width;
				outer.Width = innerWidth + outerBox.Width + skin.Border.Horizontal;
				outer.Height = minHeight;
				outer.Width = Math.Min(Math.Max(outer.Width, sizeMin.Width), sizeMax.Width);
			}

			outer.Height = Math.Min(Math.Max(outer.Height, minHeight), sizeMax.Height);

			Inner = new Rectangle(
				outerBox.Right + skin.Border.Left, skin.Border.Top,
				innerWidth, outer.Height - skin.Border.Vertical);
		}

        private void OnRender(Rectangle scissorRect, double timeDelta)
		{
			GLDraw.FilledRectangle(outerBox.Size, skin.BorderColor);
			GLDraw.FilledRectangle(innerBox, skin.BackgroundColor);
			if (_checked)
			{
				GLDraw.FilledRectangle(new Rectangle(innerBox.X + skin.Border.Left, innerBox.Y + skin.Border.Top,
					innerBox.Width - skin.Border.Horizontal, innerBox.Height - skin.Border.Vertical), skin.BorderColor);
				/*GLDraw.Line(innerBox.X + skin.Border.Left, innerBox.Y + skin.Border.Top,
					innerBox.Right - skin.Border.Right, innerBox.Bottom - skin.Border.Bottom, skin.BorderColor);
				GLDraw.Line(innerBox.Right - skin.Border.Right, innerBox.Y + skin.Border.Top,
					innerBox.X + skin.Border.Left, innerBox.Bottom - skin.Border.Bottom, skin.BorderColor);*/
			}
			Scissor(scissorRect, Inner);
            skin.Font.Print(textProcessed, new Vector2(Inner.X, Inner.Y), skin.Color);
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && outerBox.Contains(e.Location))
			{
				isDragged = true;
				down = true;
                Invalidate();
			}
		}

		private void OnMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (down)
				{
					down = false;
					isDragged = false;
					if(outerBox.Contains(e.Location))
					{
						_checked = !_checked;
						if(Changed != null)
							Changed(this, e);
					}
                    Invalidate();
				}
			}
		}

		private void OnMouseEnter(object sender, EventArgs e)
		{
			over = true;
            Invalidate();
		}

		private void OnMouseLeave(object sender, EventArgs e)
		{
			over = false;
            Invalidate();
		}
	}
}

