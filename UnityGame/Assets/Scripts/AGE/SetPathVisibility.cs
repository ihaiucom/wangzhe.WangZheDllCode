using Assets.Scripts.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class SetPathVisibility : TickEvent
	{
		public bool enabled = true;

		public string[] excludeMeshes = new string[0];

		private Dictionary<string, bool> excludeMeshNames = new Dictionary<string, bool>();

		public string objPath = string.Empty;

		public override BaseEvent Clone()
		{
			SetPathVisibility setPathVisibility = ClassObjPool<SetPathVisibility>.Get();
			setPathVisibility.CopyData(this);
			return setPathVisibility;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SetPathVisibility setPathVisibility = src as SetPathVisibility;
			this.enabled = setPathVisibility.enabled;
			this.excludeMeshes = setPathVisibility.excludeMeshes;
			this.excludeMeshNames = setPathVisibility.excludeMeshNames;
			this.objPath = setPathVisibility.objPath;
		}

		public override void OnUse()
		{
			base.OnUse();
		}

		public override void Process(Action _action, Track _track)
		{
			if (this.excludeMeshNames.get_Count() != this.excludeMeshes.Length)
			{
				string[] array = this.excludeMeshes;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					string text2 = text;
					this.excludeMeshNames.Add(text2, true);
				}
			}
			GameObject gameObject = GameObject.Find(this.objPath);
			if (gameObject == null)
			{
				return;
			}
			this.SetChild(gameObject);
		}

		private void SetChild(GameObject _obj)
		{
			string name = _obj.name;
			if (this.excludeMeshNames.ContainsKey(name))
			{
				return;
			}
			if (_obj.GetComponent<Renderer>())
			{
				_obj.GetComponent<Renderer>().enabled = this.enabled;
			}
			using (IEnumerator enumerator = _obj.transform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.get_Current();
					this.SetChild(transform.gameObject);
				}
			}
		}
	}
}
