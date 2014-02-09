using System;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.Drawing;
using GLGUI;
using OpenTK;

namespace GLGUI
{
	public class GLTextBox : GLControl
	{
        public string Text { get { return text; } set { if (value != text) { text = value; Invalidate(); } } }
        public bool Enabled { get { return enabled; } set { enabled = value; Invalidate(); } }
        public GLSkin.GLTextBoxSkin SkinEnabled { get { return skinEnabled; } set { skinEnabled = value; Invalidate(); } }
        public GLSkin.GLTextBoxSkin SkinActive { get { return skinActive; } set { skinActive = value; Invalidate(); } }
        public GLSkin.GLTextBoxSkin SkinHover { get { return skinHover; } set { skinHover = value; Invalidate(); } }
        public GLSkin.GLTextBoxSkin SkinDisabled { get { return skinDisabled; } set { skinDisabled = value; Invalidate(); } }

        public bool WordWrap = false;
        public bool Multiline = false;

        public event EventHandler Changed;

		private string text = "";
        private GLFontText textProcessed = new GLFontText();
		private SizeF textSize;
        private GLFont.TextPosition cursorPosition = new GLFont.TextPosition();
		private GLSkin.GLTextBoxSkin skinEnabled, skinActive, skinHover, skinDisabled;
		private GLSkin.GLTextBoxSkin skin;
		private bool enabled = true;
		private bool over = false;
		private Rectangle background;

		public GLTextBox(GLGui gui) : base(gui)
		{
			Render += OnRender;
			MouseDown += OnMouseDown;
			MouseUp += OnMouseUp;
			MouseEnter += OnMouseEnter;
			MouseLeave += OnMouseLeave;
			FocusLost += OnFocusLost;
			KeyDown += OnKeyDown;
			KeyPress += OnKeyPress;

			skinEnabled = Gui.Skin.TextBoxEnabled;
			skinActive = Gui.Skin.TextBoxActive;
			skinHover = Gui.Skin.TextBoxHover;
			skinDisabled = Gui.Skin.TextBoxDisabled;

			outer = new Rectangle(0, 0, 100, 20);
			sizeMin = new Size(8, 8);
			sizeMax = new Size(int.MaxValue, int.MaxValue);

			ContextMenu = new GLContextMenu(gui);
			ContextMenu.Add(new GLContextMenuEntry(gui) { Text = "Copy" }).Click += (s, e) => Clipboard.SetText(text);
			ContextMenu.Add(new GLContextMenuEntry(gui) { Text = "Paste" }).Click += (s, e) => { if(Clipboard.ContainsText()) Insert(Clipboard.GetText()); };
		}

        protected override void UpdateLayout()
		{
			skin = Enabled ? (HasFocus ? skinActive : (over ? skinHover : skinEnabled)) : skinDisabled;

			outer.Width = Math.Min(Math.Max(outer.Width, sizeMin.Width), sizeMax.Width);

            textSize = skin.Font.ProcessText(textProcessed, text, new SizeF(WordWrap ? outer.Width - skin.Border.Horizontal - skin.Padding.Horizontal : float.MaxValue, Multiline ? (AutoSize ? float.MaxValue : outer.Height - skin.Border.Vertical - skin.Padding.Vertical) : skin.Font.LineSpacing), GLFontAlignment.Left);
			int minHeight = Math.Max(sizeMin.Height, (int)textSize.Height + skin.Border.Vertical + skin.Padding.Vertical);

			if (AutoSize)
			{
				outer.Width = (int)textSize.Width + skin.Border.Horizontal + skin.Padding.Horizontal;
                outer.Height = minHeight;
				outer.Width = Math.Min(Math.Max(outer.Width, sizeMin.Width), sizeMax.Width);
			}

            outer.Height = Math.Min(Math.Max(outer.Height, sizeMin.Height), sizeMax.Height);

			background = new Rectangle(
				skin.Border.Left, skin.Border.Top,
				outer.Width - skin.Border.Horizontal, outer.Height - skin.Border.Vertical);
			Inner = new Rectangle(
				background.Left + skin.Padding.Left, background.Top + skin.Padding.Top,
				background.Width - skin.Padding.Horizontal, background.Height - skin.Padding.Vertical);

            cursorPosition.Position = new Vector2(float.MaxValue, float.MaxValue);
            cursorPosition = skin.Font.GetTextPosition(textProcessed, cursorPosition);
		}

