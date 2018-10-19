using Assets.Scripts.Framework;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	public class CHeroSkillStat
	{
		public void StartRecord()
		{
			this.Clear();
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnActorBuffSkillChange));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SpawnEyeEventParam>(GameSkillEventDef.Event_SpawnEye, new GameSkillEvent<SpawnEyeEventParam>(this.OnActorSpawnEye));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, new GameSkillEvent<DefaultSkillEventParam>(this.OnActorChangeSkill));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorMoveCity, new RefAction<DefaultGameEventParam>(this.OnActorMoveCity));
		}

		public void Clear()
		{
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnActorBuffSkillChange));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SpawnEyeEventParam>(GameSkillEventDef.Event_SpawnEye, new GameSkillEvent<SpawnEyeEventParam>(this.OnActorSpawnEye));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, new GameSkillEvent<DefaultSkillEventParam>(this.OnActorChangeSkill));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorMoveCity, new RefAction<DefaultGameEventParam>(this.OnActorMoveCity));
		}

		private void OnActorBuffSkillChange(ref BuffChangeEventParam prm)
		{
			if (prm.bIsAdd)
			{
				return;
			}
			if (!prm.stBuffSkill || prm.stBuffSkill.handle.skillContext == null)
			{
				return;
			}
			if (!prm.stBuffSkill.handle.skillContext.Originator || !prm.stBuffSkill.handle.skillContext.TargetActor)
			{
				return;
			}
			if (prm.stBuffSkill.handle.skillContext.SlotType < SkillSlotType.SLOT_SKILL_1 || prm.stBuffSkill.handle.skillContext.SlotType >= SkillSlotType.SLOT_SKILL_COUNT)
			{
				return;
			}
			if (prm.stBuffSkill.handle.cfgData.bEffectType != 2)
			{
				return;
			}
			if (prm.stBuffSkill.handle.cfgData.bShowType != 1 && prm.stBuffSkill.handle.cfgData.bShowType != 3 && prm.stBuffSkill.handle.cfgData.bShowType != 4 && prm.stBuffSkill.handle.cfgData.bShowType != 5 && prm.stBuffSkill.handle.cfgData.bShowType != 6)
			{
				return;
			}
			ulong num = Singleton<FrameSynchr>.GetInstance().LogicFrameTick - prm.stBuffSkill.handle.ulStartTime;
			if (prm.stBuffSkill.handle.skillContext.Originator.handle.SkillControl != null)
			{
				prm.stBuffSkill.handle.skillContext.Originator.handle.SkillControl.stSkillStat.m_uiStunTime += (uint)num;
			}
			if (prm.stBuffSkill.handle.skillContext.TargetActor.handle.SkillControl != null)
			{
				prm.stBuffSkill.handle.skillContext.TargetActor.handle.SkillControl.stSkillStat.m_uiBeStunnedTime += (uint)num;
			}
		}

		private void OnActorSpawnEye(ref SpawnEyeEventParam prm)
		{
			if (!prm.src || prm.src.handle.SkillControl == null || prm.src.handle.SkillControl.stSkillStat == null)
			{
				return;
			}
			prm.src.handle.SkillControl.stSkillStat.m_uiRealSpawnEyeTimes += 1u;
			if (prm.src.handle.SkillControl.stSkillStat.m_uiSpawnEyeTimes < 15u)
			{
				COMDT_STATISTIC_POS cOMDT_STATISTIC_POS = prm.src.handle.SkillControl.stSkillStat.stEyePostion[(int)((UIntPtr)prm.src.handle.SkillControl.stSkillStat.m_uiSpawnEyeTimes)];
				if (cOMDT_STATISTIC_POS == null)
				{
					cOMDT_STATISTIC_POS = new COMDT_STATISTIC_POS();
				}
				cOMDT_STATISTIC_POS.iTime = (int)(Singleton<FrameSynchr>.instance.LogicFrameTick / 1000uL);
				cOMDT_STATISTIC_POS.iXPos = prm.pos.x;
				cOMDT_STATISTIC_POS.iZPos = prm.pos.z;
				prm.src.handle.SkillControl.stSkillStat.m_uiSpawnEyeTimes += 1u;
			}
		}

		private void OnActorChangeSkill(ref DefaultSkillEventParam prm)
		{
			if (!prm.actor || prm.actor.handle.SkillControl == null || prm.actor.handle.SkillControl.stSkillStat == null)
			{
				return;
			}
			if (prm.slot == SkillSlotType.SLOT_SKILL_7)
			{
				prm.actor.handle.SkillControl.stSkillStat.m_uiEyeSwitchTimes += 1u;
			}
		}

		private void OnActorMoveCity(ref DefaultGameEventParam prm)
		{
			if (prm.src && prm.src.handle.SkillControl != null && prm.src.handle.SkillControl.stSkillStat != null)
			{
				prm.src.handle.SkillControl.stSkillStat.m_uiMoveCitySucessTimes += 1u;
			}
		}
	}
}
