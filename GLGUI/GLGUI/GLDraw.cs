using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Graphics;
using OpenTK;

namespace GLGUI
{
	public static class GLDraw
	{
		public static void Line(int x0, int y0, int x1, int y1, Color4 color)
		{
			if (color == Color.Transparent)
				return;
			GL.Color4(color);
			GL.Begin(PrimitiveType.Lines);
			GL.Vertex2(x0, y0);
			GL.Vertex2(x1, y1);
			GL.End();
		}

		public static void Line(Point start, Point end, Color4 color)
		{
			if (color == Color.Transparent)
				return;
			GL.Color4(color);
			GL.Begin(PrimitiveType.Lines);
			GL.Vertex2(start.X, start.Y);
			GL.Vertex2(end.X, end.Y);
			GL.End();
		}

		public static void FilledRectangle(Size size, Color4 color)
		{
			if (color == Color.Transparent)
				return;
			const float r = 4.0f;
			GL.Color4(color);
			GL.Begin(PrimitiveType.TriangleFan);
			RoundCorner(r, r, r, 0);
			RoundCorner(r, size.Height - r, r, 1);
			RoundCorner(size.Width - r, size.Height - r, r, 2);
			RoundCorner(size.Width - r, r, r, 3);
			/*GL.Vertex2(0, 0);
			GL.Vertex2(0, size.Height);
			GL.Vertex2(size.Width, size.Height);
			GL.Vertex2(size.Width, 0);*/
			GL.End();
		}

		public static void FilledRectangle(Rectangle rect, Color4 color)
		{
			if (color == Color.Transparent)
				return;
			const float r = 3.0f;
			GL.Color4(color);
			GL.Begin(PrimitiveType.TriangleFan);
			RoundCorner(rect.Left + r, rect.Top + r, r, 0);
			RoundCorner(rect.Left + r, rect.Bottom - r, r, 1);
			RoundCorner(rect.Right - r, rect.Bottom - r, r, 2);
			RoundCorner(rect.Right - r, rect.Top + r, r, 3);
			/*GL.Vertex2(rect.Left, rect.Top);
			GL.Vertex2(rect.Left, rect.Bottom);
			GL.Vertex2(rect.Right, rect.Bottom);
			GL.Vertex2(rect.Right, rect.Top);*/
			GL.End();
		}


        private static Vector2[] corners;
		private static void RoundCorner(float x, float y, float r, int a)
		{
			const int edges = 5;
            if (corners == null)
            {
                corners = new Vector2[edges * 4 + 1];
                for (int i = 0; i <= edges * 4; i++)
                {
                    float t = (i + edges) * 0.5f * (float)Math.PI / (float)edges;
                    corners[i] = new Vector2((float)Math.Cos(t), (float)Math.Sin(t));
                }
            }

            a *= edges;
            for (int i = a; i <= a + edges; i++)
                GL.Vertex2(x + r * corners[i].X, y - r * corners[i].Y);
		}
	}
}

