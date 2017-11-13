using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public abstract class ActivityWidget
	{
		private GameObject _root;

		private ActivityView _view;

		public GameObject root
		{
			get
			{
				return this._root;
			}
		}

		public ActivityView view
		{
			get
			{
				return this._view;
			}
		}

		public virtual float Height
		{
			get
			{
				return this._root.GetComponent<RectTransform>().rect.height;
			}
		}

		public ActivityWidget(GameObject node, ActivityView view)
		{
			this._root = node;
			this._view = view;
		}

		public void SetPosY(float val)
		{
			this._root.transform.localPosition = new Vector3(this._root.transform.localPosition.x, val, this._root.transform.localPosition.z);
		}

		public abstract void Clear();

		public abstract void Validate();

		public virtual void Update()
		{
		}

		public virtual void OnShow()
		{
		}
	}
}
