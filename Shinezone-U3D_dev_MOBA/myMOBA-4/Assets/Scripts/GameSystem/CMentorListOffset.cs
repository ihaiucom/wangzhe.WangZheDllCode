using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CMentorListOffset
	{
		public CS_STUDENTLIST_TYPE m_type;

		public uint m_lastNum;

		public bool m_isEnd = true;

		public bool needShowMore()
		{
			return !this.m_isEnd;
		}
	}
}
