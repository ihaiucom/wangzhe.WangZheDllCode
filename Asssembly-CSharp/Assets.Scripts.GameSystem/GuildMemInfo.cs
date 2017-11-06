using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	[Serializable]
	public class GuildMemInfo
	{
		public stGuildMemBriefInfo stBriefInfo;

		public COM_PLAYER_GUILD_STATE enPosition;

		public uint dwConstruct;

		public MemberRankInfo RankInfo;

		public COM_ACNT_GAME_STATE GameState;

		public uint dwGameStartTime;

		public uint LastLoginTime;

		public uint JoinTime;

		public COMDT_MEMBER_GUILD_MATCH_INFO GuildMatchInfo;

		public bool isGuildMatchSignedUp;

		public bool isGuildMatchOfflineInvitedByHostPlayer;

		public uint antiDisturbBits;

		public GuildMemInfo()
		{
			this.RankInfo = new MemberRankInfo();
			this.GuildMatchInfo = new COMDT_MEMBER_GUILD_MATCH_INFO();
		}

		public void Reset()
		{
			this.stBriefInfo.Reset();
			this.enPosition = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_MEMBER;
			this.dwConstruct = 0u;
			this.RankInfo.Reset();
			this.GameState = COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
			this.LastLoginTime = 0u;
			this.JoinTime = 0u;
			this.GuildMatchInfo.bIsLeader = 0;
			this.GuildMatchInfo.ullTeamLeaderUid = 0uL;
			this.GuildMatchInfo.dwScore = 0u;
			this.isGuildMatchSignedUp = false;
			this.isGuildMatchOfflineInvitedByHostPlayer = false;
		}

		public override bool Equals(object obj)
		{
			GuildMemInfo guildMemInfo = obj as GuildMemInfo;
			return guildMemInfo != null && this.stBriefInfo.uulUid == guildMemInfo.stBriefInfo.uulUid;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
