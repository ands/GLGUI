using System.Drawing;

namespace GLGUI
{
    public class GLFontGlyph
    {
        /// <summary>
        /// Which texture page the glyph is on
        /// </summary>
        public int page; 
        
        /// <summary>
        /// The rectangle defining the glyphs position on the page
        /// </summary>
        public Rectangle rect;
        
        /// <summary>
        /// How far the glyph would need to be vertically offset to be vertically in line with the tallest glyph in the set of all glyphs
        /// </summary>
        public int yOffset;

        /// <summary>
        /// Which character this glyph represents
        /// </summary>
        public char character;

        public PointF textureMin, textureMax;

        public GLFontGlyph(int page, Rectangle rect, int yOffset, char character)
        {
            this.page = page;
            this.rect = rect;
            this.yOffset = yOffset;
            this.character = character;
        }
    }
}
