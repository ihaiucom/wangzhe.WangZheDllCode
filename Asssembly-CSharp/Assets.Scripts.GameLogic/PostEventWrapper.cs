using Assets.Scripts.Framework;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct PostEventWrapper<T> : IPostEventWrapper
	{
		public T prm;

		public uint frameNum;

		public RefAction<T> callback;

		public PostEventWrapper(RefAction<T> _call, T _prm, uint _delayFrame = 1u)
		{
			this.prm = _prm;
			this.frameNum = Singleton<FrameSynchr>.instance.CurFrameNum + _delayFrame;
			this.callback = _call;
		}

		public uint GetFrameNum()
		{
			return this.frameNum;
		}

		public void ExecCommand()
		{
			if (this.callback != null)
			{
				this.callback(ref this.prm);
			}
		}
	}
}
