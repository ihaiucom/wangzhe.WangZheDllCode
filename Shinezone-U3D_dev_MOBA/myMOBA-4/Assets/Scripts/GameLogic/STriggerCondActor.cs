using Assets.Scripts.Common;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public struct STriggerCondActor
	{
		public ActorTypeDef[] ActorType;

		public int[] ConfigID;

		public COM_PLAYERCAMP[] CmpType;

		public bool FilterMatch(ref PoolObjHandle<ActorRoot> inActor)
		{
			return inActor && (this.ActorType == null || this.ActorType.Length <= 0 || LinqS.Contains<ActorTypeDef>(this.ActorType, inActor.handle.TheActorMeta.ActorType)) && (this.ConfigID == null || this.ConfigID.Length <= 0 || LinqS.Contains<int>(this.ConfigID, inActor.handle.TheActorMeta.ConfigId)) && (this.CmpType == null || this.CmpType.Length <= 0 || LinqS.Contains<COM_PLAYERCAMP>(this.CmpType, inActor.handle.TheActorMeta.ActorCamp));
		}
	}
}
