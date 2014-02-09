using System;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace GLGUI
{
    class GLFontTexture : IDisposable
    {
        int gLTexID;
        int width, height;

        public int GLTexID { get { return gLTexID; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public GLFontTexture(BitmapData dataSource)
        {
            width = dataSource.Width;
            height = dataSource.Height;

            GL.Enable(EnableCap.Texture2D);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.GenTextures(1, out gLTexID);
            GL.BindTexture(TextureTarget.Texture2D, gLTexID);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, dataSource.Scan0);

            GL.Disable(EnableCap.Texture2D);
        }

        public void Dispose()
        {
            GL.DeleteTexture(gLTexID);
        }
    }
}
