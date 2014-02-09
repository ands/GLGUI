using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;

namespace GLGUI.DataView
{
	public class WorldRenderer : GLViewport
	{
		float MouseSensitivity = 0.005f;
		float MovementSpeed = 2.0f;

		GLFont worldFont;
		GLFontText worldText;
		double time = 0.0;
		bool down = false;
		Point startMouseDrag;

		Vector3 position;
		int[] movement = new int[6]; // WASDQE
		Vector3 eulerRotation;
		Vector3 startEulerRotationDrag;

		public WorldRenderer(GLGui gui) : base(gui)
		{
			Render += OnRender;
			MouseMove += OnMouseMove;
			MouseDown += OnMouseDown;
			MouseUp += OnMouseUp;
			KeyDown += OnKeyDown;
			KeyUp += OnKeyUp;

			GL.ClearColor(Color.Black);

			worldFont = new GLFont(new Font(FontFamily.GenericSansSerif, 64.0f));
			worldText = new GLFontText();
			worldFont.ProcessText(worldText, "Hello World", GLFontAlignment.Centre);

			eulerRotation = new Vector3();
			position = new Vector3(0, 0, 5);
		}

		private void OnRender(GLViewport source, double timeDelta)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.MatrixMode(MatrixMode.Projection);
			var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), source.AspectRatio, 0.1f, 100.0f);
			GL.LoadMatrix(ref proj);
			GL.MatrixMode(MatrixMode.Modelview);
			var view = Matrix4.CreateRotationX(eulerRotation.X) * Matrix4.CreateRotationY(eulerRotation.Y) * Matrix4.CreateTranslation(position);
			Vector3 movementVec = new Vector3();
			movementVec.X = (movement[3] - movement[1]) * (float)timeDelta * MovementSpeed;
			movementVec.Y = (movement[4] - movement[5]) * (float)timeDelta * MovementSpeed;
			movementVec.Z = (movement[2] - movement[0]) * (float)timeDelta * MovementSpeed;
			position += Vector3.TransformVector(movementVec, view);
			view.Invert();
			GL.LoadMatrix(ref view);

			time += timeDelta;

			GL.Enable(EnableCap.DepthTest);

			GL.PushMatrix();
			GL.Rotate(32.0f * time, 0.0f, 1.0f, 0.0f);

			GL.Begin(PrimitiveType.Quads);
			GL.Color3(1.0, 0.0, 0.0);
			GL.Vertex3(0.5, -0.5, -0.5);
			GL.Color3(0.0, 1.0, 0.0);
			GL.Vertex3(0.5, 0.5, -0.5);
			GL.Color3(0.0, 0.0, 1.0);
			GL.Vertex3(-0.5, 0.5, -0.5);
			GL.Color3(1.0, 0.0, 1.0);
			GL.Vertex3(-0.5, -0.5, -0.5);

			GL.Color3(1.0, 1.0, 1.0);
			GL.Vertex3(0.5, -0.5, 0.5);
			GL.Vertex3(0.5, 0.5, 0.5);
			GL.Vertex3(-0.5, 0.5, 0.5);
			GL.Vertex3(-0.5, -0.5, 0.5);

			GL.Color3(1.0, 0.0, 1.0);
			GL.Vertex3(0.5, -0.5, -0.5);
			GL.Vertex3(0.5, 0.5, -0.5);
			GL.Vertex3(0.5, 0.5, 0.5);
			GL.Vertex3(0.5, -0.5, 0.5);

			GL.Color3(0.0, 1.0, 0.0);
			GL.Vertex3(-0.5, -0.5, 0.5);
			GL.Vertex3(-0.5, 0.5, 0.5);
			GL.Vertex3(-0.5, 0.5, -0.5);
			GL.Vertex3(-0.5, -0.5, -0.5);

			GL.Color3(0.0, 0.0, 1.0);
			GL.Vertex3(0.5, 0.5, 0.5);
			GL.Vertex3(0.5, 0.5, -0.5);
			GL.Vertex3(-0.5, 0.5, -0.5);
			GL.Vertex3(-0.5, 0.5, 0.5);

			GL.Color3(1.0, 0.0, 0.0);
			GL.Vertex3(0.5, -0.5, -0.5);
			GL.Vertex3(0.5, -0.5, 0.5);
			GL.Vertex3(-0.5, -0.5, 0.5);
			GL.Vertex3(-0.5, -0.5, -0.5);
			GL.End();
			GL.PopMatrix();

			GL.DepthMask(false);
			GL.Scale(0.01f, -0.01f, 0.01f);
			worldFont.Print(worldText, new Vector2(0, 100), Color.White);
			GL.DepthMask(true);

			GL.Disable(EnableCap.DepthTest);
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (down)
			{
				eulerRotation.X = startEulerRotationDrag.X - (e.Location.Y - startMouseDrag.Y) * MouseSensitivity;
				eulerRotation.X = Math.Min(Math.Max(eulerRotation.X, (float)-Math.PI), (float)Math.PI);
				eulerRotation.Y = startEulerRotationDrag.Y - (e.Location.X - startMouseDrag.X) * MouseSensitivity;
			}
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				isDragged = true;
				down = true;
				startMouseDrag = e.Location;
				startEulerRotationDrag = eulerRotation;
			}
		}

		private void OnMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				isDragged = false;
				down = false;
			}
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.W)
				movement[0] = 1;
			if (e.KeyCode == Keys.A)
				movement[1] = 1;
			if (e.KeyCode == Keys.S)
				movement[2] = 1;
			if (e.KeyCode == Keys.D)
				movement[3] = 1;
			if (e.KeyCode == Keys.Q)
				movement[4] = 1;
			if (e.KeyCode == Keys.E)
				movement[5] = 1;
		}

		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.W)
				movement[0] = 0;
			if (e.KeyCode == Keys.A)
				movement[1] = 0;
			if (e.KeyCode == Keys.S)
				movement[2] = 0;
			if (e.KeyCode == Keys.D)
				movement[3] = 0;
			if (e.KeyCode == Keys.Q)
				movement[4] = 0;
			if (e.KeyCode == Keys.E)
				movement[5] = 0;
		}
	}
}

