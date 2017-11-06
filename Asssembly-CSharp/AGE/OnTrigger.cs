using Assets.Scripts.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	[EventCategory("Condition")]
	public class OnTrigger : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{
			typeof(Collider)
		})]
		public int targetId;

		public string scriptName = "TriggerHelper";

		public string methodName = "GetCollisionSet";

		public string[] tags = new string[0];

		public override BaseEvent Clone()
		{
			OnTrigger onTrigger = ClassObjPool<OnTrigger>.Get();
			onTrigger.CopyData(this);
			return onTrigger;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			OnTrigger onTrigger = src as OnTrigger;
			this.targetId = onTrigger.targetId;
			this.scriptName = onTrigger.scriptName;
			this.methodName = onTrigger.methodName;
			this.tags = onTrigger.tags;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.scriptName = "TriggerHelper";
			this.methodName = "GetCollisionSet";
			this.tags = new string[0];
		}

		public override bool Check(Action _action, Track _track)
		{
			GameObject gameObject = _action.GetGameObject(this.targetId);
			if (gameObject == null)
			{
				return false;
			}
			Component component = gameObject.GetComponent(this.scriptName);
			if (component == null)
			{
				return false;
			}
			Type type = component.GetType();
			object[] array = new object[0];
			List<GameObject> list = type.InvokeMember(this.methodName, 318, null, component, array) as List<GameObject>;
			if (this.tags.Length > 0)
			{
				using (List<GameObject>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GameObject current = enumerator.get_Current();
						if (!(current == null))
						{
							string[] array2 = this.tags;
							for (int i = 0; i < array2.Length; i++)
							{
								string text = array2[i];
								if (current.tag == text)
								{
									bool flag = true;
									bool result = flag;
									return result;
								}
							}
						}
					}
				}
			}
			else
			{
				using (List<GameObject>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						GameObject current2 = enumerator2.get_Current();
						if (current2 != null)
						{
							bool flag2 = true;
							bool result = flag2;
							return result;
						}
					}
				}
			}
			return false;
		}
	}
}
