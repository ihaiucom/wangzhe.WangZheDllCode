using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class GameTaskDestroy : GameTask
	{
		protected RES_BATTLE_TASK_SUBJECT SubjectType
		{
			get
			{
				return (RES_BATTLE_TASK_SUBJECT)base.Config.iParam1;
			}
		}

		protected int SubjectID
		{
			get
			{
				return base.Config.iParam2;
			}
		}

		public override float Progress
		{
			get
			{
				if (this.SubjectType == RES_BATTLE_TASK_SUBJECT.ORGAN || this.SubjectType == RES_BATTLE_TASK_SUBJECT.MONSTER)
				{
					PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.GetActor(new ActorFilterDelegate(this.FilterTargetActor));
					if (actor)
					{
						return base.Progress + (1f - (float)actor.handle.ValueComponent.actorHp / (float)actor.handle.ValueComponent.actorHpTotal) / (float)this.Target;
					}
				}
				return base.Progress;
			}
		}

		protected override void OnInitial()
		{
		}

		protected override void OnDestroy()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
		}

		protected override void OnStart()
		{
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
		}

		protected override void OnClose()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
		}

		private bool FilterTargetActor(ref PoolObjHandle<ActorRoot> acr)
		{
			bool flag = true;
			if (this.SubjectType == RES_BATTLE_TASK_SUBJECT.ORGAN)
			{
				flag &= (acr.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ);
			}
			else if (this.SubjectType == RES_BATTLE_TASK_SUBJECT.MONSTER)
			{
				flag &= (acr.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster);
			}
			return flag & this.SubjectID == acr.handle.TheActorMeta.ConfigId;
		}

		private void onActorDead(ref GameDeadEventParam prm)
		{
			if ((this.SubjectType == RES_BATTLE_TASK_SUBJECT.ORGAN || this.SubjectType == RES_BATTLE_TASK_SUBJECT.MONSTER) && this.SubjectID == prm.src.handle.TheActorMeta.ConfigId)
			{
				base.Current++;
			}
		}
	}
}
