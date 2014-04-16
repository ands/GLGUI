using System.Windows.Forms;

namespace GLGUI.GLControlExample
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            Text = "GLGUI.GLControlExample";
            Size = new System.Drawing.Size(1024, 600);

            var guiControl = new GuiControl();
            guiControl.Dock = DockStyle.Fill;
            Controls.Add(guiControl);
        }
    }
}
