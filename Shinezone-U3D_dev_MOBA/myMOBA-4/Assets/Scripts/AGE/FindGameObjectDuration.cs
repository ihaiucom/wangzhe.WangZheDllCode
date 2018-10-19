using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("Utility")]
	public class FindGameObjectDuration : DurationEvent
	{
		[ObjectTemplate(true)]
		public int targetId = -1;

		public string objectName;

		public override BaseEvent Clone()
		{
			FindGameObjectDuration findGameObjectDuration = ClassObjPool<FindGameObjectDuration>.Get();
			findGameObjectDuration.CopyData(this);
			return findGameObjectDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			FindGameObjectDuration findGameObjectDuration = src as FindGameObjectDuration;
			this.targetId = findGameObjectDuration.targetId;
			this.objectName = findGameObjectDuration.objectName;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.objectName = null;
		}

		public override void Enter(Action _action, Track _track)
		{
			if (this.targetId == -1)
			{
				return;
			}
			_action.ExpandGameObject(this.targetId);
			if (string.IsNullOrEmpty(this.objectName))
			{
				_action.SetGameObject(this.targetId, null);
			}
			else
			{
				_action.SetGameObject(this.targetId, GameObject.Find(this.objectName));
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.targetId == -1)
			{
				return;
			}
			_action.SetGameObject(this.targetId, null);
		}
	}
}
