using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class ChangeSkillTriggerTick : TickEvent
	{
		private struct SelectionData
		{
			public int ID;

			public int ProbabilityBase;
		}

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int attackTargetId = -1;

		public SkillSlotType slotType;

		public int effectTime;

		[AssetReference(AssetRefType.SkillID)]
		public int changeSkillID;

		public int changeSkillID1Probability = 100;

		[AssetReference(AssetRefType.SkillID)]
		public int changeSkillID2;

		public int changeSkillID2Probability = 100;

		[AssetReference(AssetRefType.SkillID)]
		public int changeSkillID3;

		public int changeSkillID3Probability = 100;

		[AssetReference(AssetRefType.SkillID)]
		public int changeSkillID4;

		public int changeSkillID4Probability = 100;

		public int recoverSkillID;

		public bool bCheckCondition;

		public bool bOvertimeCD = true;

		public bool bSendEvent = true;

		public bool bAbort;

		public bool bUseStop = true;

		public bool bUseCombo = true;

		private static List<ChangeSkillTriggerTick.SelectionData> randomList = new List<ChangeSkillTriggerTick.SelectionData>(4);

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.slotType = SkillSlotType.SLOT_SKILL_0;
			this.effectTime = 0;
			this.changeSkillID = 0;
			this.changeSkillID2 = 0;
			this.changeSkillID3 = 0;
			this.changeSkillID4 = 0;
			this.recoverSkillID = 0;
			this.changeSkillID1Probability = 100;
			this.changeSkillID2Probability = 100;
			this.changeSkillID3Probability = 100;
			this.changeSkillID4Probability = 100;
			this.bCheckCondition = false;
			this.bOvertimeCD = true;
			this.bSendEvent = true;
			this.bAbort = false;
			this.bUseCombo = true;
		}

		public override BaseEvent Clone()
		{
			ChangeSkillTriggerTick changeSkillTriggerTick = ClassObjPool<ChangeSkillTriggerTick>.Get();
			changeSkillTriggerTick.CopyData(this);
			return changeSkillTriggerTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ChangeSkillTriggerTick changeSkillTriggerTick = src as ChangeSkillTriggerTick;
			this.targetId = changeSkillTriggerTick.targetId;
			this.slotType = changeSkillTriggerTick.slotType;
			this.effectTime = changeSkillTriggerTick.effectTime;
			this.changeSkillID = changeSkillTriggerTick.changeSkillID;
			this.changeSkillID2 = changeSkillTriggerTick.changeSkillID2;
			this.changeSkillID3 = changeSkillTriggerTick.changeSkillID3;
			this.changeSkillID4 = changeSkillTriggerTick.changeSkillID4;
			this.recoverSkillID = changeSkillTriggerTick.recoverSkillID;
			this.changeSkillID1Probability = changeSkillTriggerTick.changeSkillID1Probability;
			this.changeSkillID2Probability = changeSkillTriggerTick.changeSkillID2Probability;
			this.changeSkillID3Probability = changeSkillTriggerTick.changeSkillID3Probability;
			this.changeSkillID4Probability = changeSkillTriggerTick.changeSkillID4Probability;
			this.bCheckCondition = changeSkillTriggerTick.bCheckCondition;
			this.attackTargetId = changeSkillTriggerTick.attackTargetId;
			this.bOvertimeCD = changeSkillTriggerTick.bOvertimeCD;
			this.bSendEvent = changeSkillTriggerTick.bSendEvent;
			this.bAbort = changeSkillTriggerTick.bAbort;
			this.bUseStop = changeSkillTriggerTick.bUseStop;
			this.bUseCombo = changeSkillTriggerTick.bUseCombo;
		}

		private int RandomSkillID(out int index)
		{
			ChangeSkillTriggerTick.randomList.Clear();
			index = 0;
			int num = 0;
			if (this.changeSkillID != 0 && this.changeSkillID1Probability > 0)
			{
				num += this.changeSkillID1Probability;
				ChangeSkillTriggerTick.randomList.Add(new ChangeSkillTriggerTick.SelectionData
				{
					ID = this.changeSkillID,
					ProbabilityBase = num
				});
			}
			if (this.changeSkillID2 != 0 && this.changeSkillID2Probability > 0)
			{
				num += this.changeSkillID2Probability;
				ChangeSkillTriggerTick.randomList.Add(new ChangeSkillTriggerTick.SelectionData
				{
					ID = this.changeSkillID2,
					ProbabilityBase = num
				});
			}
			if (this.changeSkillID3 != 0 && this.changeSkillID3Probability > 0)
			{
				num += this.changeSkillID3Probability;
				ChangeSkillTriggerTick.randomList.Add(new ChangeSkillTriggerTick.SelectionData
				{
					ID = this.changeSkillID3,
					ProbabilityBase = num
				});
			}
			if (this.changeSkillID4 != 0 && this.changeSkillID4Probability > 0)
			{
				num += this.changeSkillID4Probability;
				ChangeSkillTriggerTick.randomList.Add(new ChangeSkillTriggerTick.SelectionData
				{
					ID = this.changeSkillID4,
					ProbabilityBase = num
				});
			}
			if (ChangeSkillTriggerTick.randomList.get_Count() == 1)
			{
				return ChangeSkillTriggerTick.randomList.get_Item(0).ID;
			}
			if (ChangeSkillTriggerTick.randomList.get_Count() == 0)
			{
				return 0;
			}
			int num2 = (int)FrameRandom.Random((uint)num);
			for (int i = 0; i < ChangeSkillTriggerTick.randomList.get_Count(); i++)
			{
				if (num2 < ChangeSkillTriggerTick.randomList.get_Item(i).ProbabilityBase)
				{
					index = i + 1;
					return ChangeSkillTriggerTick.randomList.get_Item(i).ID;
				}
			}
			return 0;
		}

		private bool CheckChangeSkillCondition(Action _action)
		{
			if (!this.bCheckCondition)
			{
				return true;
			}
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.attackTargetId);
			return actorHandle && actorHandle.handle.ActorControl.IsDeadState;
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			SkillComponent skillControl = actorHandle.handle.SkillControl;
			if (skillControl == null)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			int num = 0;
			int num2 = this.RandomSkillID(out num);
			if (num > 0)
			{
				_action.refParams.AddRefParam("SpecifiedSkillCombineIndex", num);
			}
			if (num2 == 0)
			{
				return;
			}
			SkillSlot skillSlot;
			if (skillControl.TryGetSkillSlot(this.slotType, out skillSlot) && skillSlot != null)
			{
				if (!this.CheckChangeSkillCondition(_action))
				{
					skillSlot.StartSkillCD();
					return;
				}
				int time = this.effectTime;
				skillSlot.skillChangeEvent.Start(time, num2, this.recoverSkillID, this.bOvertimeCD, this.bSendEvent, this.bAbort, this.bUseStop, this.bUseCombo);
			}
		}
	}
}
