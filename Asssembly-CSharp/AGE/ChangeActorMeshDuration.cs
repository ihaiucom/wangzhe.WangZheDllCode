using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/System")]
	public class ChangeActorMeshDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		[AssetReference(AssetRefType.Prefab)]
		public string prefabName = string.Empty;

		public bool bUseSkin;

		private bool switchFinished;

		private GameObject actorMesh;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override BaseEvent Clone()
		{
			ChangeActorMeshDuration changeActorMeshDuration = ClassObjPool<ChangeActorMeshDuration>.Get();
			changeActorMeshDuration.CopyData(this);
			return changeActorMeshDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ChangeActorMeshDuration changeActorMeshDuration = src as ChangeActorMeshDuration;
			this.targetId = changeActorMeshDuration.targetId;
			this.prefabName = changeActorMeshDuration.prefabName;
			this.bUseSkin = changeActorMeshDuration.bUseSkin;
			this.switchFinished = changeActorMeshDuration.switchFinished;
			this.actorMesh = changeActorMeshDuration.actorMesh;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.prefabName = string.Empty;
			this.bUseSkin = false;
			this.switchFinished = false;
			this.actorMesh = null;
		}

		private void ChangeMesh(ref PoolObjHandle<ActorRoot> srcActor, GameObject newMesh)
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

		public override void Enter(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				return;
			}
			this.switchFinished = false;
			this.actorMesh = actorHandle.handle.ActorMesh;
			this.actorMesh.CustomSetActive(false);
			string resourceName;
			if (this.bUseSkin)
			{
				resourceName = SkinResourceHelper.GetResourceName(_action, this.prefabName, false);
			}
			else
			{
				resourceName = this.prefabName;
			}
			GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(resourceName, false, SceneObjType.ActionRes, Vector3.zero);
			if (pooledGameObjLOD == null && this.bUseSkin)
			{
				pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(this.prefabName, false, SceneObjType.ActionRes, Vector3.zero);
			}
			this.ChangeMesh(ref actorHandle, pooledGameObjLOD);
			base.Enter(_action, _track);
		}

		public override void Leave(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				return;
			}
			this.switchFinished = true;
			this.ChangeMesh(ref actorHandle, this.actorMesh);
			this.actorMesh.CustomSetActive(true);
			this.actorMesh = null;
			base.Leave(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.switchFinished;
		}
	}
}
