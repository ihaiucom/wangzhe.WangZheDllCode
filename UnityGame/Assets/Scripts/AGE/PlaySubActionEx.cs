using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class PlaySubActionEx : DurationEvent
	{
		[AssetReference(AssetRefType.Action)]
		public string actionName = string.Empty;

		public bool pushGameObjs;

		public bool pushRefParams;

		private Action subAction;

		public override void OnUse()
		{
			base.OnUse();
			this.actionName = string.Empty;
			this.pushGameObjs = false;
			this.pushRefParams = false;
			this.subAction = null;
		}

		public override BaseEvent Clone()
		{
			PlaySubActionEx playSubActionEx = ClassObjPool<PlaySubActionEx>.Get();
			playSubActionEx.CopyData(this);
			return playSubActionEx;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			PlaySubActionEx playSubActionEx = src as PlaySubActionEx;
			this.actionName = playSubActionEx.actionName;
			this.pushGameObjs = playSubActionEx.pushGameObjs;
			this.pushRefParams = playSubActionEx.pushRefParams;
			this.subAction = playSubActionEx.subAction;
		}

		public override void Enter(Action _action, Track _track)
		{
			if (this.subAction != null)
			{
				this.subAction.Stop(false);
			}
			if (this.pushGameObjs)
			{
				this.subAction = ActionManager.Instance.PlaySubAction(_action, this.actionName, (float)this.length, _action.GetGameObjectList().ToArray());
			}
			else
			{
				this.subAction = ActionManager.Instance.PlaySubAction(_action, this.actionName, (float)this.length, new GameObject[0]);
			}
			if (this.subAction == null)
			{
				return;
			}
			if (this.pushRefParams)
			{
				this.subAction.InheritRefParams(_action);
			}
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
