using System;
using System.Drawing;
using System.Linq;

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

        protected override void UpdateLayout()
        {
            if (Controls.Count() > 0)
            {
                int maxWidth = 0;
                foreach (var entry in Controls)
                {
                    entry.AutoSize = true;
                    maxWidth = Math.Max(maxWidth, entry.Width);
                }
                foreach (var entry in Controls)
                {
                    entry.AutoSize = false;
                    entry.Size = new Size(maxWidth, entry.Height);
                }
            }

            base.UpdateLayout();
        }
	}
}
