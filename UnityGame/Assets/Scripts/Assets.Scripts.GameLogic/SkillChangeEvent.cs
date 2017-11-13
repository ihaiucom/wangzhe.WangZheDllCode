using System;

namespace Assets.Scripts.GameLogic
{
	public class SkillChangeEvent
	{
		private bool bActive;

		private SkillSlot curSkillSlot;

		private int effectMaxTime;

		private int effectTime;

		private int changeSkillID;

		private int recoverSkillID;

		private bool bOvertimeCD;

		private bool bSendEvent;

		private bool bAbortChange;

		private bool bUseStop;

		private bool bUseCombo;

		public SkillChangeEvent(SkillSlot slot)
		{
			this.curSkillSlot = slot;
		}

		public int GetEffectTIme()
		{
			return this.effectTime;
		}

		public int GetEffectMaxTime()
		{
			return this.effectMaxTime;
		}

		public bool IsDisplayUI()
		{
			return this.bSendEvent;
		}

		public bool IsActive()
		{
			return this.bActive;
		}

		public void Start(int _time, int _changeId, int _recoverId, bool _bOvertimeCD, bool _bSendEvent, bool _bAbort, bool _bUseStop, bool _bUseCombo)
		{
			this.effectTime = _time;
			this.effectMaxTime = _time;
			this.changeSkillID = _changeId;
			this.recoverSkillID = _recoverId;
			this.bOvertimeCD = _bOvertimeCD;
			this.bAbortChange = _bAbort;
			this.bSendEvent = _bSendEvent;
			this.bUseStop = _bUseStop;
			this.bUseCombo = _bUseCombo;
			this.Enter();
		}

		public void Stop()
		{
			if (this.bUseStop)
			{
				this.bActive = false;
			}
		}

		public void UpdateSkillCD(int nDelta)
		{
			if (this.bActive)
			{
				this.effectTime -= nDelta;
				if (this.effectTime < 0)
				{
					this.Leave();
				}
			}
		}

		public void Abort()
		{
			if (this.bAbortChange)
			{
				this.Leave();
			}
		}

		private void Enter()
		{
			Skill skill = new Skill(this.changeSkillID);
			if (this.curSkillSlot.SlotType != SkillSlotType.SLOT_SKILL_0)
			{
				this.curSkillSlot.skillIndicator.UnInitIndicatePrefab(true);
				this.curSkillSlot.skillIndicator.CreateIndicatePrefab(skill);
			}
			this.curSkillSlot.NextSkillObj = skill;
			this.bActive = true;
			ChangeSkillEventParam changeSkillEventParam = new ChangeSkillEventParam(this.curSkillSlot.SlotType, this.changeSkillID, this.effectTime, this.bUseCombo);
			if (this.bSendEvent)
			{
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<ChangeSkillEventParam>(GameSkillEventDef.Event_ChangeSkill, this.curSkillSlot.Actor, ref changeSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
			}
		}

		public void Leave()
		{
			this.bActive = false;
			if (this.recoverSkillID == 0)
			{
				return;
			}
			Skill skill = new Skill(this.recoverSkillID);
			if (this.curSkillSlot.SlotType != SkillSlotType.SLOT_SKILL_0)
			{
				this.curSkillSlot.skillIndicator.UnInitIndicatePrefab(true);
				this.curSkillSlot.skillIndicator.CreateIndicatePrefab(skill);
			}
			this.curSkillSlot.NextSkillObj = skill;
			if (this.bOvertimeCD)
			{
				this.curSkillSlot.StartSkillCD();
			}
			if (this.bSendEvent)
			{
				DefaultSkillEventParam defaultSkillEventParam = new DefaultSkillEventParam(this.curSkillSlot.SlotType, this.recoverSkillID, this.curSkillSlot.Actor);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_RecoverSkill, this.curSkillSlot.Actor, ref defaultSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
			}
			this.recoverSkillID = 0;
		}

		public void Reset()
		{
			this.bActive = false;
			this.bAbortChange = false;
			if (this.recoverSkillID == 0)
			{
				return;
			}
			this.curSkillSlot.NextSkillObj = null;
			this.curSkillSlot.SkillObj = this.curSkillSlot.InitSkillObj;
			this.curSkillSlot.skillIndicator.UnInitIndicatePrefab(true);
			this.curSkillSlot.skillIndicator.CreateIndicatePrefab(this.curSkillSlot.SkillObj);
		}
	}
}
