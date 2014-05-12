namespace GLGUI
{
	public class GLContextMenuEntry : GLButton
	{
		public GLContextMenuEntry(GLGui gui) : base(gui)
		{
			SkinEnabled = Gui.Skin.ContextMenuEntryEnabled;
			SkinPressed = Gui.Skin.ContextMenuEntryPressed;
			SkinHover = Gui.Skin.ContextMenuEntryHover;
			SkinDisabled = Gui.Skin.ContextMenuEntryDisabled;

			Click += (s, e) => Gui.CloseContextMenu();
		}
	}
}
