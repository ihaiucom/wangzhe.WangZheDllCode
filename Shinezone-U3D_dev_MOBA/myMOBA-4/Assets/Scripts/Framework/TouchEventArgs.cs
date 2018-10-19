using System;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class TouchEventArgs : EventArgs
	{
		private Touch TouchEvent;

		private int Index;

		public virtual int index
		{
			get
			{
				return this.Index;
			}
		}

		public virtual Vector2 deltaPosition
		{
			get
			{
				return this.TouchEvent.deltaPosition;
			}
		}

		public virtual float deltaTime
		{
			get
			{
				return this.TouchEvent.deltaTime;
			}
		}

		public virtual int fingerId
		{
			get
			{
				return this.TouchEvent.fingerId;
			}
		}

		public virtual int tapCount
		{
			get
			{
				return this.TouchEvent.tapCount;
			}
		}

		public virtual Vector2 position
		{
			get
			{
				return this.TouchEvent.position;
			}
		}

		public virtual TouchPhase phase
		{
			get
			{
				return this.TouchEvent.phase;
			}
		}

		public TouchEventArgs(Touch InTouch, int InIndex)
		{
			this.TouchEvent = InTouch;
			this.Index = InIndex;
		}
	}
}
