using System;
using System.Drawing;
using System.Windows.Forms;

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

	public class GLContextMenu : GLFlowLayout
	{
		public GLContextMenu(GLGui gui) : base(gui)
		{
			FlowDirection = FlowDirection.TopDown;
			AutoSize = true;

			Skin = Gui.Skin.ContextMenu;
		}
	}
}

