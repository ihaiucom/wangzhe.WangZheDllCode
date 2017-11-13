using System;

namespace Assets.Scripts.GameSystem
{
	public class CSignalTips : CSignalTipsElement
	{
		public int m_signalID;

		public uint m_heroID;

		public bool m_isHostPlayer;

		public byte m_bAlies;

		public byte m_elementType;

		public uint m_targetHeroID;

		public CSignalTips(int signalID, uint heroID, bool isHostPlayer, byte bAlice = 0, byte elementType = 0, uint targetHeroID = 0u) : base(CSignalTipsElement.EType.Signal)
		{
			this.m_signalID = signalID;
			this.m_heroID = heroID;
			this.m_isHostPlayer = isHostPlayer;
			this.m_bAlies = bAlice;
			this.m_elementType = elementType;
			this.m_targetHeroID = targetHeroID;
		}
	}
}
