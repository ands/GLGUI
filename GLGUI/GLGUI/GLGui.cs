using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections;

namespace GLGUI
{
	public class GLGui : GLControl
	{
        public readonly GameWindow ParentWindow;
		public GLSkin Skin = new GLSkin();
		public double RenderDuration { get { return renderDuration; } }
		public bool LayoutSuspended { get { return suspendCounter > 0; } }

        public GLCursor Cursor
        {
            get { return cursor; }
            set
            {
                cursor = value;
                if(ParentWindow != null)
                    ParentWindow.CursorHandle = cursor.Handle;
#if REFERENCE_OPENTK_GLCONTROL
                if(ParentControl != null)
                    ParentControl.Cursor = cursor.Cursor;
#endif
            }
        }

		internal static List<IDisposable> toDispose = new List<IDisposable>();
		internal static int usedTextures = 0;
		internal static int usedVertexArrays = 0;
		private static int lastUsedTextures = 0;
		private static int lastUsedVertexArrays = 0;

		private GLContextMenu currentContextMenu;
        private Stopwatch stopwatch;
		private double renderDuration;
		private int suspendCounter = 0;
		private GLCursor cursor;

        public GLGui(GameWindow parent) : base(null)
        {
            GLCursor.LoadCursors(parent);

            Gui = this;
            base.Parent = this;
            ParentWindow = parent;
            Outer = parent.ClientRectangle;
            Anchor = GLAnchorStyles.All;

            parent.Mouse.Move += (s, e) => DoMouseMove(e);
            parent.Mouse.ButtonDown += OnMouseDown;
            parent.Mouse.ButtonUp += OnMouseUp;
            parent.Mouse.WheelChanged += (s, e) => DoMouseWheel(e);
            parent.MouseEnter += (s, e) => DoMouseEnter();
            parent.MouseLeave += (s, e) => DoMouseLeave();
            parent.KeyDown += (s, e) => DoKeyDown(e);
            parent.KeyUp += (s, e) => DoKeyUp(e);
            parent.KeyPress += (s, e) => DoKeyPress(e);
            parent.Resize += (s, e) => Outer = parent.ClientRectangle;
        }

#if REFERENCE_OPENTK_GLCONTROL
        public readonly OpenTK.GLControl ParentControl;

        public GLGui(OpenTK.GLControl parent) : base(null)
        {
            GLCursor.LoadCursors(null);

            Gui = this;
            base.Parent = this;
            ParentControl = parent;
            Outer = parent.ClientRectangle;
            Anchor = GLAnchorStyles.All;

            int lastX = 0, lastY = 0, wheelValue = 0;
            parent.MouseMove += (s, e) => { DoMouseMove(new MouseMoveEventArgs(e.X, e.Y, e.X - lastX, e.Y - lastY)); lastX = e.X; lastY = e.Y; };
            parent.MouseDown += (s, e) => OnMouseDown(s, new MouseButtonEventArgs(e.X, e.Y, ToOpenTK(e.Button), true));
            parent.MouseUp += (s, e) => OnMouseUp(s, new MouseButtonEventArgs(e.X, e.Y, ToOpenTK(e.Button), false));
            parent.MouseWheel += (s, e) => { wheelValue += e.Delta; DoMouseWheel(new MouseWheelEventArgs(e.X, e.Y, wheelValue, e.Delta)); };
            parent.MouseEnter += (s, e) => DoMouseEnter();
            parent.MouseLeave += (s, e) => DoMouseLeave();
            parent.KeyDown += (s, e) => DoKeyDown(ToOpenTK(e));
            parent.KeyUp += (s, e) => DoKeyUp(ToOpenTK(e));
            parent.KeyPress += (s, e) => DoKeyPress(new KeyPressEventArgs(e.KeyChar));
            parent.Resize += (s, e) => Outer = parent.ClientRectangle;
        }

        private MouseButton ToOpenTK(System.Windows.Forms.MouseButtons button)
        {
            switch (button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    return MouseButton.Left;
                case System.Windows.Forms.MouseButtons.Middle:
                    return MouseButton.Middle;
                case System.Windows.Forms.MouseButtons.Right:
                    return MouseButton.Right;
                case System.Windows.Forms.MouseButtons.XButton1:
                    return MouseButton.Button1;
                case System.Windows.Forms.MouseButtons.XButton2:
                    return MouseButton.Button2;
                default:
                    return MouseButton.LastButton;
            }
        }

