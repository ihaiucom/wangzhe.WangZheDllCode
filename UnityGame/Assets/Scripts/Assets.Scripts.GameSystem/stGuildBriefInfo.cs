using System;

namespace Assets.Scripts.GameSystem
{
	[Serializable]
	public struct stGuildBriefInfo
	{
		public ulong uulUid;

		public int LogicWorldId;

		public string sName;

		public byte bLevel;

		public byte bMemberNum;

		public string sBulletin;

		public uint dwHeadId;

		public uint dwSettingMask;

		public uint Rank;

		public uint Ability;

		private byte levelLimit;

		private byte gradeLimit;

		public byte LevelLimit
		{
			get
			{
				return CGuildHelper.GetFixedGuildLevelLimit(this.levelLimit);
			}
			set
			{
				this.levelLimit = value;
			}
		}

		public byte GradeLimit
		{
			get
			{
				return CGuildHelper.GetFixedGuildGradeLimit(this.gradeLimit);
			}
			set
			{
				this.gradeLimit = value;
			}
		}

		public void Reset()
		{
			this.uulUid = 0uL;
			this.sName = null;
			this.bLevel = 0;
			this.bMemberNum = 0;
			this.sBulletin = null;
			this.dwHeadId = 0u;
			this.dwSettingMask = 0u;
			this.Rank = 0u;
			this.Ability = 0u;
			this.LevelLimit = 0;
			this.GradeLimit = 0;
		}
	}
}
