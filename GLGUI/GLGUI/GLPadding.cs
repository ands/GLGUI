namespace GLGUI
{
	public struct GLPadding
	{
		public int Left, Top, Right, Bottom;

		public int Horizontal { get { return Left + Right; } }
		public int Vertical { get { return Top + Bottom; } }

		public GLPadding(int all)
		{
			Left = Top = Right = Bottom = all;
		}
		public GLPadding(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}
	}
}
