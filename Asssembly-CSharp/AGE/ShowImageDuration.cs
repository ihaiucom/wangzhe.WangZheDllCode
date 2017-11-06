using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class ShowImageDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int srcId;

		[ObjectTemplate(new Type[]
		{

		})]
		public int atkerId;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.srcId = 0;
			this.atkerId = 0;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ShowImageDuration showImageDuration = src as ShowImageDuration;
			this.srcId = showImageDuration.srcId;
			this.atkerId = showImageDuration.atkerId;
		}

		public override BaseEvent Clone()
		{
			ShowImageDuration showImageDuration = ClassObjPool<ShowImageDuration>.Get();
			showImageDuration.CopyData(this);
			return showImageDuration;
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			GameObject gameObject = _action.GetGameObject(this.srcId);
			GameObject gameObject2 = _action.GetGameObject(this.atkerId);
		}

		public override void Leave(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.srcId);
			GameObject gameObject2 = _action.GetGameObject(this.atkerId);
			base.Leave(_action, _track);
		}
	}
}
