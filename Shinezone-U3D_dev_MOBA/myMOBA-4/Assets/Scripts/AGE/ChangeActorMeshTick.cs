using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/System")]
	public class ChangeActorMeshTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		[AssetReference(AssetRefType.Prefab)]
		public string prefabName = string.Empty;

		public override BaseEvent Clone()
		{
			ChangeActorMeshTick changeActorMeshTick = ClassObjPool<ChangeActorMeshTick>.Get();
			changeActorMeshTick.CopyData(this);
			return changeActorMeshTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ChangeActorMeshTick changeActorMeshTick = src as ChangeActorMeshTick;
			this.targetId = changeActorMeshTick.targetId;
			this.prefabName = changeActorMeshTick.prefabName;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.prefabName = string.Empty;
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				return;
			}
			GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(this.prefabName, false, SceneObjType.ActionRes, Vector3.zero);
			if (pooledGameObjLOD != null)
			{
				Transform transform = pooledGameObjLOD.transform;
				transform.SetParent(actorHandle.handle.myTransform);
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				actorHandle.handle.SetActorMesh(pooledGameObjLOD);
			}
		}
	}
}
