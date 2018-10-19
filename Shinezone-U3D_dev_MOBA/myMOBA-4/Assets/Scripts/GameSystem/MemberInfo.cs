using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	public class MemberInfo
	{
		public uint RoomMemberType;

		public ulong ullUid;

		public uint dwObjId;

		public int iFromGameEntity;

		public int iLogicWorldID;

		public COM_PLAYERCAMP camp;

		public uint dwPosOfCamp;

		public string MemberName;

		public string MemberHeadUrl;

		public uint dwMemberPvpLevel;

		public COMDT_CHOICEHERO[] ChoiceHero;

		public bool isPrepare;

		public bool isGM;

		public COMDT_FAKEACNT_DETAIL WarmNpc;

		public uint dwMemberHeadId;

		public uint dwMemberLevel;

		public uint[] canUseHero;

		public uint swapSeq;

		public byte swapStatus;

		public ulong swapUid;

		public COMDT_RECENT_USED_HERO recentUsedHero;

		public COMDT_HERO_STATISTIC_DETAIL selectHeroInfo;
	}
}
