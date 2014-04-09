using System;
using OpenTK.Graphics.OpenGL;

namespace GLGUI
{
	public class GLFontVertexBuffer : IDisposable
    {
		public int TextureID { get { return textureID; } internal set { textureID = value; } }
		public int VaoID { get { return vaoID; } }
		public int VertexCount { get { return vertexCount; } }

		int textureID;
		int vaoID, vboID;
        float[] vertices = new float[1024];
        int floatCount = 0;
        int vertexCount = 0;

        public GLFontVertexBuffer(int textureID)
        {
			this.textureID = textureID;
			vaoID = GL.GenVertexArray();
			vboID = GL.GenBuffer();
			GLGui.usedVertexArrays++;
        }

		~GLFontVertexBuffer()
		{
			lock(GLGui.toDispose)
			{
				GLGui.toDispose.Add(this);
			}
		}

        public void Dispose()
        {
			GL.DeleteVertexArray(vaoID);
			GL.DeleteBuffer(vboID);
			GLGui.usedVertexArrays--;
        }

        public void Reset()
        {
            floatCount = 0;
            vertexCount = 0;
        }

        public void AddQuad(float minx, float miny, float maxx, float maxy, float mintx, float minty, float maxtx, float maxty)
        {
            if (floatCount + 16 >= vertices.Length)
                Array.Resize(ref vertices, vertices.Length * 2);

            vertices[floatCount + 0] = minx;
            vertices[floatCount + 1] = miny;
            vertices[floatCount + 2] = mintx;
            vertices[floatCount + 3] = minty;

            vertices[floatCount + 4] = minx;
            vertices[floatCount + 5] = maxy;
            vertices[floatCount + 6] = mintx;
            vertices[floatCount + 7] = maxty;

            vertices[floatCount + 8] = maxx;
            vertices[floatCount + 9] = maxy;
            vertices[floatCount + 10] = maxtx;
            vertices[floatCount + 11] = maxty;

            vertices[floatCount + 12] = maxx;
            vertices[floatCount + 13] = miny;
            vertices[floatCount + 14] = maxtx;
            vertices[floatCount + 15] = minty;
            floatCount += 16;
            vertexCount += 4;
        }

        public void Load()
        {
            if (floatCount == 0)
                return;

			GL.BindVertexArray(vaoID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(floatCount * 4), vertices, BufferUsageHint.StaticDraw);

			GL.VertexPointer(2, VertexPointerType.Float, 16, 0);
			GL.EnableClientState(ArrayCap.VertexArray);
			GL.TexCoordPointer(2, TexCoordPointerType.Float, 16, 8);
			GL.EnableClientState(ArrayCap.TextureCoordArray);
			GL.BindVertexArray(0);
        }

        public void Draw()
        {
            if (vertexCount == 0)
                return;

            GL.Enable(EnableCap.Texture2D);
			GL.BindTexture(TextureTarget.Texture2D, textureID);

			GL.BindVertexArray(vaoID);
			GL.DrawArrays(PrimitiveType.Quads, 0, vertexCount);
			GL.BindVertexArray(0);

            GL.Disable(EnableCap.Texture2D);
        }
    }
}
