using AGE;
using Assets.Scripts.Common;
using CSProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionJungle : TriggerActionBase
	{
		private HashSet<uint> _deadActors;

		public TriggerActionJungle(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
			this._deadActors = new HashSet<uint>();
		}

		public override void Destroy()
		{
			this._deadActors = null;
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			ActorInGrassParam actorInGrassParam = new ActorInGrassParam(src, true);
			Singleton<GameEventSys>.instance.SendEvent<ActorInGrassParam>(GameEventDef.Event_ActorInGrass, ref actorInGrassParam);
			if (!FogOfWar.enable)
			{
				this.ModifyHorizonMarks(src, inTrigger, true);
			}
			return null;
		}

		public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			if (src && src.handle.ActorControl.IsDeadState && !this._deadActors.Contains(src.handle.ObjID))
			{
				this._deadActors.Add(src.handle.ObjID);
				if (!FogOfWar.enable)
				{
					this.ModifyHorizonMarks(src, inTrigger, false);
				}
			}
		}

		public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
		{
			if (!src)
			{
				return;
			}
			ActorInGrassParam actorInGrassParam = new ActorInGrassParam(src, false);
			Singleton<GameEventSys>.instance.SendEvent<ActorInGrassParam>(GameEventDef.Event_ActorInGrass, ref actorInGrassParam);
			if (this._deadActors.Contains(src.handle.ObjID))
			{
				this._deadActors.Remove(src.handle.ObjID);
			}
			else if (!FogOfWar.enable)
			{
				this.ModifyHorizonMarks(src, inTrigger, false);
			}
		}

		private void ModifyHorizonMarks(PoolObjHandle<ActorRoot> src, ITrigger inTrigger, bool enterOrLeave)
		{
			if (src)
			{
				int num = (!enterOrLeave) ? -1 : 1;
				AreaEventTrigger areaEventTrigger = inTrigger as AreaEventTrigger;
				List<PoolObjHandle<ActorRoot>> actors = areaEventTrigger.GetActors((PoolObjHandle<ActorRoot> enr) => enr.handle.TheActorMeta.ActorCamp != src.handle.TheActorMeta.ActorCamp);
				for (int i = 0; i < actors.Count; i++)
				{
					PoolObjHandle<ActorRoot> poolObjHandle = actors[i];
					poolObjHandle.handle.HorizonMarker.AddShowMark(src.handle.TheActorMeta.ActorCamp, HorizonConfig.ShowMark.Jungle, num * 1);
					src.handle.HorizonMarker.AddShowMark(poolObjHandle.handle.TheActorMeta.ActorCamp, HorizonConfig.ShowMark.Jungle, num * 1);
				}
				COM_PLAYERCAMP[] othersCmp = BattleLogic.GetOthersCmp(src.handle.TheActorMeta.ActorCamp);
				for (int j = 0; j < othersCmp.Length; j++)
				{
					src.handle.HorizonMarker.AddHideMark(othersCmp[j], HorizonConfig.HideMark.Jungle, num * 1, false);
				}
			}
		}
	}
}
