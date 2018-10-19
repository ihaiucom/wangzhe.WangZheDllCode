using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public struct DAMAGE_ACTOR_INFO
	{
		public string actorName;

		public string playerName;

		public ActorTypeDef actorType;

		public int ConfigId;

		public byte actorSubType;

		public byte bMonsterType;

		public SortedList<ulong, SKILL_SLOT_HURT_INFO[]> listDamage;
	}
}
