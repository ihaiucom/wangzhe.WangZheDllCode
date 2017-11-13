using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("Utility")]
	public class GetSubObjectDuration : DurationEvent
	{
		[ObjectTemplate(true)]
		public int targetId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int parentId = -1;

		public bool isGetByName;

		public string subObjectName = string.Empty;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override BaseEvent Clone()
		{
			GetSubObjectDuration getSubObjectDuration = ClassObjPool<GetSubObjectDuration>.Get();
			getSubObjectDuration.CopyData(this);
			return getSubObjectDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			GetSubObjectDuration getSubObjectDuration = src as GetSubObjectDuration;
			this.targetId = getSubObjectDuration.targetId;
			this.parentId = getSubObjectDuration.parentId;
			this.isGetByName = getSubObjectDuration.isGetByName;
			this.subObjectName = getSubObjectDuration.subObjectName;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.parentId = -1;
			this.isGetByName = false;
			this.subObjectName = string.Empty;
		}

		public override void Enter(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.parentId);
			if (gameObject == null)
			{
				return;
			}
			if (this.targetId >= 0)
			{
				_action.ExpandGameObject(this.targetId);
				GameObject gameObject2 = _action.GetGameObject(this.targetId);
				if (gameObject2 != null)
				{
					return;
				}
				if (this.isGetByName)
				{
					Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						if (componentsInChildren[i].gameObject.name == this.subObjectName)
						{
							gameObject2 = componentsInChildren[i].gameObject;
							break;
						}
					}
				}
				else if (gameObject.transform.childCount > 0)
				{
					gameObject2 = gameObject.transform.GetChild(0).gameObject;
				}
				_action.SetGameObject(this.targetId, gameObject2);
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.targetId >= 0 && _action.GetGameObject(this.targetId))
			{
				_action.SetGameObject(this.targetId, null);
			}
		}
	}
}
