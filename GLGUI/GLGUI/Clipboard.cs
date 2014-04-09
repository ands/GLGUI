using System;

namespace GLGUI
{
	#if !REFERENCE_WINDOWS_FORMS
	public static class Clipboard
	{
		private static string contents = "";
		public static void SetText(string s) { contents = s; }
		public static bool ContainsText() { return contents.Length > 0; }
		public static string GetText() { return contents; }
	}
	#endif
}