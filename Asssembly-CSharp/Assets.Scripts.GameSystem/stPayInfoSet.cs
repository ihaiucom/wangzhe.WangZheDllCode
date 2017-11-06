using System;

namespace Assets.Scripts.GameSystem
{
	public struct stPayInfoSet
	{
		public int m_payInfoCount;

		public stPayInfo[] m_payInfos;

		public stPayInfoSet(int maxPayInfoCount)
		{
			this.m_payInfoCount = 0;
			this.m_payInfos = new stPayInfo[maxPayInfoCount];
		}
	}
}
