using System;

namespace Assets.Scripts.GameSystem
{
	public class CSignalTips_InBatMsg : CSignalTipsElement
	{
		public ulong playerID;

		public uint heroID;

		public string content;

		public string sound;

		public CSignalTips_InBatMsg(ulong playerID, uint heroID, string content, string sound) : base(CSignalTipsElement.EType.InBattleMsg)
		{
			this.playerID = playerID;
			this.heroID = heroID;
			this.content = content;
			this.sound = sound;
		}
	}
}
