using System;

namespace behaviac
{
	public struct CStringID
	{
		private uint m_id;

		public CStringID(string str)
		{
			this.m_id = CRC32.CalcCRC(str);
		}

		public void SetId(string str)
		{
			this.m_id = CRC32.CalcCRC(str);
		}

		public uint GetId()
		{
			return this.m_id;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			CStringID cStringID = (CStringID)obj;
			return this.m_id == cStringID.m_id;
		}

		public bool Equals(CStringID p)
		{
			return p != null && this.m_id == p.m_id;
		}

		public override int GetHashCode()
		{
			return (int)this.m_id;
		}

		public static bool operator ==(CStringID a, CStringID b)
		{
			return a.m_id == b.m_id;
		}

		public static bool operator !=(CStringID a, CStringID b)
		{
			return a.m_id != b.m_id;
		}
	}
}
