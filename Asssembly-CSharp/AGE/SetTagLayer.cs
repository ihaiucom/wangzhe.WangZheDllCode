using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("Utility")]
	public class SetTagLayer : TickEvent
	{
		[ObjectTemplate(true)]
		public int targetId = -1;

		public bool enableLayer;

		public int layer;

		public bool enableTag;

		public string tag = string.Empty;

		public override BaseEvent Clone()
		{
			SetTagLayer setTagLayer = ClassObjPool<SetTagLayer>.Get();
			setTagLayer.CopyData(this);
			return setTagLayer;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SetTagLayer setTagLayer = src as SetTagLayer;
			this.targetId = setTagLayer.targetId;
			this.enableLayer = setTagLayer.enableLayer;
			this.layer = setTagLayer.layer;
			this.enableTag = setTagLayer.enableTag;
			this.tag = setTagLayer.tag;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.enableLayer = false;
			this.layer = 0;
			this.enableTag = false;
			this.tag = string.Empty;
		}

		public override void Process(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.targetId);
			if (gameObject == null)
			{
				Debug.LogWarning("not find setting layer/tag target object");
				return;
			}
			if (this.enableLayer)
			{
				gameObject.layer = this.layer;
				Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].gameObject.layer = this.layer;
				}
			}
			if (this.enableTag)
			{
				gameObject.tag = this.tag;
				Transform[] componentsInChildren2 = gameObject.GetComponentsInChildren<Transform>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].gameObject.tag = this.tag;
				}
			}
		}
	}
}