        private Key ToOpenTK(System.Windows.Forms.Keys k)
        {
            switch (k)
            {
                case System.Windows.Forms.Keys.Escape: return Key.Escape;
                case System.Windows.Forms.Keys.D1: return Key.Number1;
                case System.Windows.Forms.Keys.D2: return Key.Number2;
                case System.Windows.Forms.Keys.D3: return Key.Number3;
                case System.Windows.Forms.Keys.D4: return Key.Number4;
                case System.Windows.Forms.Keys.D5: return Key.Number5;
                case System.Windows.Forms.Keys.D6: return Key.Number6;
                case System.Windows.Forms.Keys.D7: return Key.Number7;
                case System.Windows.Forms.Keys.D8: return Key.Number8;
                case System.Windows.Forms.Keys.D9: return Key.Number9;
                case System.Windows.Forms.Keys.D0: return Key.Number0;
                case System.Windows.Forms.Keys.OemMinus: return Key.Minus;
                case System.Windows.Forms.Keys.Oemplus: return Key.Plus;
                case System.Windows.Forms.Keys.Back: return Key.BackSpace;
                case System.Windows.Forms.Keys.Tab: return Key.Tab;
                case System.Windows.Forms.Keys.Q: return Key.Q;
                case System.Windows.Forms.Keys.W: return Key.W;
                case System.Windows.Forms.Keys.E: return Key.E;
                case System.Windows.Forms.Keys.R: return Key.R;
                case System.Windows.Forms.Keys.T: return Key.T;
                case System.Windows.Forms.Keys.Y: return Key.Y;
                case System.Windows.Forms.Keys.U: return Key.U;
                case System.Windows.Forms.Keys.I: return Key.I;
                case System.Windows.Forms.Keys.O: return Key.O;
                case System.Windows.Forms.Keys.P: return Key.P;
                case System.Windows.Forms.Keys.OemOpenBrackets: return Key.BracketLeft;
                case System.Windows.Forms.Keys.OemCloseBrackets: return Key.BracketRight;
                case System.Windows.Forms.Keys.Enter: return Key.Enter;
                case System.Windows.Forms.Keys.LControlKey: return Key.ControlLeft;
                case System.Windows.Forms.Keys.A: return Key.A;
                case System.Windows.Forms.Keys.S: return Key.S;
                case System.Windows.Forms.Keys.D: return Key.D;
                case System.Windows.Forms.Keys.F: return Key.F;
                case System.Windows.Forms.Keys.G: return Key.G;
                case System.Windows.Forms.Keys.H: return Key.H;
                case System.Windows.Forms.Keys.J: return Key.J;
                case System.Windows.Forms.Keys.K: return Key.K;
                case System.Windows.Forms.Keys.L: return Key.L;
                case System.Windows.Forms.Keys.OemSemicolon: return Key.Semicolon;
                case System.Windows.Forms.Keys.OemQuotes: return Key.Quote;
                //case System.Windows.Forms.Keys.: return Key.Grave; ????
                case System.Windows.Forms.Keys.LShiftKey: return Key.ShiftLeft;
                case System.Windows.Forms.Keys.OemBackslash: return Key.BackSlash;
                case System.Windows.Forms.Keys.Z: return Key.Z;
                case System.Windows.Forms.Keys.X: return Key.X;
                case System.Windows.Forms.Keys.C: return Key.C;
                case System.Windows.Forms.Keys.V: return Key.V;
                case System.Windows.Forms.Keys.B: return Key.B;
                case System.Windows.Forms.Keys.N: return Key.N;
                case System.Windows.Forms.Keys.M: return Key.M;
                case System.Windows.Forms.Keys.Oemcomma: return Key.Comma;
                case System.Windows.Forms.Keys.OemPeriod: return Key.Period;
                case System.Windows.Forms.Keys.Oem2: return Key.Slash;
                case System.Windows.Forms.Keys.RShiftKey: return Key.ShiftRight;
                case System.Windows.Forms.Keys.PrintScreen: return Key.PrintScreen;
                case System.Windows.Forms.Keys.Alt: return Key.AltLeft;
                case System.Windows.Forms.Keys.Space: return Key.Space;
                case System.Windows.Forms.Keys.CapsLock: return Key.CapsLock;
                case System.Windows.Forms.Keys.F1: return Key.F1;
                case System.Windows.Forms.Keys.F2: return Key.F2;
                case System.Windows.Forms.Keys.F3: return Key.F3;
                case System.Windows.Forms.Keys.F4: return Key.F4;
                case System.Windows.Forms.Keys.F5: return Key.F5;
                case System.Windows.Forms.Keys.F6: return Key.F6;
                case System.Windows.Forms.Keys.F7: return Key.F7;
                case System.Windows.Forms.Keys.F8: return Key.F8;
                case System.Windows.Forms.Keys.F9: return Key.F9;
                case System.Windows.Forms.Keys.F10: return Key.F10;
                case System.Windows.Forms.Keys.NumLock: return Key.NumLock;
                case System.Windows.Forms.Keys.Scroll: return Key.ScrollLock;
                case System.Windows.Forms.Keys.Home: return Key.Home;
                case System.Windows.Forms.Keys.Up: return Key.Up;
                case System.Windows.Forms.Keys.PageUp: return Key.PageUp;
                case System.Windows.Forms.Keys.Subtract: return Key.KeypadMinus;
                case System.Windows.Forms.Keys.Left: return Key.Left;
                case System.Windows.Forms.Keys.NumPad5: return Key.Keypad5;
                case System.Windows.Forms.Keys.Right: return Key.Right;
                case System.Windows.Forms.Keys.Add: return Key.KeypadPlus;
                case System.Windows.Forms.Keys.End: return Key.End;
                case System.Windows.Forms.Keys.Down: return Key.Down;
                case System.Windows.Forms.Keys.PageDown: return Key.PageDown;
                case System.Windows.Forms.Keys.Insert: return Key.Insert;
                case System.Windows.Forms.Keys.Delete: return Key.Delete;
                //case System.Windows.Forms.Keys.Oem102: return Key.NonUSBackSlash; overlaps other keycode
                case System.Windows.Forms.Keys.F11: return Key.F11;
                case System.Windows.Forms.Keys.F12: return Key.F12;
                case System.Windows.Forms.Keys.Pause: return Key.Pause;
                case System.Windows.Forms.Keys.LWin: return Key.WinLeft;
                case System.Windows.Forms.Keys.RWin: return Key.WinRight;
                case System.Windows.Forms.Keys.Menu: return Key.Menu;
                case System.Windows.Forms.Keys.F13: return Key.F13;
                case System.Windows.Forms.Keys.F14: return Key.F14;
                case System.Windows.Forms.Keys.F15: return Key.F15;
                case System.Windows.Forms.Keys.F16: return Key.F16;
                case System.Windows.Forms.Keys.F17: return Key.F17;
                case System.Windows.Forms.Keys.F18: return Key.F18;
                case System.Windows.Forms.Keys.F19: return Key.F19;
                default: return Key.Unknown;
            }
        }

