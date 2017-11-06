using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("Utility")]
	public class PlaySubAction : DurationEvent
	{
		[AssetReference(AssetRefType.Action)]
		public string actionName = string.Empty;

		public int[] gameObjectIds = new int[0];

		private GameObject[] gameObjects;

		private Action subAction;

		public override BaseEvent Clone()
		{
			PlaySubAction playSubAction = ClassObjPool<PlaySubAction>.Get();
			playSubAction.CopyData(this);
			return playSubAction;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			PlaySubAction playSubAction = src as PlaySubAction;
			this.actionName = playSubAction.actionName;
			this.gameObjectIds = playSubAction.gameObjectIds;
			this.gameObjects = playSubAction.gameObjects;
			this.subAction = playSubAction.subAction;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.actionName = string.Empty;
			this.gameObjectIds = new int[0];
			this.gameObjects = null;
			this.subAction = null;
		}

		public override void Enter(Action _action, Track _track)
		{
			if (this.subAction != null)
			{
				this.subAction.Stop(false);
			}
			if (this.gameObjects == null)
			{
				this.gameObjects = new GameObject[this.gameObjectIds.Length];
				for (int i = 0; i < this.gameObjectIds.Length; i++)
				{
					this.gameObjects[i] = _action.GetGameObject(this.gameObjectIds[i]);
				}
			}
			this.subAction = ActionManager.Instance.PlaySubAction(_action, this.actionName, (float)this.length, this.gameObjects);
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.subAction != null)
			{
				this.subAction.Stop(false);
				this.subAction = null;
			}
		}
	}
}
