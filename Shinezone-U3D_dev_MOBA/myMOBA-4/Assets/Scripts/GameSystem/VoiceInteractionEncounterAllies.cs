using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	[VoiceInteraction(2)]
	public class VoiceInteractionEncounterAllies : VoiceInteraction
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
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain || hostPlayer.Captain.handle.HorizonMarker == null)
			{
				return;
			}
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
			List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = heroActors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PoolObjHandle<ActorRoot> current = enumerator.Current;
				if (current && current.handle.TheActorMeta.ConfigId == base.groupID && current.handle.HorizonMarker != null && current.handle.HorizonMarker.IsVisibleFor(hostPlayer.Captain.handle.TheActorMeta.ActorCamp) && this.CheckActor(ref current, ref hostPlayer.Captain))
				{
					return;
				}
			}
		}

		private bool CheckActor(ref PoolObjHandle<ActorRoot> InTestActor, ref PoolObjHandle<ActorRoot> HostActor)
		{
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
			List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = heroActors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PoolObjHandle<ActorRoot> current = enumerator.Current;
				if (!(current == InTestActor) && current && current.handle.IsSelfCamp(InTestActor) && current.handle.HorizonMarker != null && base.ValidateTriggerActor(ref current) && current.handle.HorizonMarker.IsVisibleFor(HostActor.handle.TheActorMeta.ActorCamp))
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
