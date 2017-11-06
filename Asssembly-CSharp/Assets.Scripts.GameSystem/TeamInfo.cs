using System;

namespace Assets.Scripts.GameSystem
{
	public class TeamInfo
	{
		public PlayerUniqueID stSelfInfo;

		public PlayerUniqueID stTeamMaster;

		public TeamAttrib stTeamInfo;

		public ListView<TeamMember> MemInfoList;

		public uint TeamId;

		public uint TeamSeq;

		public int TeamEntity;

		public ulong TeamFeature;

		public TeamInfo()
		{
			this.stSelfInfo = new PlayerUniqueID();
			this.stTeamMaster = new PlayerUniqueID();
			this.stTeamInfo = new TeamAttrib();
			this.MemInfoList = new ListView<TeamMember>();
		}

		public static TeamMember GetMemberInfo(TeamInfo teamInfo, int Pos)
		{
			for (int i = 0; i < teamInfo.MemInfoList.Count; i++)
			{
				if ((ulong)teamInfo.MemInfoList[i].dwPosOfTeam == (ulong)((long)(Pos - 1)))
				{
					return teamInfo.MemInfoList[i];
				}
			}
			return null;
		}
	}
}
