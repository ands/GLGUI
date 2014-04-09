using System;
using System.Drawing;
using OpenTK;
using OpenTK.Input;

#if REFERENCE_WINDOWS_FORMS
using Clipboard = System.Windows.Forms.Clipboard;
#endif

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
        private GLFontTextPosition cursorPosition = new GLFontTextPosition();
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
			Focus += OnFocus;
			FocusLost += OnFocusLost;
			KeyDown += OnKeyDown;
			KeyPress += OnKeyPress;

			skinEnabled = Gui.Skin.TextBoxEnabled;
			skinActive = Gui.Skin.TextBoxActive;
			skinHover = Gui.Skin.TextBoxHover;
			skinDisabled = Gui.Skin.TextBoxDisabled;

			outer = new Rectangle(0, 0, 100, 0);

			ContextMenu = new GLContextMenu(gui);
			ContextMenu.Add(new GLContextMenuEntry(gui) { Text = "Copy" }).Click += (s, e) => Clipboard.SetText(text);
			ContextMenu.Add(new GLContextMenuEntry(gui) { Text = "Paste" }).Click += (s, e) => { if(Clipboard.ContainsText()) Insert(Clipboard.GetText()); };
		}

        protected override void UpdateLayout()
		{
			skin = Enabled ? (HasFocus ? skinActive : (over ? skinHover : skinEnabled)) : skinDisabled;

			outer.Width = Math.Min(Math.Max(outer.Width, sizeMin.Width), sizeMax.Width);

            textSize = skin.Font.ProcessText(textProcessed, text,
				new SizeF(WordWrap ? outer.Width - skin.Border.Horizontal - skin.Padding.Horizontal : float.MaxValue, Multiline ? (AutoSize ? float.MaxValue : outer.Height - skin.Border.Vertical - skin.Padding.Vertical) : skin.Font.LineSpacing),
				GLFontAlignment.Left);
			int minHeight = Math.Max(sizeMin.Height, (int)textSize.Height + skin.Border.Vertical + skin.Padding.Vertical);

			if (AutoSize)
			{
				outer.Width = (int)textSize.Width + skin.Border.Horizontal + skin.Padding.Horizontal;
                outer.Height = minHeight;
				outer.Width = Math.Min(Math.Max(outer.Width, sizeMin.Width), sizeMax.Width);
			}

			outer.Height = Math.Min(Math.Max(Multiline ? outer.Height : (int)skin.Font.LineSpacing + skin.Border.Vertical + skin.Padding.Vertical, sizeMin.Height), sizeMax.Height);

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
        private void OnRender(double timeDelta)
		{
            caretBlinkTimer += timeDelta;

			GLDraw.Fill(ref skin.BorderColor);
			GLDraw.FillRect(ref background, ref skin.BackgroundColor);
            GLDraw.Text(textProcessed, Inner, ref skin.Color);
			if (enabled)
			{
				if (HasFocus && caretBlinkTimer < caretBlinkInterval)
                    GLDraw.FillRect(
                        new Rectangle(
                            Inner.X + (int)cursorPosition.Position.X,
						    Inner.Y + (int)cursorPosition.Position.Y,
                            1, (int)skin.Font.LineSpacing), ref skin.Color);
				else if (caretBlinkTimer > caretBlinkInterval * 2.0)
						caretBlinkTimer -= caretBlinkInterval * 2.0;
			}
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (enabled && e.Button == MouseButton.Left)
			{
				isDragged = true;
                cursorPosition.Index = int.MaxValue;
				cursorPosition.Position = new Vector2(e.X - Inner.X, e.Y - Inner.Y);
                cursorPosition = skin.Font.GetTextPosition(textProcessed, cursorPosition);
			}
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (enabled && e.Button == MouseButton.Left)
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
			Gui.Cursor = GLCursor.IBeam;
            Invalidate();
		}

		private void OnMouseLeave(object sender, EventArgs e)
		{
			over = false;
			Gui.Cursor = GLCursor.Default;
            Invalidate();
		}

		private void OnFocus(object sender, EventArgs e)
		{
			Invalidate();
		}

		private void OnFocusLost(object sender, EventArgs e)
		{
            Invalidate();
		}

		private bool OnKeyPress(object sender, KeyPressEventArgs e)
		{
			if (!enabled)
				return false;
			if (!char.IsControl(e.KeyChar))
			{
				Insert(e.KeyChar.ToString());
				return true;
			}
			return false;
		}

		private bool OnKeyDown(object sender, KeyboardKeyEventArgs e)
		{
			if (!enabled)
				return false;
			if (e.Key == Key.Delete && cursorPosition.Index < text.Length)
			{
                text = text.Remove(cursorPosition.Index, 1);
                Invalidate();
                if (Changed != null) Changed(this, e);
				return true;
			}
			if (e.Key == Key.BackSpace && cursorPosition.Index > 0)
			{
				text = text.Remove(--cursorPosition.Index, 1);
				Invalidate();
				if (Changed != null) Changed(this, e);
				return true;
			}
			if (e.Key == Key.Enter && Multiline)
			{
				Insert("\n");
				return true;
			}
            if (e.Key == Key.Left && cursorPosition.Index > 0)
			{
                cursorPosition.Index--;
                cursorPosition.Position = new Vector2(float.MaxValue, float.MaxValue);
                cursorPosition = skin.Font.GetTextPosition(textProcessed, cursorPosition);
				return true;
			}
            if (e.Key == Key.Right && cursorPosition.Index < text.Length)
			{
                cursorPosition.Index++;
                cursorPosition.Position = new Vector2(float.MaxValue, float.MaxValue);
                cursorPosition = skin.Font.GetTextPosition(textProcessed, cursorPosition);
				return true;
			}
            if (e.Key == Key.Up && cursorPosition.Position.Y > 0.0f)
            {
                cursorPosition.Index = int.MaxValue;
                cursorPosition.Position.Y -= skin.Font.LineSpacing;
                cursorPosition = skin.Font.GetTextPosition(textProcessed, cursorPosition);
				return true;
            }
            if (e.Key == Key.Down)
            {
                cursorPosition.Index = int.MaxValue;
                cursorPosition.Position.Y += skin.Font.LineSpacing;
                cursorPosition = skin.Font.GetTextPosition(textProcessed, cursorPosition);
				return true;
            }
			if (e.Control && e.Key == Key.C)
			{
				Clipboard.SetText(text);
				return true;
			}
			if (e.Control && e.Key == Key.V)
            {
                if (Clipboard.ContainsText())
					Insert(Clipboard.GetText());
				return true;
            }
            if (e.Key == Key.Tab)
            {
				Insert("\t");
				return true;
            }
			return false;
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

