using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using CSProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class GameTaskImprison : GameTask
	{
		public override float Progress
		{
			get
			{
				return 1f - (float)base.TimeRemain / (float)base.TimeLimit;
			}
		}

		protected COM_PLAYERCAMP WaitCamp
		{
			get
			{
				return (COM_PLAYERCAMP)base.Config.iParam1;
			}
		}

		protected override void OnInitial()
		{
		}

		protected override void OnDestroy()
		{
		}

		private bool FilterWaitActor(ref PoolObjHandle<ActorRoot> actor)
		{
			return actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && actor.handle.TheActorMeta.ActorCamp == this.WaitCamp;
		}

		protected override void OnStart()
		{
			List<PoolObjHandle<ActorRoot>> list = ActorHelper.FilterActors(Singleton<GameObjMgr>.instance.HeroActors, new ActorFilterDelegate(this.FilterWaitActor));
			for (int i = 0; i < list.get_Count(); i++)
			{
				list.get_Item(i).handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
			}
		}

		protected override void OnClose()
		{
			List<PoolObjHandle<ActorRoot>> list = ActorHelper.FilterActors(Singleton<GameObjMgr>.instance.HeroActors, new ActorFilterDelegate(this.FilterWaitActor));
			for (int i = 0; i < list.get_Count(); i++)
			{
				list.get_Item(i).handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
			}
		}

		protected override void OnTimeOver()
		{
			base.Current = this.Target;
		}
	}
}
