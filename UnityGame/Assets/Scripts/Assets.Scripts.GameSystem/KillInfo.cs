using Assets.Scripts.GameLogic;
using System;

namespace Assets.Scripts.GameSystem
{
	public struct KillInfo
	{
		public uint killerObjID;

		public string KillerImgSrc;

		public string VictimImgSrc;

		public string[] assistImgSrc;

		public KillDetailInfoType MsgType;

		public bool bSrcAllies;

		public bool bPlayerSelf_KillOrKilled;

		public ActorTypeDef actorType;

		public byte bPriority;

		public bool bSuicide;

		public KillInfo(string inKillerImg, string inVictimImg, KillDetailInfoType type, bool bSrcAllies, bool bPlayerSelf_KillOrKilled, ActorTypeDef actorType, bool bSuicide = false)
		{
			this.KillerImgSrc = inKillerImg;
			this.VictimImgSrc = inVictimImg;
			this.MsgType = type;
			this.bSrcAllies = bSrcAllies;
			this.bPlayerSelf_KillOrKilled = bPlayerSelf_KillOrKilled;
			this.actorType = actorType;
			this.bSuicide = bSuicide;
			KillNotify.knPriority.TryGetValue(type, ref this.bPriority);
			this.assistImgSrc = new string[4];
			this.killerObjID = 0u;
		}
	}
}
