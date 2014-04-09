namespace GLGUI
{
    class GLFontTextNode
    {
        public GLFontTextNodeType Type;
        public string Text;
		public float Length; // pixel length (without tweaks)
		public float LengthTweak; // length tweak for justification

        public float ModifiedLength
        {
            get { return Length + LengthTweak; }
        }

		public GLFontTextNode(GLFontTextNodeType type, string text)
        {
			Type = type;
			Text = text;
        }

        public GLFontTextNode Next;
        public GLFontTextNode Previous;
    }
}
