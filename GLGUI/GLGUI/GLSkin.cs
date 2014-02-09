using GLGUI;
using System.Windows.Forms;
using System.Drawing;

namespace GLGUI
{
	public class GLSkin
	{
		public struct GLFormSkin
		{
			public GLFont Font;
            public Color Color;
			public Padding Border;
			public Color BorderColor;
			public Color BackgroundColor;
		}

		public struct GLButtonSkin
		{
			public GLFont Font;
            public Color Color;
			public Padding Border;
			public Color BorderColor;
			public Color BackgroundColor;
		}

		public struct GLLabelSkin
		{
			public GLFont Font;
            public Color Color;
            public Padding Padding;
			public Color BackgroundColor;
		}

		public struct GLLinkLabelSkin
		{
			public GLFont Font;
			public Color Color;
			public Padding Padding;
			public Color BackgroundColor;
		}

		public struct GLTextBoxSkin
		{
			public GLFont Font;
            public Color Color;
			public Padding Border;
			public Padding Padding;
			public Color BorderColor;
			public Color BackgroundColor;
		}

		public struct GLCheckBoxSkin
		{
			public GLFont Font;
            public Color Color;
			public Padding Border;
			public Color BorderColor;
			public Color BackgroundColor;
		}

        public struct GLGroupLayoutSkin
        {
            public Padding Border;
            public Color BorderColor;
            public Color BackgroundColor;
        }

		public struct GLFlowLayoutSkin
		{
			public Padding Padding;
			public Padding Border;
			public Color BorderColor;
			public Color BackgroundColor;
			public int Space;
		}

		public struct GLSplitLayoutSkin
		{
			public Color BackgroundColor;
			public int SplitterSize;
		}

        public struct GLSliderSkin
        {
            public Color KnobColor;
            public Color BackgroundColor;
        }

        public struct GLScrollableControlSkin
        {
            public Padding Border;
            public Color BorderColor;
            public Color BackgroundColor;
        }

		public GLFormSkin FormActive = new GLFormSkin();
		public GLFormSkin FormInactive = new GLFormSkin();

		public GLButtonSkin ButtonEnabled = new GLButtonSkin();
		public GLButtonSkin ButtonDisabled = new GLButtonSkin();
		public GLButtonSkin ButtonHover = new GLButtonSkin();
		public GLButtonSkin ButtonPressed = new GLButtonSkin();

		public GLLabelSkin LabelEnabled = new GLLabelSkin();
		public GLLabelSkin LabelDisabled = new GLLabelSkin();

		public GLLinkLabelSkin LinkLabelEnabled = new GLLinkLabelSkin();
		public GLLinkLabelSkin LinkLabelDisabled = new GLLinkLabelSkin();

		public GLTextBoxSkin TextBoxEnabled = new GLTextBoxSkin();
		public GLTextBoxSkin TextBoxActive = new GLTextBoxSkin();
		public GLTextBoxSkin TextBoxHover = new GLTextBoxSkin();
		public GLTextBoxSkin TextBoxDisabled = new GLTextBoxSkin();

		public GLCheckBoxSkin CheckBoxEnabled = new GLCheckBoxSkin();
		public GLCheckBoxSkin CheckBoxPressed = new GLCheckBoxSkin();
		public GLCheckBoxSkin CheckBoxHover = new GLCheckBoxSkin();
		public GLCheckBoxSkin CheckBoxDisabled = new GLCheckBoxSkin();

        public GLGroupLayoutSkin GroupLayout = new GLGroupLayoutSkin();

		public GLFlowLayoutSkin FlowLayout = new GLFlowLayoutSkin();

		public GLSplitLayoutSkin SplitLayout = new GLSplitLayoutSkin();

        public GLSliderSkin SliderEnabled = new GLSliderSkin();
        public GLSliderSkin SliderDisabled = new GLSliderSkin();
        public GLSliderSkin SliderHover = new GLSliderSkin();
        public GLSliderSkin SliderPressed = new GLSliderSkin();

        public GLScrollableControlSkin ScrollableControl = new GLScrollableControlSkin();

		public GLFlowLayoutSkin ContextMenu = new GLFlowLayoutSkin();

		public GLButtonSkin ContextMenuEntryEnabled = new GLButtonSkin();
		public GLButtonSkin ContextMenuEntryDisabled = new GLButtonSkin();
		public GLButtonSkin ContextMenuEntryHover = new GLButtonSkin();
		public GLButtonSkin ContextMenuEntryPressed = new GLButtonSkin();


		public GLFont DefaultFont;

