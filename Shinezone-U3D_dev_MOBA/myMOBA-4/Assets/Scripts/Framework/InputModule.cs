using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Framework
{
	public class InputModule : Singleton<InputModule>, IGameModule
	{
		public event TouchActions TouchAction;

		public override void Init()
		{
		}

		public void UpdateFrame()
		{
			this.CheckTouchState();
		}

		private void CheckTouchState()
		{
			if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}
			if (this.TouchAction != null)
			{
				for (int i = 0; i < Input.touchCount; i++)
				{
					this.TouchAction(this, new TouchEventArgs(Input.GetTouch(i), i));
				}
			}
		}
	}
}