        private double caretBlinkTimer = 0.0;
        private const double caretBlinkInterval = 0.7;
        private void OnRender(Rectangle scissorRect, double timeDelta)
		{
            caretBlinkTimer += timeDelta;

			GLDraw.FilledRectangle(outer.Size, skin.BorderColor);
			GLDraw.FilledRectangle(background, skin.BackgroundColor);
			Scissor(scissorRect, background);
            skin.Font.Print(textProcessed, new Vector2(Inner.Left, Inner.Top), skin.Color);
            if (HasFocus && caretBlinkTimer < caretBlinkInterval)
                GLDraw.Line(
                    Inner.X + (int)cursorPosition.Position.X,
                    Inner.Y + (int)cursorPosition.Position.Y,
                    Inner.X + (int)cursorPosition.Position.X,
                    Inner.Y + (int)cursorPosition.Position.Y + (int)skin.Font.LineSpacing,
                    skin.Color);
            else if (caretBlinkTimer > caretBlinkInterval * 2.0)
                caretBlinkTimer -= caretBlinkInterval * 2.0;
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				isDragged = true;
                cursorPosition.Index = int.MaxValue;
				cursorPosition.Position = new Vector2(e.X - Inner.X, e.Y - Inner.Y);
                cursorPosition = skin.Font.GetTextPosition(textProcessed, cursorPosition);
			}
		}

		private void OnMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (isDragged)
				{
					isDragged = false;
				}
			}
		}

		private void OnMouseEnter(object sender, EventArgs e)
		{
			over = true;
			Gui.Parent.Cursor = Cursors.IBeam;
            Invalidate();
		}

		private void OnMouseLeave(object sender, EventArgs e)
		{
			over = false;
			Gui.Parent.Cursor = Cursors.Default;
            Invalidate();
		}

		private void OnFocusLost(object sender, EventArgs e)
		{
            Invalidate();
		}

		private void OnKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar))
			{
				Insert(e.KeyChar.ToString());
			}
            else if (e.KeyChar == (char)Keys.Back && cursorPosition.Index > 0)
			{
                text = text.Remove(--cursorPosition.Index, 1);
                Invalidate();
                if (Changed != null) Changed(this, e);
			}
            else if (e.KeyChar == (char)Keys.Return && Multiline)
            {
				Insert("\n");
            }
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
            if (e.KeyCode == Keys.Delete && cursorPosition.Index < text.Length)
			{
                text = text.Remove(cursorPosition.Index, 1);
                Invalidate();
                if (Changed != null) Changed(this, e);
			}
            else if (e.KeyCode == Keys.Left && cursorPosition.Index > 0)
			{
                cursorPosition.Index--;
                cursorPosition.Position = new Vector2(float.MaxValue, float.MaxValue);
                cursorPosition = skin.Font.GetTextPosition(textProcessed, cursorPosition);
			}
            else if (e.KeyCode == Keys.Right && cursorPosition.Index < text.Length)
			{
                cursorPosition.Index++;
                cursorPosition.Position = new Vector2(float.MaxValue, float.MaxValue);
                cursorPosition = skin.Font.GetTextPosition(textProcessed, cursorPosition);
			}
            else if (e.KeyCode == Keys.Up && cursorPosition.Position.Y > 0.0f)
            {
                cursorPosition.Index = int.MaxValue;
                cursorPosition.Position.Y -= skin.Font.LineSpacing;
                cursorPosition = skin.Font.GetTextPosition(textProcessed, cursorPosition);
            }
            else if (e.KeyCode == Keys.Down)
            {
                cursorPosition.Index = int.MaxValue;
                cursorPosition.Position.Y += skin.Font.LineSpacing;
                cursorPosition = skin.Font.GetTextPosition(textProcessed, cursorPosition);
            }
            else if ((e.Modifiers & Keys.Control) != 0 && e.KeyCode == Keys.V)
            {
                if (Clipboard.ContainsText())
					Insert(Clipboard.GetText());
            }
            else if (e.KeyCode == Keys.Tab)
            {
				Insert("\t");
            }
		}

		private void Insert(string str)
		{
			text = text.Insert(cursorPosition.Index, str);
			cursorPosition.Index += str.Length;
			Invalidate();
			if (Changed != null) Changed(this, EventArgs.Empty);
		}
	}
}

