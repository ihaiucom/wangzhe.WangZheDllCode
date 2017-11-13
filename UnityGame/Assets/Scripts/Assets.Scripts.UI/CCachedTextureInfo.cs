using System;

namespace Assets.Scripts.UI
{
	public class CCachedTextureInfo : IComparable
	{
		public string m_key;

		public int m_width;

		public int m_height;

		public DateTime m_lastModifyTime;

		public bool m_isGif;

		public int CompareTo(object obj)
		{
			CCachedTextureInfo cCachedTextureInfo = obj as CCachedTextureInfo;
			if (this.m_lastModifyTime > cCachedTextureInfo.m_lastModifyTime)
			{
				return 1;
			}
			if (this.m_lastModifyTime == cCachedTextureInfo.m_lastModifyTime)
			{
				return 0;
			}
			return -1;
		}
	}
}
