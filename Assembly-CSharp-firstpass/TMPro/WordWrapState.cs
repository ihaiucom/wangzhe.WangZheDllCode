using System;
using UnityEngine;

namespace TMPro
{
	public struct WordWrapState
	{
		public int previous_WordBreak;

		public int total_CharacterCount;

		public int visible_CharacterCount;

		public int visible_SpriteCount;

		public int firstVisibleCharacterIndex;

		public int lastVisibleCharIndex;

		public int lineNumber;

		public float maxAscender;

		public float maxDescender;

		public float xAdvance;

		public float preferredWidth;

		public float preferredHeight;

		public float maxFontScale;

		public float previousLineScale;

		public int wordCount;

		public FontStyles fontStyle;

		public float fontScale;

		public float currentFontSize;

		public float baselineOffset;

		public float lineOffset;

		public TMP_TextInfo textInfo;

		public TMP_LineInfo lineInfo;

		public Color32 vertexColor;

		public int colorStackIndex;

		public Extents meshExtents;
	}
}
