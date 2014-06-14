using System.Drawing;
using OpenTK.Graphics;

namespace GLGUI
{
	public class GLSkin
	{
		public struct GLFormSkin
		{
			public GLFont Font;
            public Color4 Color;
			public GLPadding Border;
			public Color4 BorderColor;
			public Color4 BackgroundColor;
		}

		public struct GLButtonSkin
		{
			public GLFont Font;
            public Color4 Color;
            public GLFontAlignment TextAlign;
			public GLPadding Border;
			public Color4 BorderColor;
			public Color4 BackgroundColor;
		}

		public struct GLLabelSkin
		{
			public GLFont Font;
            public Color4 Color;
            public GLFontAlignment TextAlign;
            public GLPadding Padding;
			public Color4 BackgroundColor;
		}

		public struct GLTextBoxSkin
		{
			public GLFont Font;
            public Color4 Color;
            public Color4 SelectionColor;
            public GLFontAlignment TextAlign;
			public GLPadding Border;
			public GLPadding Padding;
			public Color4 BorderColor;
			public Color4 BackgroundColor;
		}

		public struct GLCheckBoxSkin
		{
			public GLFont Font;
            public Color4 Color;
			public GLPadding Border;
			public Color4 BorderColor;
			public Color4 BackgroundColor;
		}

        public struct GLGroupLayoutSkin
        {
            public GLPadding Border;
            public Color4 BorderColor;
            public Color4 BackgroundColor;
        }

		public struct GLFlowLayoutSkin
		{
			public GLPadding Padding;
			public GLPadding Border;
			public Color4 BorderColor;
			public Color4 BackgroundColor;
			public int Space;
		}

		public struct GLSplitLayoutSkin
		{
			public Color4 BackgroundColor;
			public int SplitterSize;
		}

        public struct GLSliderSkin
        {
            public Color4 KnobColor;
            public Color4 BackgroundColor;
        }

        public struct GLScrollableControlSkin
        {
            public GLPadding Border;
            public Color4 BorderColor;
            public Color4 BackgroundColor;
        }

		public GLFormSkin FormActive = new GLFormSkin();
		public GLFormSkin FormInactive = new GLFormSkin();

		public GLButtonSkin ButtonEnabled = new GLButtonSkin();
		public GLButtonSkin ButtonDisabled = new GLButtonSkin();
		public GLButtonSkin ButtonHover = new GLButtonSkin();
		public GLButtonSkin ButtonPressed = new GLButtonSkin();

		public GLLabelSkin LabelEnabled = new GLLabelSkin();
		public GLLabelSkin LabelDisabled = new GLLabelSkin();

		public GLLabelSkin LinkLabelEnabled = new GLLabelSkin();
		public GLLabelSkin LinkLabelDisabled = new GLLabelSkin();

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

