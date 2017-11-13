using System;

namespace Assets.Scripts.GameLogic.DataCenter
{
	[Serializable]
	public struct ActorServerSkillData
	{
		public ActorMeta TheActorMeta;

		public ActorSkillSlot SkillSlot;

		public bool IsUnlocked;

		public uint SkillLevel;

		public uint SelfSkill;
	}
}