		public GLSkin()
		{
			DefaultFont = new GLFont(new Font("Arial", 8.0f));

			FormActive.Font = DefaultFont;
			FormActive.Color = Color.FromArgb(240, 240, 240);
			FormActive.Border = new Padding(2);
			FormActive.BorderColor = Color.FromArgb(192, 96, 96, 96);
			FormActive.BackgroundColor = Color.FromArgb(240, 240, 240);

			FormInactive.Font = DefaultFont;
			FormInactive.Color = Color.FromArgb(160, 160, 160);
			FormInactive.Border = new Padding(2);
            FormInactive.BorderColor = Color.FromArgb(192, 96, 96, 96);
			FormInactive.BackgroundColor = Color.FromArgb(240, 240, 240);


            ButtonEnabled.Font = DefaultFont;
            ButtonEnabled.Color = Color.FromArgb(240, 240, 240);
			ButtonEnabled.Border = new Padding(2);
			ButtonEnabled.BorderColor = Color.FromArgb(96, 96, 96);
			ButtonEnabled.BackgroundColor = Color.Transparent;

            ButtonDisabled.Font = DefaultFont;
            ButtonDisabled.Color = Color.FromArgb(128, 128, 128);
			ButtonDisabled.Border = new Padding(2);
			ButtonDisabled.BorderColor = Color.FromArgb(96, 96, 96);
			ButtonDisabled.BackgroundColor = Color.Transparent;

            ButtonHover.Font = DefaultFont;
            ButtonHover.Color = Color.FromArgb(255, 255, 255);
			ButtonHover.Border = new Padding(2);
			ButtonHover.BorderColor = Color.FromArgb(128, 128, 128);
			ButtonHover.BackgroundColor = Color.Transparent;

            ButtonPressed.Font = DefaultFont;
            ButtonPressed.Color = Color.FromArgb(96, 96, 96);
			ButtonPressed.Border = new Padding(2);
			ButtonPressed.BorderColor = Color.FromArgb(128, 128, 128);
			ButtonPressed.BackgroundColor = Color.Transparent;


            LabelEnabled.Font = DefaultFont;
            LabelEnabled.Color = Color.FromArgb(96, 96, 96);
			LabelEnabled.Padding = new Padding(1, 1, 1, 1);
			LabelEnabled.BackgroundColor = Color.Transparent;

            LabelDisabled.Font = DefaultFont;
            LabelDisabled.Color = Color.FromArgb(128, 128, 128);
			LabelDisabled.Padding = new Padding(1, 1, 1, 1);
			LabelDisabled.BackgroundColor = Color.Transparent;


			LinkLabelEnabled.Font = DefaultFont;
			LinkLabelEnabled.Color = Color.FromArgb(0, 0, 255);
			LinkLabelEnabled.Padding = new Padding(1, 1, 1, 1);
			LinkLabelEnabled.BackgroundColor = Color.Transparent;

			LinkLabelDisabled.Font = DefaultFont;
			LinkLabelDisabled.Color = Color.FromArgb(96, 96, 192);
			LinkLabelDisabled.Padding = new Padding(1, 1, 1, 1);
			LinkLabelDisabled.BackgroundColor = Color.Transparent;


            TextBoxEnabled.Font = DefaultFont;
            TextBoxEnabled.Color = Color.FromArgb(96, 96, 96);
			TextBoxEnabled.Border = new Padding(1);
			TextBoxEnabled.Padding = new Padding(1, 0, 1, 2);
			TextBoxEnabled.BorderColor = Color.FromArgb(96, 96, 96);
			TextBoxEnabled.BackgroundColor = Color.FromArgb(255, 255, 255);

            TextBoxActive.Font = DefaultFont;
            TextBoxActive.Color = Color.FromArgb(96, 96, 96);
			TextBoxActive.Border = new Padding(1);
			TextBoxActive.Padding = new Padding(1, 0, 1, 2);
			TextBoxActive.BorderColor = Color.FromArgb(255, 192, 96);
			TextBoxActive.BackgroundColor = Color.FromArgb(255, 255, 255);

            TextBoxHover.Font = DefaultFont;
            TextBoxHover.Color = Color.FromArgb(96, 96, 96);
			TextBoxHover.Border = new Padding(1);
			TextBoxHover.Padding = new Padding(1, 0, 1, 2);
			TextBoxHover.BorderColor = Color.FromArgb(128, 128, 128);
			TextBoxHover.BackgroundColor = Color.FromArgb(255, 255, 255);

            TextBoxDisabled.Font = DefaultFont;
            TextBoxDisabled.Color = Color.FromArgb(128, 128, 128);
			TextBoxDisabled.Border = new Padding(1);
			TextBoxDisabled.Padding = new Padding(1, 0, 1, 2);
			TextBoxDisabled.BorderColor = Color.FromArgb(128, 128, 128);
			TextBoxDisabled.BackgroundColor = Color.FromArgb(192, 192, 192);


            CheckBoxEnabled.Font = DefaultFont;
            CheckBoxEnabled.Color = Color.FromArgb(96, 96, 96);
			CheckBoxEnabled.Border = new Padding(1);
			CheckBoxEnabled.BorderColor = Color.FromArgb(96, 96, 96);
			CheckBoxEnabled.BackgroundColor = Color.FromArgb(255, 255, 255);

            CheckBoxPressed.Font = DefaultFont;
            CheckBoxPressed.Color = Color.FromArgb(96, 96, 96);
			CheckBoxPressed.Border = new Padding(1);
			CheckBoxPressed.BorderColor = Color.FromArgb(255, 192, 96);
			CheckBoxPressed.BackgroundColor = Color.FromArgb(255, 255, 255);

            CheckBoxHover.Font = DefaultFont;
            CheckBoxHover.Color = Color.FromArgb(96, 96, 96);
			CheckBoxHover.Border = new Padding(1);
			CheckBoxHover.BorderColor = Color.FromArgb(128, 128, 128);
			CheckBoxHover.BackgroundColor = Color.FromArgb(255, 255, 255);

            CheckBoxDisabled.Font = DefaultFont;
            CheckBoxDisabled.Color = Color.FromArgb(128, 128, 128);
			CheckBoxDisabled.Border = new Padding(1);
			CheckBoxDisabled.BorderColor = Color.FromArgb(128, 128, 128);
			CheckBoxDisabled.BackgroundColor = Color.FromArgb(192, 192, 192);


			GroupLayout.Border = new Padding(2);
            GroupLayout.BorderColor = Color.FromArgb(96, 96, 96);
            GroupLayout.BackgroundColor = Color.FromArgb(240, 240, 240);


			FlowLayout.Padding = new Padding(2);
			FlowLayout.Border = new Padding(0);
			FlowLayout.BorderColor = Color.Transparent;
			FlowLayout.BackgroundColor = Color.Transparent;
			FlowLayout.Space = 2;


			SplitLayout.BackgroundColor = Color.FromArgb(192, 192, 192);
			SplitLayout.SplitterSize = 2;


            SliderEnabled.KnobColor = Color.FromArgb(96, 96, 96);
            SliderEnabled.BackgroundColor = Color.FromArgb(128, 128, 128);

            SliderDisabled.KnobColor = Color.Transparent; //Color.FromArgb(96, 96, 96);
            SliderDisabled.BackgroundColor = Color.FromArgb(128, 128, 128);

            SliderHover.KnobColor = Color.FromArgb(96, 96, 96);
            SliderHover.BackgroundColor = Color.FromArgb(144, 144, 144);

            SliderPressed.KnobColor = Color.FromArgb(80, 80, 80);
            SliderPressed.BackgroundColor = Color.FromArgb(144, 144, 144);


			ScrollableControl.Border = new Padding(2);
            ScrollableControl.BorderColor = Color.FromArgb(96, 96, 96);
            ScrollableControl.BackgroundColor = Color.FromArgb(240, 240, 240);


			ContextMenu.Padding = new Padding(1);
			ContextMenu.Border = new Padding(1);
			ContextMenu.BorderColor = Color.FromArgb(96, 96, 96);
			ContextMenu.BackgroundColor = Color.FromArgb(240, 240, 240);
			ContextMenu.Space = 1;


			ContextMenuEntryEnabled.Font = DefaultFont;
			ContextMenuEntryEnabled.Color = Color.FromArgb(240, 240, 240);
			ContextMenuEntryEnabled.Border = new Padding(3);
			ContextMenuEntryEnabled.BorderColor = Color.FromArgb(96, 96, 96);
			ContextMenuEntryEnabled.BackgroundColor = Color.Transparent;

			ContextMenuEntryDisabled.Font = DefaultFont;
			ContextMenuEntryDisabled.Color = Color.FromArgb(128, 128, 128);
			ContextMenuEntryDisabled.Border = new Padding(3);
			ContextMenuEntryDisabled.BorderColor = Color.FromArgb(96, 96, 96);
			ContextMenuEntryDisabled.BackgroundColor = Color.Transparent;

			ContextMenuEntryHover.Font = DefaultFont;
			ContextMenuEntryHover.Color = Color.FromArgb(255, 255, 255);
			ContextMenuEntryHover.Border = new Padding(3);
			ContextMenuEntryHover.BorderColor = Color.FromArgb(128, 128, 128);
			ContextMenuEntryHover.BackgroundColor = Color.Transparent;

			ContextMenuEntryPressed.Font = DefaultFont;
			ContextMenuEntryPressed.Color = Color.FromArgb(96, 96, 96);
			ContextMenuEntryPressed.Border = new Padding(3);
			ContextMenuEntryPressed.BorderColor = Color.FromArgb(128, 128, 128);
			ContextMenuEntryPressed.BackgroundColor = Color.Transparent;
		}
	}
}

