using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("Utility")]
	public class SetObjActiveTick : TickEvent
	{
		public bool enabled = true;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public override BaseEvent Clone()
		{
			SetObjActiveTick setObjActiveTick = ClassObjPool<SetObjActiveTick>.Get();
			setObjActiveTick.CopyData(this);
			return setObjActiveTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SetObjActiveTick setObjActiveTick = src as SetObjActiveTick;
			this.enabled = setObjActiveTick.enabled;
			this.targetId = setObjActiveTick.targetId;
		}

		public override void OnUse()
		{
			base.OnUse();
		}

		public override void Process(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.targetId);
			if (gameObject == null)
			{
				return;
			}
			gameObject.SetActive(this.enabled);
		}
	}
}
