using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	[VoiceInteraction(3)]
	public class VoiceInteractionEncounterEnemy : VoiceInteraction
	{
		public override void Init(ResVoiceInteraction InInteractionCfg)
		{
			base.Init(InInteractionCfg);
			Singleton<CTimerManager>.instance.AddTimer(1000, 0, new CTimer.OnTimeUpHandler(this.OnCheckEncounter), false);
		}

		public override void Unit()
		{
			Singleton<CTimerManager>.instance.RemoveTimer(new CTimer.OnTimeUpHandler(this.OnCheckEncounter), false);
			base.Unit();
		}

		private void OnCheckEncounter(int TimeSeq)
		{
			if (!this.ForwardCheck())
			{
				return;
			}
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
			List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = heroActors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PoolObjHandle<ActorRoot> current = enumerator.Current;
				if (current && current.handle.TheActorMeta.ConfigId == base.groupID && current.handle.HorizonMarker != null && this.CheckActor(ref current))
				{
					return;
				}
			}
		}

		private bool CheckActor(ref PoolObjHandle<ActorRoot> InTestActor)
		{
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
			List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = heroActors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PoolObjHandle<ActorRoot> current = enumerator.Current;
				if (!(current == InTestActor) && current && current.handle.HorizonMarker != null && !current.handle.IsSelfCamp(InTestActor) && base.ValidateTriggerActor(ref current) && current.handle.HorizonMarker.IsVisibleFor(InTestActor.handle.TheActorMeta.ActorCamp) && InTestActor.handle.HorizonMarker.IsVisibleFor(current.handle.TheActorMeta.ActorCamp))
				{
					if (this.CheckTriggerDistance(ref InTestActor, ref current))
					{
						this.TryTrigger(ref InTestActor, ref current, ref InTestActor);
						return true;
					}
				}
			}
			return false;
		}
	}
}
