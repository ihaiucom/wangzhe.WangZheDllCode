using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using CSProtocol;
using System;
using System.Collections.Generic;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SkillUseExposingTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int BeneficiaryId = -1;

		public int ExposeDuration = 2000;

		public int InvolveRange = 10000;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillUseExposingTick skillUseExposingTick = src as SkillUseExposingTick;
			this.targetId = skillUseExposingTick.targetId;
			this.BeneficiaryId = skillUseExposingTick.BeneficiaryId;
		}

		public override BaseEvent Clone()
		{
			SkillUseExposingTick skillUseExposingTick = ClassObjPool<SkillUseExposingTick>.Get();
			skillUseExposingTick.CopyData(this);
			return skillUseExposingTick;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.BeneficiaryId = -1;
			this.ExposeDuration = 2000;
			this.InvolveRange = 10000;
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				return;
			}
			if (this.ExposeDuration <= 0)
			{
				return;
			}
			ActorRoot handle = actorHandle.handle;
			if (handle.HorizonMarker == null)
			{
				return;
			}
			COM_PLAYERCAMP actorCamp = handle.TheActorMeta.ActorCamp;
			COM_PLAYERCAMP cOM_PLAYERCAMP = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
			PoolObjHandle<ActorRoot> actorHandle2 = _action.GetActorHandle(this.BeneficiaryId);
			if (actorHandle2)
			{
				cOM_PLAYERCAMP = actorHandle2.handle.TheActorMeta.ActorCamp;
			}
			else if (actorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				cOM_PLAYERCAMP = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
			}
			else if (actorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				cOM_PLAYERCAMP = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
			}
			if (cOM_PLAYERCAMP == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				return;
			}
			if (this.InvolveRange > 0 && !this.CheckInvolvingEnemyHero(handle, this.InvolveRange))
			{
				return;
			}
			handle.HorizonMarker.ExposeAsAttacker(cOM_PLAYERCAMP, this.ExposeDuration);
		}

		private bool CheckInvolvingEnemyHero(ActorRoot InActor, int srchR)
		{
			bool result = false;
			long num = (long)srchR * (long)srchR;
			COM_PLAYERCAMP actorCamp = InActor.TheActorMeta.ActorCamp;
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
			int count = heroActors.Count;
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> ptr = heroActors[i];
				if (ptr)
				{
					ActorRoot handle = ptr.handle;
					if (handle.TheActorMeta.ActorCamp != actorCamp)
					{
						long sqrMagnitudeLong2D = (handle.location - InActor.location).sqrMagnitudeLong2D;
						if (sqrMagnitudeLong2D < num)
						{
							result = true;
							break;
						}
					}
				}
			}
			return result;
		}
	}
}
