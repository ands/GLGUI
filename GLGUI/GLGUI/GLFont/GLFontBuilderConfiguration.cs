using System;
using System.Collections.Generic;
using System.Text;

namespace GLGUI
{
    public enum TextGenerationRenderHint
    {
        /// <summary>
        /// Use AntiAliasGridFit when rendering the ttf character set to create the GLFont texture
        /// </summary>
        AntiAliasGridFit,
        /// <summary>
        /// Use AntiAlias when rendering the ttf character set to create the GLFont texture
        /// </summary>
        AntiAlias,
        /// <summary>
        /// Use ClearTypeGridFit if the font is smaller than 12, otherwise use AntiAlias
        /// </summary>
        SizeDependent,
        /// <summary>
        /// Use ClearTypeGridFit when rendering the ttf character set to create the GLFont texture
        /// </summary>
        ClearTypeGridFit,
        /// <summary>
        /// Use SystemDefault when rendering the ttf character set to create the GLFont texture
        /// </summary>
        SystemDefault
    }

    public enum CharacterKerningRule
    {
        /// <summary>
        /// Ordinary kerning
        /// </summary>
        Normal,
        /// <summary>
        /// All kerning pairs involving this character will kern by 0. This will
        /// override both Normal and NotMoreThanHalf for any pair.
        /// </summary>
        Zero,
        /// <summary>
        /// Any kerning pairs involving this character will not kern
        /// by more than half the minimum width of the two characters 
        /// involved. This will override Normal for any pair.
        /// </summary>
        NotMoreThanHalf
    }

    public class GLFontKerningConfiguration
    {
        /// <summary>
        /// Kerning rules for particular characters
        /// </summary>
        private Dictionary<char, CharacterKerningRule> CharacterKerningRules = new Dictionary<char, CharacterKerningRule>();

        /// <summary>
        /// When measuring the bounds of glyphs, and performing kerning calculations, 
        /// this is the minimum alpha level that is necessray for a pixel to be considered
        /// non-empty. This should be set to a value on the range [0,255]
        /// </summary>
        public byte alphaEmptyPixelTolerance = 0;


        /// <summary>
        /// Sets all characters in the given string to the specified kerning rule.
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="rule"></param>
        public void BatchSetCharacterKerningRule(String chars, CharacterKerningRule rule)
        {
            foreach (var c in chars)
            {
                CharacterKerningRules[c] = rule;
            }
        }

        /// <summary>
        /// Sets the specified character kerning rule.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="rule"></param>
        public void SetCharacterKerningRule(char c, CharacterKerningRule rule)
        {
            CharacterKerningRules[c] = rule;
        }

        public CharacterKerningRule GetCharacterKerningRule(char c)
        {
            if (CharacterKerningRules.ContainsKey(c))
            {
                return CharacterKerningRules[c];
            }

            return CharacterKerningRule.Normal;
        }

        /// <summary>
        /// Given a pair of characters, this will return the overriding 
        /// CharacterKerningRule.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public CharacterKerningRule GetOverridingCharacterKerningRuleForPair(String str)
        {

            if (str.Length < 2)
            {
                return CharacterKerningRule.Normal;
            }

            char c1 = str[0];
            char c2 = str[1];

            if (GetCharacterKerningRule(c1) == CharacterKerningRule.Zero || GetCharacterKerningRule(c2) == CharacterKerningRule.Zero)
            {
                return CharacterKerningRule.Zero;
            }
            else if (GetCharacterKerningRule(c1) == CharacterKerningRule.NotMoreThanHalf || GetCharacterKerningRule(c2) == CharacterKerningRule.NotMoreThanHalf)
            {
                return CharacterKerningRule.NotMoreThanHalf;
            }

            return CharacterKerningRule.Normal;
        }

        public GLFontKerningConfiguration()
        {
            BatchSetCharacterKerningRule("_^", CharacterKerningRule.Zero);
            SetCharacterKerningRule('\"', CharacterKerningRule.NotMoreThanHalf);
            SetCharacterKerningRule('\'', CharacterKerningRule.NotMoreThanHalf);
        }
    }

    /// <summary>
    /// What settings to use when building the font
    /// </summary>
    public class GLFontBuilderConfiguration
    {
        public GLFontKerningConfiguration KerningConfig = new GLFontKerningConfiguration();

        /// <summary>
        /// Whether to use super sampling when building font texture pages
        /// 
        /// 
        /// </summary>
        public int SuperSampleLevels = 1;

        /// <summary>
        /// The standard width of texture pages (the page will
        /// automatically be cropped if there is extra space)
        /// </summary>
        public int PageWidth = 512;

        /// <summary>
        /// The standard height of texture pages (the page will
        /// automatically be cropped if there is extra space)
        /// </summary>
        public int PageHeight = 512;

        /// <summary>
        /// Whether to force texture pages to use a power of two.
        /// </summary>
        public bool ForcePowerOfTwo = true;

        /// <summary>
        /// The margin (on all sides) around glyphs when rendered to
        /// their texture page
        /// </summary>
        public int GlyphMargin = 2;
       
        /// <summary>
        /// Set of characters to support
        /// </summary>
        public string charSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890.:,;'\"(!?)+-*/=_{}[]@~#\\<>|^%$£&";

        /// <summary>
        /// Which render hint to use when rendering the ttf character set to create the GLFont texture
        /// </summary>
        public TextGenerationRenderHint TextGenerationRenderHint = TextGenerationRenderHint.SizeDependent;
    }
}
