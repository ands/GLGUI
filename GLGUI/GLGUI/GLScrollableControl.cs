using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace GLGUI
{
    public class GLScrolledControl : GLControl
    {
        public Size TotalSize;

        public GLScrolledControl(GLGui gui) : base(gui)
		{
            AutoSize = true;
        }

        protected override void UpdateLayout()
        {
            outer.Width = Math.Min(Math.Max(outer.Width, sizeMin.Width), sizeMax.Width);
            outer.Height = Math.Min(Math.Max(outer.Height, sizeMin.Height), sizeMax.Height);
            Inner = new Rectangle(0, 0, outer.Width, outer.Height);

            TotalSize = new Size(0, 0);
            if (Controls.Count() > 0)
            {
                TotalSize.Width = Controls.Max(c => c.Outer.Right);
                TotalSize.Height = Controls.Max(c => c.Outer.Bottom);
            }

            if (Parent != null && !Parent.AutoSize) // avoid doubled calls if autosize is true
                Parent.Invalidate();
        }
    }

	public class GLScrollableControl : GLControl
	{
        public GLSlider Horizontal { get { return horizontal; } }
        public GLSlider Vertical { get { return vertical; } }
        public GLSkin.GLScrollableControlSkin Skin { get { return skin; } set { skin = value; Invalidate(); } }

        private GLSkin.GLScrollableControlSkin skin;
		private Point scrollPosition;
        private GLScrolledControl content;
        private GLSlider horizontal, vertical;

		public GLScrollableControl(GLGui gui) : base(gui)
		{
            Render += OnRender;
            MouseWheel += OnMouseWheel;

            skin = Gui.Skin.ScrollableControl;

			outer = new Rectangle(0, 0, 32, 32);
			horizontal = base.Add(new GLSlider(gui) { Direction = GLSliderOrientation.Horizontal/*, Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom*/ });
			vertical = base.Add(new GLSlider(gui) { Direction = GLSliderOrientation.Vertical/*, Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom*/ });
			content = base.Add(new GLScrolledControl(gui) { /*Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom*/ });
			content.MouseWheel += OnMouseWheel;

            horizontal.ValueChanged += (s, e) => Invalidate();
            vertical.ValueChanged += (s, e) => Invalidate();
		}

		protected override void UpdateLayout()
		{
			if (AutoSize)
			{
				if (Controls.Count() > 0)
				{
                    outer.Width = Controls.Max(c => c.Outer.Right) + skin.Border.Horizontal;
                    outer.Height = Controls.Max(c => c.Outer.Bottom) + skin.Border.Vertical;
				}
				else
				{
                    outer.Width = skin.Border.Horizontal;
                    outer.Height = skin.Border.Vertical;
				}
			}
			outer.Width = Math.Min(Math.Max(outer.Width, sizeMin.Width), sizeMax.Width);
			outer.Height = Math.Min(Math.Max(outer.Height, sizeMin.Height), sizeMax.Height);
            Inner = new Rectangle(skin.Border.Left, skin.Border.Top, outer.Width - skin.Border.Horizontal, outer.Height - skin.Border.Vertical);

            if (content != null)
            {
                var scrollFreedom = new Size(
                    Math.Max(content.TotalSize.Width - (Inner.Width - vertical.Outer.Width), 0),
                    Math.Max(content.TotalSize.Height - (Inner.Height - horizontal.Outer.Height), 0));

                if ((scrollFreedom.Width == 0) == horizontal.Enabled)
                    horizontal.Enabled = !horizontal.Enabled;
                if ((scrollFreedom.Height == 0) == vertical.Enabled)
                    vertical.Enabled = !vertical.Enabled;

				if ((horizontal.Parent == null) && horizontal.Enabled)
					base.Add(horizontal);
				if ((horizontal.Parent != null) && !horizontal.Enabled)
					base.Remove(horizontal);
				if ((vertical.Parent == null) && vertical.Enabled)
					base.Add(vertical);
				if ((vertical.Parent != null) && !vertical.Enabled)
					base.Remove(vertical);

                scrollPosition = new Point((int)(horizontal.Value * scrollFreedom.Width), (int)(vertical.Value * scrollFreedom.Height));
				horizontal.Outer = new Rectangle(0, Inner.Height - horizontal.Height, Inner.Width - (vertical.Enabled ? vertical.Width : 0), horizontal.Height);
				vertical.Outer = new Rectangle(Inner.Width - vertical.Width, 0, vertical.Width, Inner.Height - (horizontal.Enabled ? horizontal.Height : 0));
                horizontal.MouseWheelStep = 0.5f / scrollFreedom.Width;
                vertical.MouseWheelStep = 0.5f / scrollFreedom.Height;

                content.Outer = new Rectangle(-scrollPosition.X, -scrollPosition.Y,
					Inner.Width - (vertical.Enabled ? vertical.Width : 0) + scrollPosition.X,
					Inner.Height - (horizontal.Enabled ? horizontal.Height : 0) + scrollPosition.Y);
            }
		}

        private void OnRender(Rectangle scissorRect, double timeDelta)
        {
            GLDraw.FilledRectangle(outer.Size, skin.BorderColor);
            GLDraw.FilledRectangle(Inner, skin.BackgroundColor);
        }

        public override T Add<T>(T control)
        {
            return content.Add(control);
        }

        public override void Remove(GLControl control)
        {
            content.Remove(control);
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (vertical.Enabled)
                vertical.DoMouseWheel(e);
            else
                horizontal.DoMouseWheel(e);
        }
	}
}

