using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SpawnPetDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		[AssetReference(AssetRefType.Prefab)]
		public string prefabName = string.Empty;

		public Vector3 offset;

		public PetType petType;

		private PoolObjHandle<ActorRoot> actorObj;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.prefabName = string.Empty;
			this.offset = Vector3.zero;
			this.actorObj.Release();
		}

		public override BaseEvent Clone()
		{
			SpawnPetDuration spawnPetDuration = ClassObjPool<SpawnPetDuration>.Get();
			spawnPetDuration.CopyData(this);
			return spawnPetDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SpawnPetDuration spawnPetDuration = src as SpawnPetDuration;
			this.targetId = spawnPetDuration.targetId;
			this.prefabName = spawnPetDuration.prefabName;
			this.offset = spawnPetDuration.offset;
			this.petType = spawnPetDuration.petType;
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.actorObj = _action.GetActorHandle(this.targetId);
			if (this.actorObj && this.actorObj.handle.PetControl != null)
			{
				this.actorObj.handle.PetControl.CreatePet(this.petType, this.prefabName, this.offset);
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.actorObj && this.actorObj.handle.PetControl != null)
			{
				this.actorObj.handle.PetControl.DestoryPet(this.petType);
			}
			base.Leave(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
		}
	}
}
