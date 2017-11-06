using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class MiniMapTrackDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		[AssetReference(AssetRefType.Prefab)]
		public string iconPath = string.Empty;

		private PoolObjHandle<ActorRoot> actorObj;

		private uint actorObjID;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.actorObj.Release();
		}

		public override BaseEvent Clone()
		{
			MiniMapTrackDuration miniMapTrackDuration = ClassObjPool<MiniMapTrackDuration>.Get();
			miniMapTrackDuration.CopyData(this);
			return miniMapTrackDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			MiniMapTrackDuration miniMapTrackDuration = src as MiniMapTrackDuration;
			this.targetId = miniMapTrackDuration.targetId;
			this.iconPath = miniMapTrackDuration.iconPath;
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.actorObj = _action.GetActorHandle(this.targetId);
			if (!this.actorObj)
			{
				return;
			}
			this.actorObjID = this.actorObj.handle.ObjID;
			MiniMapTrack_3DUI.Prepare(this.actorObj, this.iconPath);
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
			MiniMapTrack_3DUI.Recyle(this.actorObjID);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
			if (!this.actorObj)
			{
				return;
			}
			MiniMapTrack_3DUI.SetTrackPosition(this.actorObj, this.iconPath);
		}
	}
}
