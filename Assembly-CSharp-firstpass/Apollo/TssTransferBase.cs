using System;

namespace Apollo
{
	public abstract class TssTransferBase
	{
		internal ITssService tssService;

		public abstract bool IsConnected();

		public abstract void OnTssDataCollected(byte[] data);

		public void SetRecvedTssData(byte[] data, int len = 0)
		{
			TssService tssService = this.tssService as TssService;
			if (tssService != null)
			{
				tssService.SetAntiData(data, len);
			}
		}
	}
}
