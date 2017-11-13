using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using System;

namespace Assets.Scripts.GameLogic
{
	public class HostPlayerLogic : IUpdateLogic
	{
		public enum Place
		{
			Far,
			Near,
			In
		}

		private Player _hostPlayer;

		private AreaCheck _areaCheck;

		public HostPlayerLogic()
		{
			this._hostPlayer = null;
			this._areaCheck = null;
		}

		public void FightStart()
		{
			this._hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			this._areaCheck = new AreaCheck(new ActorFilterDelegate(this.AroundTowerFilter), new AreaCheck.ActorProcess(this.ActorMarkProcess), Singleton<GameObjMgr>.instance.OrganActors);
		}

		public void FightOver()
		{
			this._hostPlayer = null;
			this._areaCheck = null;
		}

		private void ActorMarkProcess(PoolObjHandle<ActorRoot> inActor, AreaCheck.ActorAction action)
		{
			if (!inActor)
			{
				return;
			}
			OrganWrapper organWrapper = inActor.handle.ActorControl as OrganWrapper;
			if (organWrapper == null)
			{
				return;
			}
			switch (action)
			{
			case AreaCheck.ActorAction.Enter:
			case AreaCheck.ActorAction.Hover:
			{
				long num = inActor.handle.SkillControl.GetSkillSlot(SkillSlotType.SLOT_SKILL_0).SkillObj.cfgData.iMaxAttackDistance;
				if (action == AreaCheck.ActorAction.Enter)
				{
					organWrapper.ShowAroundEffect(OrganAroundEffect.HostPlayerNear, true, true, (float)num / 10000f);
				}
				else
				{
					long sqrMagnitudeLong2D = (inActor.handle.location - this._hostPlayer.Captain.handle.location).sqrMagnitudeLong2D;
					HostPlayerLogic.Place place = (sqrMagnitudeLong2D >= num * num) ? HostPlayerLogic.Place.Near : HostPlayerLogic.Place.In;
					bool flag = organWrapper.myTarget == this._hostPlayer.Captain;
					organWrapper.ShowAroundEffect((place == HostPlayerLogic.Place.Near) ? OrganAroundEffect.HostPlayerNear : (flag ? OrganAroundEffect.HostPlayerInAndHit : OrganAroundEffect.HostPlayerInNotHit), true, true, (float)num / 10000f);
					if (place == HostPlayerLogic.Place.In && flag)
					{
						Singleton<EventRouter>.GetInstance().BroadCastEvent("NewbieHostPlayerInAndHitByTower");
					}
				}
				break;
			}
			case AreaCheck.ActorAction.Leave:
				organWrapper.ShowAroundEffect(OrganAroundEffect.HostPlayerNear, false, true, 1f);
				break;
			}
		}

		public void UpdateLogic(int delta)
		{
			if (this._areaCheck != null)
			{
				this._areaCheck.UpdateLogic(3u);
			}
		}

		private bool AroundTowerFilter(ref PoolObjHandle<ActorRoot> actor)
		{
			if (actor && actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && (actor.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 1 || actor.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2 || actor.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 4 || actor.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 13) && this._hostPlayer != null && this._hostPlayer.Captain && actor.handle.TheActorMeta.ActorCamp != this._hostPlayer.Captain.handle.TheActorMeta.ActorCamp && !actor.handle.ActorControl.IsDeadState && !this._hostPlayer.Captain.handle.ActorControl.IsDeadState)
			{
				SkillSlot skillSlot = actor.handle.SkillControl.GetSkillSlot(SkillSlotType.SLOT_SKILL_0);
				if (skillSlot != null)
				{
					long num = (long)(skillSlot.SkillObj.cfgData.iMaxAttackDistance * 135 / 100);
					return (actor.handle.location - this._hostPlayer.Captain.handle.location).sqrMagnitudeLong2D < num * num;
				}
			}
			return false;
		}
	}
}
