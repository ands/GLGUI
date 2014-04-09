using System;

namespace GLGUI
{
	[Flags]
	public enum GLAnchorStyles : byte
	{
		None = 0x00,
		Left = 0x01,
		Top = 0x02,
		Right = 0x04,
		Bottom = 0x08,
		All = 0x0f
	}
}
