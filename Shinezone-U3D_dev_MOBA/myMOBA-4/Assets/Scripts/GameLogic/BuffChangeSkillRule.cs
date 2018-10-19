using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using System;

namespace Assets.Scripts.GameLogic
{
	public class BuffChangeSkillRule
	{
		private BuffHolderComponent buffHolder;

		private PoolObjHandle<ActorRoot> sourceActor;

		private ChangeSkillSlot[] changeSkillSlot = new ChangeSkillSlot[10];

		public void Init(BuffHolderComponent _buffHolder)
		{
			this.buffHolder = _buffHolder;
			this.sourceActor = _buffHolder.actorPtr;
			for (int i = 0; i < 10; i++)
			{
				this.changeSkillSlot[i].changeCount = 0;
				this.changeSkillSlot[i].initSkillID = 0;
				this.changeSkillSlot[i].changeSkillID = 0;
			}
		}

		public bool GetChangeSkillSlot(int _slotType, out int _changeSkillID)
		{
			if (this.changeSkillSlot[_slotType].changeCount > 0)
			{
				_changeSkillID = this.changeSkillSlot[_slotType].changeSkillID;
				if (_changeSkillID != 0)
				{
					return true;
				}
			}
			_changeSkillID = 0;
			return false;
		}

		public void ChangeSkillSlot(SkillSlotType _slotType, int _skillID, int _orgSkillID = 0)
		{
			int num = 0;
			int num2 = 0;
			SkillSlot skillSlot = null;
			if (this.sourceActor.handle.SkillControl.TryGetSkillSlot(_slotType, out skillSlot))
			{
				int num3 = 0;
				if (_slotType == SkillSlotType.SLOT_SKILL_7)
				{
					SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
					if (curLvelContext != null)
					{
						if ((long)this.sourceActor.handle.SkillControl.ornamentFirstSwitchCdEftTime >= (long)Singleton<FrameSynchr>.instance.LogicFrameTick)
						{
							num3 = curLvelContext.m_ornamentFirstSwitchCd;
						}
						else
						{
							num3 = curLvelContext.m_ornamentSwitchCD;
						}
						this.sourceActor.handle.SkillControl.ornamentFirstSwitchCdEftTime = 0;
					}
				}
				else
				{
					num3 = skillSlot.CurSkillCD;
				}
				int skillLevel = skillSlot.GetSkillLevel();
				if (skillSlot.SkillObj != null)
				{
					num = skillSlot.SkillObj.SkillID;
				}
				if (skillSlot.PassiveSkillObj != null)
				{
					num2 = skillSlot.PassiveSkillObj.SkillID;
				}
				if (_orgSkillID != 0 && num != _orgSkillID)
				{
					return;
				}
				skillSlot.DestoryIndicatePrefab();
				this.sourceActor.handle.SkillControl.InitSkillSlot((int)_slotType, _skillID, num2);
				if (this.sourceActor.handle.SkillControl.TryGetSkillSlot(_slotType, out skillSlot))
				{
					skillSlot.CurSkillCD = num3;
					skillSlot.IsCDReady = (num3 == 0);
					skillSlot.SetSkillLevel(skillLevel);
					DefaultSkillEventParam defaultSkillEventParam = new DefaultSkillEventParam(_slotType, 0, this.sourceActor);
					Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, this.sourceActor, ref defaultSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
					if (this.changeSkillSlot[(int)_slotType].changeCount == 0)
					{
						this.changeSkillSlot[(int)_slotType].initSkillID = num;
						this.changeSkillSlot[(int)_slotType].initPassiveSkillID = num2;
					}
					this.changeSkillSlot[(int)_slotType].changeSkillID = _skillID;
					ChangeSkillSlot[] expr_1C2_cp_0 = this.changeSkillSlot;
					expr_1C2_cp_0[(int)_slotType].changeCount = expr_1C2_cp_0[(int)_slotType].changeCount + 1;
				}
			}
		}

		public void RecoverSkillSlot(SkillSlotType _slotType)
		{
			SkillSlot skillSlot = null;
			if (this.sourceActor.handle.SkillControl.TryGetSkillSlot(_slotType, out skillSlot))
			{
				if (this.changeSkillSlot[(int)_slotType].changeCount == 1)
				{
					int initSkillID = this.changeSkillSlot[(int)_slotType].initSkillID;
					int initPassiveSkillID = this.changeSkillSlot[(int)_slotType].initPassiveSkillID;
					int num = skillSlot.CurSkillCD;
					int skillLevel = skillSlot.GetSkillLevel();
					skillSlot.DestoryIndicatePrefab();
					this.sourceActor.handle.SkillControl.InitSkillSlot((int)_slotType, initSkillID, initPassiveSkillID);
					if (this.sourceActor.handle.SkillControl.TryGetSkillSlot(_slotType, out skillSlot))
					{
						skillSlot.SetSkillLevel(skillLevel);
						skillSlot.CurSkillCD = num;
						skillSlot.IsCDReady = (num == 0);
						DefaultSkillEventParam defaultSkillEventParam = new DefaultSkillEventParam(_slotType, 0, this.sourceActor);
						Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, this.sourceActor, ref defaultSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
					}
					this.changeSkillSlot[(int)_slotType].initSkillID = 0;
					this.changeSkillSlot[(int)_slotType].changeSkillID = 0;
					this.changeSkillSlot[(int)_slotType].initPassiveSkillID = 0;
				}
				ChangeSkillSlot[] expr_136_cp_0 = this.changeSkillSlot;
				expr_136_cp_0[(int)_slotType].changeCount = expr_136_cp_0[(int)_slotType].changeCount - 1;
			}
		}
	}
}
