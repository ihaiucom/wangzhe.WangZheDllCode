using Assets.Scripts.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	[EventCategory("Utility")]
	public class SetVisibility : TickEvent
	{
		public bool enabled = true;

		public string[] excludeMeshes = new string[0];

		private Dictionary<string, bool> excludeMeshNames = new Dictionary<string, bool>();

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public override BaseEvent Clone()
		{
			SetVisibility setVisibility = ClassObjPool<SetVisibility>.Get();
			setVisibility.CopyData(this);
			return setVisibility;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SetVisibility setVisibility = src as SetVisibility;
			this.enabled = setVisibility.enabled;
			this.excludeMeshes = setVisibility.excludeMeshes;
			this.excludeMeshNames = setVisibility.excludeMeshNames;
			this.targetId = setVisibility.targetId;
		}

		public override void OnUse()
		{
			base.OnUse();
		}

		public override void Process(Action _action, Track _track)
		{
			if (this.excludeMeshNames.Count != this.excludeMeshes.Length)
			{
				string[] array = this.excludeMeshes;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					string key = text;
					this.excludeMeshNames.Add(key, true);
				}
			}
			GameObject gameObject = _action.GetGameObject(this.targetId);
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
			foreach (Transform transform in _obj.transform)
			{
				this.SetChild(transform.gameObject);
			}
		}
	}
}
