using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Framework
{
	public class InputModule : Singleton<InputModule>, IGameModule
	{
		public event TouchActions TouchAction
		{
			[MethodImpl(32)]
			add
			{
				this.TouchAction = (TouchActions)Delegate.Combine(this.TouchAction, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.TouchAction = (TouchActions)Delegate.Remove(this.TouchAction, value);
			}
		}

		public override void Init()
		{
		}

		public void UpdateFrame()
		{
			this.CheckTouchState();
		}

		private void CheckTouchState()
		{
			if (EventSystem.get_current() && EventSystem.get_current().IsPointerOverGameObject())
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
