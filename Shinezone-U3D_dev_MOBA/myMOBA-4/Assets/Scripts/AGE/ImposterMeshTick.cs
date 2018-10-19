using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/System")]
	public class ImposterMeshTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		private PoolObjHandle<ActorRoot> callActor;

		private PoolObjHandle<ActorRoot> imposterActor;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override BaseEvent Clone()
		{
			ImposterMeshTick imposterMeshTick = ClassObjPool<ImposterMeshTick>.Get();
			imposterMeshTick.CopyData(this);
			return imposterMeshTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ImposterMeshTick imposterMeshTick = src as ImposterMeshTick;
			this.targetId = imposterMeshTick.targetId;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			if (this.callActor)
			{
				this.callActor.Release();
			}
			if (this.imposterActor)
			{
				this.imposterActor.Release();
			}
		}

		private void ChangeMeshImpl(ref PoolObjHandle<ActorRoot> srcActor, GameObject newMesh)
		{
			if (newMesh != null)
			{
				Transform transform = newMesh.transform;
				transform.SetParent(srcActor.handle.myTransform);
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				srcActor.handle.SetActorMesh(newMesh);
			}
		}

		private string GetMeshName(ref PoolObjHandle<ActorRoot> actor)
		{
			ActorMeta actorMeta = default(ActorMeta);
			ActorMeta actorMeta2 = actorMeta;
			actorMeta2.ConfigId = actor.handle.TheActorMeta.ConfigId;
			actorMeta2.ActorCamp = actor.handle.TheActorMeta.ActorCamp;
			actorMeta2.PlayerId = actor.handle.TheActorMeta.PlayerId;
			actorMeta = actorMeta2;
			ActorStaticData actorStaticData = default(ActorStaticData);
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
			actorDataProvider.GetActorStaticData(ref actorMeta, ref actorStaticData);
			if (!string.IsNullOrEmpty(actorStaticData.TheResInfo.ResPath))
			{
				CActorInfo actorInfo = CActorInfo.GetActorInfo(actorStaticData.TheResInfo.ResPath, enResourceType.BattleScene);
				if (actorInfo != null)
				{
					string artPrefabName = actorInfo.GetArtPrefabName((int)actorMeta.SkinID, -1);
					if (!string.IsNullOrEmpty(artPrefabName))
					{
						return artPrefabName;
					}
				}
			}
			return string.Empty;
		}

		private void EnterImposterMesh(ref PoolObjHandle<ActorRoot> srcActor, ref PoolObjHandle<ActorRoot> imposter)
		{
			srcActor.handle.RecordOriginalActorMesh();
			GameObject actorMesh = srcActor.handle.ActorMesh;
			actorMesh.CustomSetActive(false);
			string meshName = this.GetMeshName(ref imposter);
			GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(meshName, false, SceneObjType.ActionRes, Vector3.zero);
			this.ChangeMeshImpl(ref srcActor, pooledGameObjLOD);
		}

		public void Enter(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				return;
			}
			HeroWrapper heroWrapper = actorHandle.handle.ActorControl as HeroWrapper;
			this.callActor = heroWrapper.GetCallActor();
			CallActorWrapper callActorWrapper = this.callActor.handle.ActorControl as CallActorWrapper;
			if (callActorWrapper != null)
			{
				this.imposterActor = callActorWrapper.GetImposterActor();
			}
			this.EnterImposterMesh(ref this.callActor, ref this.imposterActor);
		}

		public override void Process(Action _action, Track _track)
		{
			this.Enter(_action, _track);
			base.Process(_action, _track);
		}
	}
}
