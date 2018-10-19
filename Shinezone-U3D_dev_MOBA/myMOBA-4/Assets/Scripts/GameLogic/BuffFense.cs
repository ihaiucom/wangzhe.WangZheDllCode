using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public class BuffFense
	{
		public PoolObjHandle<BuffSkill> buffPtr = default(PoolObjHandle<BuffSkill>);

		public BuffFense(BuffSkill inBuff)
		{
			this.buffPtr = new PoolObjHandle<BuffSkill>(inBuff);
		}

		public void Stop()
		{
			if (this.buffPtr)
			{
				this.buffPtr.handle.Stop();
				this.buffPtr.Release();
			}
		}
	}
}
