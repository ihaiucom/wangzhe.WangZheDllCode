using System;

namespace TMPro
{
	public struct TMP_WordInfo
	{
		public int firstCharacterIndex;

		public int lastCharacterIndex;

		public int characterCount;

		public float length;

		public string GetWord(TMP_CharacterInfo[] charInfo)
		{
			string text = string.Empty;
			for (int i = this.firstCharacterIndex; i < this.lastCharacterIndex + 1; i++)
			{
				text += charInfo[i].character;
			}
			return text;
		}
	}
}
