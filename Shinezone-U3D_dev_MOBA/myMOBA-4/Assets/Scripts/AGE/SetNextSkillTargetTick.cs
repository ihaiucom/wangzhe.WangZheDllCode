using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SetNextSkillTargetTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int nextSkillTargetID = -1;

		public bool clear;

		public SkillSlotType slotType;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SetNextSkillTargetTick setNextSkillTargetTick = src as SetNextSkillTargetTick;
			this.targetId = setNextSkillTargetTick.targetId;
			this.nextSkillTargetID = setNextSkillTargetTick.nextSkillTargetID;
			this.slotType = setNextSkillTargetTick.slotType;
			this.clear = setNextSkillTargetTick.clear;
		}

		public override BaseEvent Clone()
		{
			SetNextSkillTargetTick setNextSkillTargetTick = ClassObjPool<SetNextSkillTargetTick>.Get();
			setNextSkillTargetTick.CopyData(this);
			return setNextSkillTargetTick;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.nextSkillTargetID = -1;
			this.slotType = SkillSlotType.SLOT_SKILL_0;
			this.clear = false;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				return;
			}
			SkillSlot skillSlot = actorHandle.handle.SkillControl.GetSkillSlot(this.slotType);
			if (skillSlot != null)
			{
				if (this.clear)
				{
					skillSlot.NextSkillTargetIDs.Clear();
				}
				else
				{
					PoolObjHandle<ActorRoot> actorHandle2 = _action.GetActorHandle(this.nextSkillTargetID);
					if (!actorHandle2)
					{
						return;
					}
					if (skillSlot.NextSkillTargetIDs.IndexOf(actorHandle2.handle.ObjID) < 0)
					{
						skillSlot.NextSkillTargetIDs.Add(actorHandle2.handle.ObjID);
					}
				}
			}
		}
	}
}
