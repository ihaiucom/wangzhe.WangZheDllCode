using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class CallActorTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int TargetId = -1;

		[AssetReference(AssetRefType.CallActorConfigId)]
		public int ConfigId;

		public int objectSpaceId = -1;

		public int tempIdId = -1;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			CallActorTick callActorTick = src as CallActorTick;
			this.TargetId = callActorTick.TargetId;
			this.ConfigId = callActorTick.ConfigId;
			this.objectSpaceId = callActorTick.objectSpaceId;
			this.tempIdId = callActorTick.tempIdId;
		}

		public override BaseEvent Clone()
		{
			CallActorTick callActorTick = ClassObjPool<CallActorTick>.Get();
			callActorTick.CopyData(this);
			return callActorTick;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.TargetId = -1;
			this.ConfigId = 0;
			this.objectSpaceId = -1;
			this.tempIdId = -1;
		}

		private void SetSoulLevel(ref PoolObjHandle<ActorRoot> actor, int level)
		{
			actor.handle.ValueComponent.actorSoulLevel = level;
		}

		private PoolObjHandle<ActorRoot> SpawnCallActor(Action _action, int configId, ref PoolObjHandle<ActorRoot> tarActor)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.objectSpaceId);
			ActorMeta actorMeta = default(ActorMeta);
			ActorMeta actorMeta2 = actorMeta;
			actorMeta2.ConfigId = configId;
			actorMeta2.HostConfigId = tarActor.handle.TheActorMeta.ConfigId;
			actorMeta2.ActorType = ActorTypeDef.Actor_Type_Call;
			actorMeta2.ActorCamp = tarActor.handle.TheActorMeta.ActorCamp;
			actorMeta2.PlayerId = tarActor.handle.TheActorMeta.PlayerId;
			actorMeta = actorMeta2;
			VInt3 pos = VInt3.zero;
			VInt3 forward = tarActor.handle.forward;
			if (actorHandle)
			{
				pos = actorHandle.handle.location;
			}
			else
			{
				pos = tarActor.handle.location;
			}
			PoolObjHandle<ActorRoot> poolObjHandle = Singleton<GameObjMgr>.instance.SpawnCallActorEx(null, ref actorMeta, pos, forward, false, true);
			poolObjHandle.handle.InitActor();
			CallActorWrapper callActorWrapper = poolObjHandle.handle.ActorControl as CallActorWrapper;
			if (callActorWrapper != null)
			{
				callActorWrapper.InitProperty(ref tarActor);
			}
			poolObjHandle.handle.PrepareFight();
			poolObjHandle.handle.StartFight();
			this.SetSoulLevel(ref poolObjHandle, tarActor.handle.ValueComponent.actorSoulLevel);
			if (callActorWrapper != null)
			{
				callActorWrapper.CopyImposterInfo();
			}
			Singleton<GameObjMgr>.instance.AddActor(poolObjHandle);
			return poolObjHandle;
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.TargetId);
			if (!actorHandle)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			if (actorHandle && actorHandle.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				HeroWrapper heroWrapper = actorHandle.handle.ActorControl as HeroWrapper;
				if (heroWrapper != null)
				{
					PoolObjHandle<ActorRoot> callActor = heroWrapper.GetCallActor();
					if (callActor)
					{
						callActor.handle.Suicide();
					}
					PoolObjHandle<ActorRoot> ptr = this.SpawnCallActor(_action, this.ConfigId, ref actorHandle);
					heroWrapper.SetCallActor(ref ptr);
					if (ptr && this.tempIdId >= 0)
					{
						_action.ExpandGameObject(this.tempIdId);
						_action.SetGameObject(this.tempIdId, ptr.handle.gameObject);
					}
				}
			}
		}
	}
}