		public GLSkin(GLFont defaultFont = null)
		{
			if(defaultFont == null)
				defaultFont = new GLFont(new Font("Arial", 8.0f));

			FormActive.Font = defaultFont;
			FormActive.Color = Color.FromArgb(240, 240, 240);
			FormActive.Border = new GLPadding(2);
			FormActive.BorderColor = Color.FromArgb(192, 56, 56, 56);
			FormActive.BackgroundColor = Color.FromArgb(41, 41, 41);

			FormInactive.Font = defaultFont;
			FormInactive.Color = Color.FromArgb(160, 160, 160);
			FormInactive.Border = new GLPadding(2);
			FormInactive.BorderColor = Color.FromArgb(192, 56, 56, 56);
			FormInactive.BackgroundColor = Color.FromArgb(41, 41, 41);


			ButtonEnabled.Font = defaultFont;
			ButtonEnabled.Color = Color.FromArgb(240, 240, 240);
            ButtonEnabled.TextAlign = GLFontAlignment.Centre;
			ButtonEnabled.Border = new GLPadding(2);
			ButtonEnabled.BorderColor = Color.FromArgb(56, 56, 56);
			ButtonEnabled.BackgroundColor = Color.Transparent;

			ButtonDisabled.Font = defaultFont;
			ButtonDisabled.Color = Color.FromArgb(128, 128, 128);
            ButtonDisabled.TextAlign = GLFontAlignment.Centre;
			ButtonDisabled.Border = new GLPadding(2);
			ButtonDisabled.BorderColor = Color.FromArgb(56, 56, 56);
			ButtonDisabled.BackgroundColor = Color.Transparent;

			ButtonHover.Font = defaultFont;
			ButtonHover.Color = Color.FromArgb(255, 255, 255);
            ButtonHover.TextAlign = GLFontAlignment.Centre;
			ButtonHover.Border = new GLPadding(2);
			ButtonHover.BorderColor = Color.FromArgb(64, 64, 64);
			ButtonHover.BackgroundColor = Color.Transparent;

			ButtonPressed.Font = defaultFont;
			ButtonPressed.Color = Color.FromArgb(96, 96, 96);
            ButtonPressed.TextAlign = GLFontAlignment.Centre;
			ButtonPressed.Border = new GLPadding(2);
			ButtonPressed.BorderColor = Color.FromArgb(32, 32, 32);
			ButtonPressed.BackgroundColor = Color.Transparent;


			LabelEnabled.Font = defaultFont;
			LabelEnabled.Color = Color.FromArgb(192, 192, 192);
            LabelEnabled.TextAlign = GLFontAlignment.Left;
			LabelEnabled.Padding = new GLPadding(1, 1, 1, 1);
			LabelEnabled.BackgroundColor = Color.Transparent;

			LabelDisabled.Font = defaultFont;
			LabelDisabled.Color = Color.FromArgb(128, 128, 128);
            LabelDisabled.TextAlign = GLFontAlignment.Left;
			LabelDisabled.Padding = new GLPadding(1, 1, 1, 1);
			LabelDisabled.BackgroundColor = Color.Transparent;


			LinkLabelEnabled.Font = defaultFont;
			LinkLabelEnabled.Color = Color.FromArgb(128, 128, 255);
            LinkLabelEnabled.TextAlign = GLFontAlignment.Left;
			LinkLabelEnabled.Padding = new GLPadding(1, 1, 1, 1);
			LinkLabelEnabled.BackgroundColor = Color.Transparent;

			LinkLabelDisabled.Font = defaultFont;
			LinkLabelDisabled.Color = Color.FromArgb(96, 96, 192);
            LinkLabelDisabled.TextAlign = GLFontAlignment.Left;
			LinkLabelDisabled.Padding = new GLPadding(1, 1, 1, 1);
			LinkLabelDisabled.BackgroundColor = Color.Transparent;


			TextBoxEnabled.Font = defaultFont;
			TextBoxEnabled.Color = Color.FromArgb(192, 192, 192);
            TextBoxEnabled.SelectionColor = Color.FromArgb(80, 80, 80);
            TextBoxEnabled.TextAlign = GLFontAlignment.Left;
			TextBoxEnabled.Border = new GLPadding(1);
			TextBoxEnabled.Padding = new GLPadding(1, 0, 1, 2);
			TextBoxEnabled.BorderColor = Color.FromArgb(96, 96, 96);
			TextBoxEnabled.BackgroundColor = Color.FromArgb(56, 56, 56);

			TextBoxActive.Font = defaultFont;
			TextBoxActive.Color = Color.FromArgb(192, 192, 192);
            TextBoxActive.SelectionColor = Color.FromArgb(96, 96, 96);
            TextBoxActive.TextAlign = GLFontAlignment.Left;
			TextBoxActive.Border = new GLPadding(1);
			TextBoxActive.Padding = new GLPadding(1, 0, 1, 2);
			TextBoxActive.BorderColor = Color.FromArgb(255, 192, 96);
			TextBoxActive.BackgroundColor = Color.FromArgb(56, 56, 56);

			TextBoxHover.Font = defaultFont;
			TextBoxHover.Color = Color.FromArgb(192, 192, 192);
            TextBoxHover.SelectionColor = Color.FromArgb(80, 80, 80);
            TextBoxHover.TextAlign = GLFontAlignment.Left;
			TextBoxHover.Border = new GLPadding(1);
			TextBoxHover.Padding = new GLPadding(1, 0, 1, 2);
			TextBoxHover.BorderColor = Color.FromArgb(128, 128, 128);
			TextBoxHover.BackgroundColor = Color.FromArgb(56, 56, 56);

			TextBoxDisabled.Font = defaultFont;
			TextBoxDisabled.Color = Color.FromArgb(128, 128, 128);
            TextBoxDisabled.SelectionColor = Color.FromArgb(80, 80, 80);
            TextBoxDisabled.TextAlign = GLFontAlignment.Left;
			TextBoxDisabled.Border = new GLPadding(1);
			TextBoxDisabled.Padding = new GLPadding(1, 0, 1, 2);
			TextBoxDisabled.BorderColor = Color.FromArgb(128, 128, 128);
			TextBoxDisabled.BackgroundColor = Color.FromArgb(56, 56, 56);


			CheckBoxEnabled.Font = defaultFont;
			CheckBoxEnabled.Color = Color.FromArgb(192, 192, 192);
			CheckBoxEnabled.Border = new GLPadding(1);
			CheckBoxEnabled.BorderColor = Color.FromArgb(96, 96, 96);
			CheckBoxEnabled.BackgroundColor = Color.FromArgb(56, 56, 56);

			CheckBoxPressed.Font = defaultFont;
			CheckBoxPressed.Color = Color.FromArgb(192, 192, 192);
			CheckBoxPressed.Border = new GLPadding(1);
			CheckBoxPressed.BorderColor = Color.FromArgb(255, 192, 96);
			CheckBoxPressed.BackgroundColor = Color.FromArgb(56, 56, 56);

			CheckBoxHover.Font = defaultFont;
			CheckBoxHover.Color = Color.FromArgb(192, 192, 192);
			CheckBoxHover.Border = new GLPadding(1);
			CheckBoxHover.BorderColor = Color.FromArgb(128, 128, 128);
			CheckBoxHover.BackgroundColor = Color.FromArgb(56, 56, 56);

			CheckBoxDisabled.Font = defaultFont;
			CheckBoxDisabled.Color = Color.FromArgb(128, 128, 128);
			CheckBoxDisabled.Border = new GLPadding(1);
			CheckBoxDisabled.BorderColor = Color.FromArgb(128, 128, 128);
			CheckBoxDisabled.BackgroundColor = Color.FromArgb(56, 56, 56);


			GroupLayout.Border = new GLPadding(1);
			GroupLayout.BorderColor = Color.Transparent;//Color.FromArgb(96, 96, 96);
			GroupLayout.BackgroundColor = Color.Transparent;//Color.FromArgb(240, 240, 240);


			FlowLayout.Padding = new GLPadding(2);
			FlowLayout.Border = new GLPadding(0);
			FlowLayout.BorderColor = Color.Transparent;
			FlowLayout.BackgroundColor = Color.Transparent;
			FlowLayout.Space = 2;


			SplitLayout.BackgroundColor = Color.FromArgb(0, 0, 0);
			SplitLayout.SplitterSize = 1;


			SliderEnabled.KnobColor = Color.FromArgb(80, 80, 80);
			SliderEnabled.BackgroundColor = Color.FromArgb(28, 28, 28);//Color.FromArgb(56, 56, 56);

			SliderDisabled.KnobColor = Color.Transparent; //Color.FromArgb(96, 96, 96);
			SliderDisabled.BackgroundColor = Color.FromArgb(28, 28, 28);//Color.FromArgb(56, 56, 56);

			SliderHover.KnobColor = Color.FromArgb(96, 96, 96);
			SliderHover.BackgroundColor = Color.FromArgb(32, 32, 32);

			SliderPressed.KnobColor = Color.FromArgb(80, 80, 80);
			SliderPressed.BackgroundColor = Color.FromArgb(32, 32, 32);


			ScrollableControl.Border = new GLPadding(1);
			ScrollableControl.BorderColor = Color.FromArgb(56, 56, 56);
			ScrollableControl.BackgroundColor = Color.FromArgb(41, 41, 41);


			ContextMenu.Padding = new GLPadding(1);
			ContextMenu.Border = new GLPadding(1);
            ContextMenu.BorderColor = Color.FromArgb(128, 128, 128);
            ContextMenu.BackgroundColor = Color.FromArgb(32, 32, 32);
			ContextMenu.Space = 1;


			ContextMenuEntryEnabled.Font = defaultFont;
			ContextMenuEntryEnabled.Color = Color.FromArgb(240, 240, 240);
            ContextMenuEntryEnabled.TextAlign = GLFontAlignment.Left;
			ContextMenuEntryEnabled.Border = new GLPadding(2);
			ContextMenuEntryEnabled.BorderColor = Color.FromArgb(56, 56, 56);
			ContextMenuEntryEnabled.BackgroundColor = Color.Transparent;

			ContextMenuEntryDisabled.Font = defaultFont;
			ContextMenuEntryDisabled.Color = Color.FromArgb(128, 128, 128);
            ContextMenuEntryDisabled.TextAlign = GLFontAlignment.Left;
			ContextMenuEntryDisabled.Border = new GLPadding(2);
			ContextMenuEntryDisabled.BorderColor = Color.FromArgb(56, 56, 56);
			ContextMenuEntryDisabled.BackgroundColor = Color.Transparent;

			ContextMenuEntryHover.Font = defaultFont;
			ContextMenuEntryHover.Color = Color.FromArgb(255, 255, 255);
            ContextMenuEntryHover.TextAlign = GLFontAlignment.Left;
			ContextMenuEntryHover.Border = new GLPadding(2);
			ContextMenuEntryHover.BorderColor = Color.FromArgb(64, 64, 64);
			ContextMenuEntryHover.BackgroundColor = Color.Transparent;

			ContextMenuEntryPressed.Font = defaultFont;
			ContextMenuEntryPressed.Color = Color.FromArgb(96, 96, 96);
            ContextMenuEntryPressed.TextAlign = GLFontAlignment.Left;
			ContextMenuEntryPressed.Border = new GLPadding(2);
			ContextMenuEntryPressed.BorderColor = Color.FromArgb(32, 32, 32);
			ContextMenuEntryPressed.BackgroundColor = Color.Transparent;
		}
	}
}
