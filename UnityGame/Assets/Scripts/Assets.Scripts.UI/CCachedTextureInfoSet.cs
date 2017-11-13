using System;

namespace Assets.Scripts.UI
{
	public class CCachedTextureInfoSet
	{
		public const int c_version = 10003;

		public ListView<CCachedTextureInfo> m_cachedTextureInfos = new ListView<CCachedTextureInfo>();

		public DictionaryView<string, CCachedTextureInfo> m_cachedTextureInfoMap = new DictionaryView<string, CCachedTextureInfo>();

		public void Write(byte[] data, ref int offset)
		{
			int num = offset;
			offset += 4;
			CMemoryManager.WriteShort(10003, data, ref offset);
			CMemoryManager.WriteShort((short)this.m_cachedTextureInfos.Count, data, ref offset);
			for (int i = 0; i < this.m_cachedTextureInfos.Count; i++)
			{
				CMemoryManager.WriteString(this.m_cachedTextureInfos[i].m_key, data, ref offset);
				CMemoryManager.WriteShort((short)this.m_cachedTextureInfos[i].m_width, data, ref offset);
				CMemoryManager.WriteShort((short)this.m_cachedTextureInfos[i].m_height, data, ref offset);
				CMemoryManager.WriteDateTime(ref this.m_cachedTextureInfos[i].m_lastModifyTime, data, ref offset);
				CMemoryManager.WriteByte(this.m_cachedTextureInfos[i].m_isGif ? 1 : 0, data, ref offset);
			}
			CMemoryManager.WriteInt(offset - num, data, ref num);
		}

		public void Read(byte[] data, ref int offset)
		{
			this.m_cachedTextureInfos.Clear();
			this.m_cachedTextureInfoMap.Clear();
			if (data == null)
			{
				return;
			}
			int num = data.Length - offset;
			if (num < 6)
			{
				return;
			}
			int num2 = CMemoryManager.ReadInt(data, ref offset);
			if (num2 < 6 || num2 > num)
			{
				return;
			}
			int num3 = CMemoryManager.ReadShort(data, ref offset);
			if (num3 != 10003)
			{
				return;
			}
			int num4 = CMemoryManager.ReadShort(data, ref offset);
			for (int i = 0; i < num4; i++)
			{
				CCachedTextureInfo cCachedTextureInfo = new CCachedTextureInfo();
				cCachedTextureInfo.m_key = CMemoryManager.ReadString(data, ref offset);
				cCachedTextureInfo.m_width = CMemoryManager.ReadShort(data, ref offset);
				cCachedTextureInfo.m_height = CMemoryManager.ReadShort(data, ref offset);
				cCachedTextureInfo.m_lastModifyTime = CMemoryManager.ReadDateTime(data, ref offset);
				cCachedTextureInfo.m_isGif = (CMemoryManager.ReadByte(data, ref offset) > 0);
				if (!this.m_cachedTextureInfoMap.ContainsKey(cCachedTextureInfo.m_key))
				{
					this.m_cachedTextureInfoMap.Add(cCachedTextureInfo.m_key, cCachedTextureInfo);
					this.m_cachedTextureInfos.Add(cCachedTextureInfo);
				}
			}
			this.m_cachedTextureInfos.Sort();
		}

		public CCachedTextureInfo GetCachedTextureInfo(string key)
		{
			if (this.m_cachedTextureInfoMap.ContainsKey(key))
			{
				CCachedTextureInfo result = null;
				this.m_cachedTextureInfoMap.TryGetValue(key, out result);
				return result;
			}
			return null;
		}

		public void AddTextureInfo(string key, CCachedTextureInfo cachedTextureInfo)
		{
			if (this.m_cachedTextureInfoMap.ContainsKey(key))
			{
				return;
			}
			this.m_cachedTextureInfoMap.Add(key, cachedTextureInfo);
			this.m_cachedTextureInfos.Add(cachedTextureInfo);
		}

		public string RemoveEarliestTextureInfo()
		{
			if (this.m_cachedTextureInfos.Count <= 0)
			{
				return null;
			}
			CCachedTextureInfo cCachedTextureInfo = this.m_cachedTextureInfos[0];
			this.m_cachedTextureInfos.RemoveAt(0);
			this.m_cachedTextureInfoMap.Remove(cCachedTextureInfo.m_key);
			return cCachedTextureInfo.m_key;
		}

		public void SortTextureInfo()
		{
			this.m_cachedTextureInfos.Sort();
		}
	}
}
