using System;

namespace Assets.Scripts.GameLogic.DataCenter
{
	[Serializable]
	public struct ActorStaticSkillData
	{
		public ActorMeta TheActorMeta;

		public ActorSkillSlot SkillSlot;

		public int SkillId;

		public int PassiveSkillId;
	}
}
