namespace GLGUI
{
	public class GLContextMenu : GLFlowLayout
	{
		public GLContextMenu(GLGui gui) : base(gui)
		{
			FlowDirection = GLFlowDirection.TopDown;
			AutoSize = true;

			Skin = Gui.Skin.ContextMenu;
		}
	}
}
