using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("Utility")]
	public class DestroyObject : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public override BaseEvent Clone()
		{
			DestroyObject destroyObject = ClassObjPool<DestroyObject>.Get();
			destroyObject.CopyData(this);
			return destroyObject;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			DestroyObject destroyObject = src as DestroyObject;
			this.targetId = destroyObject.targetId;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
		}

		public override void Process(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.targetId);
			if (gameObject == null)
			{
				return;
			}
			ActionManager.Instance.DestroyObject(gameObject);
		}
	}
}
