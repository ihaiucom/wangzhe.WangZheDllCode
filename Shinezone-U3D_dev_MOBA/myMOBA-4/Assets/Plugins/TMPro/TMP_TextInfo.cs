using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public class TMP_TextInfo
	{
		public int characterCount;

		public int spriteCount;

		public int spaceCount;

		public int wordCount;

		public int lineCount;

		public int pageCount;

		public TMP_CharacterInfo[] characterInfo;

		public List<TMP_WordInfo> wordInfo;

		public TMP_LineInfo[] lineInfo;

		public TMP_PageInfo[] pageInfo;

		public TMP_MeshInfo meshInfo;

		public TMP_TextInfo()
		{
			this.characterInfo = new TMP_CharacterInfo[0];
			this.wordInfo = new List<TMP_WordInfo>(32);
			this.lineInfo = new TMP_LineInfo[16];
			this.pageInfo = new TMP_PageInfo[16];
			this.meshInfo = default(TMP_MeshInfo);
			this.meshInfo.meshArrays = new UIVertex[17][];
		}

		public void Clear()
		{
			this.characterCount = 0;
			this.spaceCount = 0;
			this.wordCount = 0;
			this.lineCount = 0;
			this.pageCount = 0;
			this.spriteCount = 0;
			Array.Clear(this.characterInfo, 0, this.characterInfo.Length);
			this.wordInfo.Clear();
			Array.Clear(this.lineInfo, 0, this.lineInfo.Length);
			Array.Clear(this.pageInfo, 0, this.pageInfo.Length);
		}
	}
}
