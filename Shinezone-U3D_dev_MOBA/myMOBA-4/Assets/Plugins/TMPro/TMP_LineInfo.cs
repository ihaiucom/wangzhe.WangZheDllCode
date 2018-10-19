using System;

namespace TMPro
{
	public struct TMP_LineInfo
	{
		public int characterCount;

		public int spaceCount;

		public int wordCount;

		public int firstCharacterIndex;

		public int lastCharacterIndex;

		public int lastVisibleCharacterIndex;

		public float lineLength;

		public float lineHeight;

		public float ascender;

		public float descender;

		public float maxAdvance;

		public TextAlignmentOptions alignment;

		public Extents lineExtents;
	}
}