        private KeyboardKeyEventArgs ToOpenTK(System.Windows.Forms.KeyEventArgs e)
        {
            var e2 = new KeyboardKeyEventArgs();
            e2.GetType().GetProperty("Key").SetValue(e2, ToOpenTK(e.KeyCode), null);
            KeyModifiers modifiers = 0;
            if ((e.Modifiers & System.Windows.Forms.Keys.Alt) != 0)
                modifiers |= KeyModifiers.Alt;
            if ((e.Modifiers & System.Windows.Forms.Keys.Control) != 0)
                modifiers |= KeyModifiers.Control;
            if ((e.Modifiers & System.Windows.Forms.Keys.Shift) != 0)
                modifiers |= KeyModifiers.Shift;
            e2.GetType().GetProperty("Modifiers").SetValue(e2, modifiers, null);
            return e2;
        }
#endif
		
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

            var prevGui = GLDraw.CurrentGui;
            GLDraw.CurrentGui = this;
            GLDraw.ControlRect = outer;
            GLDraw.ScissorRect = outer;

            DoRender(new Point(), delta);

            GLDraw.CurrentGui = prevGui;

            GL.Disable(EnableCap.ScissorTest);
            GL.Disable(EnableCap.Blend);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

			lock(toDispose)
			{
				foreach(var d in toDispose)
					d.Dispose();
				toDispose.Clear();
			}
			if(usedVertexArrays != lastUsedVertexArrays)
			{
				lastUsedVertexArrays = usedVertexArrays;
				if(usedVertexArrays > 2048)
					Console.WriteLine("Warning: Used vertex arrays by GLGUI: {0}", usedVertexArrays);
				GC.Collect();
			}
			if(usedTextures != lastUsedTextures)
			{
				lastUsedTextures = usedTextures;
				if(usedTextures > 32)
					Console.WriteLine("Warning: Used textures by GLGUI: {0}", usedTextures);
				GC.Collect();
			}

			renderDuration = stopwatch.Elapsed.TotalMilliseconds;
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

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (currentContextMenu != null)
			{
				if (currentContextMenu.Outer.Contains(e.Position))
				{
					currentContextMenu.DoMouseDown(new MouseButtonEventArgs(e.X - currentContextMenu.Outer.X, e.Y - currentContextMenu.Outer.Y, e.Button, e.IsPressed));
					return;
				}
				else
					CloseContextMenu();
			}
			    
			DoMouseDown(e);
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (currentContextMenu != null && currentContextMenu.Outer.Contains(e.Position))
			{
				currentContextMenu.DoMouseUp(new MouseButtonEventArgs(e.X - currentContextMenu.Outer.X, e.Y - currentContextMenu.Outer.Y, e.Button, e.IsPressed));
				return;
			}

			DoMouseUp(e);
		}
	}
}