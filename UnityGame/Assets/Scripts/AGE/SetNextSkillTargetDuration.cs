using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SetNextSkillTargetDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int nextSkillTargetID = -1;

		public SkillSlotType slotType;

		private PoolObjHandle<ActorRoot> nextSkillObj;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SetNextSkillTargetDuration setNextSkillTargetDuration = src as SetNextSkillTargetDuration;
			this.targetId = setNextSkillTargetDuration.targetId;
			this.nextSkillTargetID = setNextSkillTargetDuration.nextSkillTargetID;
			this.slotType = setNextSkillTargetDuration.slotType;
		}

		public override BaseEvent Clone()
		{
			SetNextSkillTargetDuration setNextSkillTargetDuration = ClassObjPool<SetNextSkillTargetDuration>.Get();
			setNextSkillTargetDuration.CopyData(this);
			return setNextSkillTargetDuration;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.nextSkillTargetID = -1;
			this.slotType = SkillSlotType.SLOT_SKILL_0;
			this.nextSkillObj.Release();
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				return;
			}
			this.nextSkillObj = _action.GetActorHandle(this.nextSkillTargetID);
			if (!this.nextSkillObj)
			{
				return;
			}
			SkillSlot skillSlot = actorHandle.handle.SkillControl.GetSkillSlot(this.slotType);
			if (skillSlot != null && this.nextSkillTargetID > 0 && skillSlot.NextSkillTargetIDs.IndexOf(this.nextSkillObj.handle.ObjID) < 0)
			{
				skillSlot.NextSkillTargetIDs.Add(this.nextSkillObj.handle.ObjID);
			}
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
			if (!this.nextSkillObj)
			{
				return;
			}
			if (this.nextSkillObj.handle.ActorControl.IsDeadState)
			{
				this.RemoveTarget(_action, this.nextSkillObj);
				this.nextSkillObj.Release();
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
			if (!this.nextSkillObj)
			{
				return;
			}
			this.RemoveTarget(_action, this.nextSkillObj);
		}

		private void RemoveTarget(Action _action, PoolObjHandle<ActorRoot> nextObj)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				return;
			}
			SkillSlot skillSlot = actorHandle.handle.SkillControl.GetSkillSlot(this.slotType);
			if (skillSlot != null)
			{
				int num = skillSlot.NextSkillTargetIDs.IndexOf(nextObj.handle.ObjID);
				if (num >= 0)
				{
					skillSlot.NextSkillTargetIDs.RemoveAt(num);
				}
			}
		}
	}
}
