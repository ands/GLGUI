using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security;
using OpenTK;

namespace GLGUI
{
	public class GLCursor
	{
		public static GLCursor Default, SizeAll, SizeNWSE, SizeNS, SizeNESW, SizeWE, IBeam, Hand, None;

		internal readonly IntPtr Handle;

		private static bool loaded = false;
		private static bool usingSdl2 = false;

		#if REFERENCE_WINDOWS_FORMS
		private GLCursor()
		{
			if (usingSdl2)
			{
				byte[] dummy = new byte[4 * 32];
				unsafe
				{
					fixed(byte* d = dummy)
					{
						Handle = CreateCursor((IntPtr)d, (IntPtr)d, 32, 32, 0, 0);
					}
				}
			}
			else
			{
				Bitmap bmp = new Bitmap(1, 1);
				Handle = bmp.GetHicon();
			}
		}

		private GLCursor(System.Windows.Forms.Cursor cursor, bool centeredHotSpot = true)
		{
			if (!usingSdl2)
			{
				Handle = cursor.Handle;
				return;
			}

			// now it's getting ugly...
			// this is converting our shiny cursors to the minimalistic cursor support of sdl2.
			// since there seems to be NO F*CKING WAY to get the cursor hot spot from X11,
			// we just center it for every cursor except the default pointer and the hand.
			int w = Math.Max(cursor.Size.Width, 32); // also, no size...
			int h = Math.Max(cursor.Size.Height, 32); // (╯°□°）╯︵ ┻━┻
			var rect = new Rectangle(0, 0, w, h);

			var bmp = new Bitmap(w, h);
			bmp.MakeTransparent();
			using(var g = Graphics.FromImage(bmp))
			{
				cursor.Draw(g, rect);
			}
			var data = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			var color = new byte[data.Height * data.Width / 8];
			var visibility = new byte[data.Height * data.Width / 8];

			unsafe
			{
				fixed(byte* c = color, v = visibility)
				{
					Point min = new Point(int.MaxValue, int.MaxValue), max = new Point();
					byte* d = (byte*)data.Scan0;
					for(int y = 0; y < data.Height; y++)
					{
						for(int x = 0; x < data.Width; x++)
						{
							int a = d[y * data.Stride + x * 4 + 3];
							int l = d[y * data.Stride + x * 4 + 0] + d[y * data.Stride + x * 4 + 1] + d[y * data.Stride + x * 4 + 2];
							if (a > 100)
							{
								min.X = Math.Min(min.X, x); min.Y = Math.Min(min.Y, y);
								max.X = Math.Max(max.X, x); max.Y = Math.Max(max.Y, y);

								v[(y * data.Width + x) / 8] |= (byte)(1 << (7 - (x & 7)));
								if (l < 300)
									c[(y * data.Width + x) / 8] |= (byte)(1 << (7 - (x & 7)));
							}
						}
					}
					Point hotspot = centeredHotSpot ? new Point((min.X + max.X) / 2, (min.Y + max.Y) / 2) : min;
					Handle = CreateCursor((IntPtr)c, (IntPtr)v, data.Width, data.Height, hotspot.X, hotspot.Y);
				}
			}
			bmp.UnlockBits(data);
		}
		#endif

		internal static void LoadCursors(NativeWindow window)
		{
			if (loaded)
				return;

			string infoType = window.WindowInfo.GetType().Name;
			if (infoType == "Sdl2WindowInfo")
				usingSdl2 = true;

			#if REFERENCE_WINDOWS_FORMS
			Default = new GLCursor(System.Windows.Forms.Cursors.Default, false);
			SizeAll = new GLCursor(System.Windows.Forms.Cursors.SizeAll);
			SizeNWSE = new GLCursor(System.Windows.Forms.Cursors.SizeNWSE);
			SizeNS = new GLCursor(System.Windows.Forms.Cursors.SizeNS);
			SizeNESW = new GLCursor(System.Windows.Forms.Cursors.SizeNESW);
			SizeWE = new GLCursor(System.Windows.Forms.Cursors.SizeWE);
			IBeam = new GLCursor(System.Windows.Forms.Cursors.IBeam);
			Hand = new GLCursor(System.Windows.Forms.Cursors.Hand, false);
			None = new GLCursor();
			#endif

			loaded = true;
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_CreateCursor", ExactSpelling = true)]
		public static extern IntPtr CreateCursor(IntPtr data, IntPtr mask, int w, int h, int hot_x, int hot_y);
	}
}

