using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace AGE
{
	public class GetSubObjectTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int parentId = -1;

		public bool isGetByName;

		public string subObjectName = "Mesh";

		public override BaseEvent Clone()
		{
			GetSubObjectTick getSubObjectTick = ClassObjPool<GetSubObjectTick>.Get();
			getSubObjectTick.CopyData(this);
			return getSubObjectTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			GetSubObjectTick getSubObjectTick = src as GetSubObjectTick;
			this.targetId = getSubObjectTick.targetId;
			this.parentId = getSubObjectTick.parentId;
			this.isGetByName = getSubObjectTick.isGetByName;
			this.subObjectName = getSubObjectTick.subObjectName;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.parentId = -1;
			this.isGetByName = false;
			this.subObjectName = "Mesh";
		}

		public override void Process(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.parentId);
			if (gameObject == null)
			{
				return;
			}
			GameObject gameObject2 = _action.GetGameObject(this.targetId);
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
}
