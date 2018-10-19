using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class GameTaskOccupy : GameTask
	{
		protected RES_BATTLE_TASK_SUBJECT SubjectType
		{
			get
			{
				return (RES_BATTLE_TASK_SUBJECT)base.Config.iParam1;
			}
		}

		protected int SourceSubj
		{
			get
			{
				return base.Config.iParam2;
			}
		}

		protected int TargetArea
		{
			get
			{
				return base.Config.iParam3;
			}
		}

		protected override void OnInitial()
		{
		}

		protected override void OnDestroy()
		{
			Singleton<TriggerEventSys>.instance.OnActorInside -= new TriggerEventDelegate(this.onActorInside);
		}

		protected override void OnStart()
		{
			Singleton<TriggerEventSys>.instance.OnActorInside += new TriggerEventDelegate(this.onActorInside);
		}

		protected override void OnClose()
		{
			Singleton<TriggerEventSys>.instance.OnActorInside -= new TriggerEventDelegate(this.onActorInside);
		}

		private void onActorInside(AreaEventTrigger sourceTrigger, object param)
		{
			if (sourceTrigger.Mark == this.TargetArea)
			{
				if (this.SubjectType == RES_BATTLE_TASK_SUBJECT.CAMP && sourceTrigger.HasActorInside((PoolObjHandle<ActorRoot> enr) => enr.handle.TheActorMeta.ActorCamp == (COM_PLAYERCAMP)this.SourceSubj))
				{
					base.Current += (int)param;
				}
				else if (this.SubjectType == RES_BATTLE_TASK_SUBJECT.ORGAN && sourceTrigger.HasActorInside((PoolObjHandle<ActorRoot> enr) => enr.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && enr.handle.TheActorMeta.ConfigId == this.SourceSubj))
				{
					base.Current += (int)param;
				}
			}
		}
	}
}
