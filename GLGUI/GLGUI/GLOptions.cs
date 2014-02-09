using System;
using System.Drawing;
using System.Linq;

namespace GLGUI
{
	public class GLOptions : GLFlowLayout
	{
        public GLCheckBox Selection { get { return selection; } set { if (Controls.Contains(value)) Select(value, EventArgs.Empty); } }

        public event EventHandler Changed;

        private GLCheckBox selection;

		public GLOptions(GLGui gui) : base(gui)
		{
			outer = new Rectangle(0, 0, 0, 0);
		}

		public override T Add<T>(T control)
		{
            if (!(control is GLCheckBox))
                throw new InvalidOperationException("only GLCheckBoxes are allowed on GLOptions instances");
			base.Add(control);
            (control as GLCheckBox).Changed += Select;
			return control;
		}

		public override void Remove(GLControl control)
		{
			base.Remove(control);
            (control as GLCheckBox).Changed -= Select;
		}

        private void Select(object sender, EventArgs e)
        {
            if(selection != null)
                selection.Checked = false;

            GLCheckBox senderCheckBox = (GLCheckBox)sender;
            selection = senderCheckBox;
            senderCheckBox.Checked = true;

            if(Changed != null)
                Changed(sender, e);
        }
	}
}

