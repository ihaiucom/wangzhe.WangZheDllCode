using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public class TextMeshProFont : ScriptableObject
	{
		[SerializeField]
		private FaceInfo m_fontInfo;

		public int fontHashCode;

		[SerializeField]
		public Texture2D atlas;

		[SerializeField]
		public Material material;

		public int materialHashCode;

		[SerializeField]
		private List<GlyphInfo> m_glyphInfoList;

		private Dictionary<int, GlyphInfo> m_characterDictionary;

		private Dictionary<int, KerningPair> m_kerningDictionary;

		[SerializeField]
		private KerningTable m_kerningInfo;

		[SerializeField]
		private KerningPair m_kerningPair;

		[SerializeField]
		private LineBreakingTable m_lineBreakingInfo;

		[SerializeField]
		public FontCreationSetting fontCreationSettings;

		[SerializeField]
		public bool propertiesChanged;

		private int[] m_characterSet;

		public float NormalStyle;

		public float BoldStyle = 0.75f;

		public byte ItalicStyle = 35;

		public byte TabSize = 10;

		public FaceInfo fontInfo
		{
			get
			{
				return this.m_fontInfo;
			}
		}

		public Dictionary<int, GlyphInfo> characterDictionary
		{
			get
			{
				return this.m_characterDictionary;
			}
		}

		public Dictionary<int, KerningPair> kerningDictionary
		{
			get
			{
				return this.m_kerningDictionary;
			}
		}

		public KerningTable kerningInfo
		{
			get
			{
				return this.m_kerningInfo;
			}
		}

		public LineBreakingTable lineBreakingInfo
		{
			get
			{
				return this.m_lineBreakingInfo;
			}
		}

		private void OnEnable()
		{
			if (this.m_characterDictionary == null)
			{
				this.ReadFontDefinition();
			}
		}

		private void OnDisable()
		{
		}

		public void AddFaceInfo(FaceInfo faceInfo)
		{
			this.m_fontInfo = faceInfo;
		}

		public void AddGlyphInfo(GlyphInfo[] glyphInfo)
		{
			this.m_glyphInfoList = new List<GlyphInfo>();
			this.m_characterSet = new int[this.m_fontInfo.CharacterCount];
			for (int i = 0; i < this.m_fontInfo.CharacterCount; i++)
			{
				GlyphInfo glyphInfo2 = new GlyphInfo();
				glyphInfo2.id = glyphInfo[i].id;
				glyphInfo2.x = glyphInfo[i].x;
				glyphInfo2.y = glyphInfo[i].y;
				glyphInfo2.width = glyphInfo[i].width;
				glyphInfo2.height = glyphInfo[i].height;
				glyphInfo2.xOffset = glyphInfo[i].xOffset;
				glyphInfo2.yOffset = glyphInfo[i].yOffset + this.m_fontInfo.Padding;
				glyphInfo2.xAdvance = glyphInfo[i].xAdvance;
				this.m_glyphInfoList.Add(glyphInfo2);
				this.m_characterSet[i] = glyphInfo2.id;
			}
		}

		public void AddKerningInfo(KerningTable kerningTable)
		{
			this.m_kerningInfo = kerningTable;
		}

		public void ReadFontDefinition()
		{
			if (this.m_fontInfo == null)
			{
				return;
			}
			this.m_characterDictionary = new Dictionary<int, GlyphInfo>();
			using (List<GlyphInfo>.Enumerator enumerator = this.m_glyphInfoList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GlyphInfo current = enumerator.get_Current();
					if (!this.m_characterDictionary.ContainsKey(current.id))
					{
						this.m_characterDictionary.Add(current.id, current);
					}
				}
			}
			GlyphInfo glyphInfo = new GlyphInfo();
			if (this.m_characterDictionary.ContainsKey(32))
			{
				this.m_characterDictionary.get_Item(32).width = this.m_fontInfo.Ascender / 5f;
				this.m_characterDictionary.get_Item(32).height = this.m_fontInfo.Ascender - this.m_fontInfo.Descender;
				this.m_characterDictionary.get_Item(32).yOffset = this.m_fontInfo.Ascender;
			}
			else
			{
				glyphInfo = new GlyphInfo();
				glyphInfo.id = 32;
				glyphInfo.x = 0f;
				glyphInfo.y = 0f;
				glyphInfo.width = this.m_fontInfo.Ascender / 5f;
				glyphInfo.height = this.m_fontInfo.Ascender - this.m_fontInfo.Descender;
				glyphInfo.xOffset = 0f;
				glyphInfo.yOffset = this.m_fontInfo.Ascender;
				glyphInfo.xAdvance = this.m_fontInfo.PointSize / 4f;
				this.m_characterDictionary.Add(32, glyphInfo);
			}
			if (!this.m_characterDictionary.ContainsKey(10))
			{
				glyphInfo = new GlyphInfo();
				glyphInfo.id = 10;
				glyphInfo.x = 0f;
				glyphInfo.y = 0f;
				glyphInfo.width = 0f;
				glyphInfo.height = 0f;
				glyphInfo.xOffset = 0f;
				glyphInfo.yOffset = 0f;
				glyphInfo.xAdvance = 0f;
				this.m_characterDictionary.Add(10, glyphInfo);
				this.m_characterDictionary.Add(13, glyphInfo);
			}
			if (!this.m_characterDictionary.ContainsKey(9))
			{
				glyphInfo = new GlyphInfo();
				glyphInfo.id = 9;
				glyphInfo.x = this.m_characterDictionary.get_Item(32).x;
				glyphInfo.y = this.m_characterDictionary.get_Item(32).y;
				glyphInfo.width = this.m_characterDictionary.get_Item(32).width * (float)this.TabSize;
				glyphInfo.height = this.m_characterDictionary.get_Item(32).height;
				glyphInfo.xOffset = this.m_characterDictionary.get_Item(32).xOffset;
				glyphInfo.yOffset = this.m_characterDictionary.get_Item(32).yOffset;
				glyphInfo.xAdvance = this.m_characterDictionary.get_Item(32).xAdvance * (float)this.TabSize;
				this.m_characterDictionary.Add(9, glyphInfo);
			}
			this.m_fontInfo.TabWidth = this.m_characterDictionary.get_Item(32).xAdvance;
			this.m_kerningDictionary = new Dictionary<int, KerningPair>();
			List<KerningPair> kerningPairs = this.m_kerningInfo.kerningPairs;
			for (int i = 0; i < kerningPairs.get_Count(); i++)
			{
				KerningPair kerningPair = kerningPairs.get_Item(i);
				KerningPairKey kerningPairKey = new KerningPairKey(kerningPair.AscII_Left, kerningPair.AscII_Right);
				if (!this.m_kerningDictionary.ContainsKey(kerningPairKey.key))
				{
					this.m_kerningDictionary.Add(kerningPairKey.key, kerningPair);
				}
				else
				{
					Debug.Log(string.Concat(new object[]
					{
						"Kerning Key for [",
						kerningPairKey.ascii_Left,
						"] and [",
						kerningPairKey.ascii_Right,
						"] already exists."
					}));
				}
			}
			this.m_lineBreakingInfo = new LineBreakingTable();
			TextAsset textAsset = Resources.Load("LineBreaking Leading Characters", typeof(TextAsset)) as TextAsset;
			if (textAsset != null)
			{
				this.m_lineBreakingInfo.leadingCharacters = this.GetCharacters(textAsset);
			}
			TextAsset textAsset2 = Resources.Load("LineBreaking Following Characters", typeof(TextAsset)) as TextAsset;
			if (textAsset2 != null)
			{
				this.m_lineBreakingInfo.followingCharacters = this.GetCharacters(textAsset2);
			}
			string name = base.name;
			this.fontHashCode = 0;
			for (int j = 0; j < name.get_Length(); j++)
			{
				this.fontHashCode = (this.fontHashCode << 5) - this.fontHashCode + (int)name.get_Chars(j);
			}
			string name2 = this.material.name;
			this.materialHashCode = 0;
			for (int k = 0; k < name2.get_Length(); k++)
			{
				this.materialHashCode = (this.materialHashCode << 5) - this.materialHashCode + (int)name2.get_Chars(k);
			}
		}

		private Dictionary<int, char> GetCharacters(TextAsset file)
		{
			Dictionary<int, char> dictionary = new Dictionary<int, char>();
			string text = file.text;
			for (int i = 0; i < text.get_Length(); i++)
			{
				char c = text.get_Chars(i);
				if (!dictionary.ContainsKey((int)c))
				{
					dictionary.Add((int)c, c);
				}
			}
			return dictionary;
		}
	}
}
