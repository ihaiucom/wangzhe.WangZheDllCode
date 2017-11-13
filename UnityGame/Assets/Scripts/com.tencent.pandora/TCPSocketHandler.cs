using System;

namespace com.tencent.pandora
{
	public abstract class TCPSocketHandler
	{
		private long theUniqueSocketId;

		public void SetUniqueSocketId(long uniqueSocketId)
		{
			this.theUniqueSocketId = uniqueSocketId;
		}

		public long GetUniqueSocketId()
		{
			return this.theUniqueSocketId;
		}

		public virtual void OnConnected()
		{
		}

		public abstract int DetectPacketSize(byte[] receivedData, int dataLen);

		public virtual void OnReceived(Packet thePacket)
		{
		}

		public virtual void OnSent(Packet thePacket)
		{
		}

		public virtual void OnClose()
		{
		}
	}
}
