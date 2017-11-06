using System;

namespace Assets.Scripts.GameLogic
{
	public struct PlayAnimParam
	{
		public string animName;

		public bool cancelCurrent;

		public bool cancelAll;

		public int layer;

		public float blendTime;

		public bool loop;

		public bool bNoTimeScale;

		public float speed;

		public bool forceOutOfStack;
	}
}
