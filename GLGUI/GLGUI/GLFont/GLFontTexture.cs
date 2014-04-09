using System;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace GLGUI
{
    class GLFontTexture : IDisposable
    {
		public readonly int TextureID;
		public readonly int Width;
		public readonly int Height;

        public GLFontTexture(BitmapData dataSource)
        {
			Width = dataSource.Width;
			Height = dataSource.Height;

            GL.Enable(EnableCap.Texture2D);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out TextureID);
            GL.BindTexture(TextureTarget.Texture2D, TextureID);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, dataSource.Scan0);

            GL.Disable(EnableCap.Texture2D);

			GLGui.usedTextures++;
        }

        public void Dispose()
        {
            GL.DeleteTexture(TextureID);
			GLGui.usedTextures--;
        }

		~GLFontTexture()
		{
			lock(GLGui.toDispose)
			{
				GLGui.toDispose.Add(this);
			}
		}
    }
}
