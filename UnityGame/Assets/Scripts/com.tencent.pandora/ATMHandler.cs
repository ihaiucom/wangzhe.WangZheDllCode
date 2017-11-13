using System;
using System.Collections.Generic;
using System.Net;

namespace com.tencent.pandora
{
	public class ATMHandler : TCPSocketHandler
	{
		private Action<int, Dictionary<string, object>> statusChangedAction;

		public ATMHandler(Action<int, Dictionary<string, object>> statusChangedAction)
		{
			this.statusChangedAction = statusChangedAction;
		}

		public override void OnConnected()
		{
			Logger.DEBUG(string.Empty);
			Message message = new Message();
			message.status = 0;
			message.content.set_Item("uniqueSocketId", base.GetUniqueSocketId());
			message.action = this.statusChangedAction;
			Pandora.Instance.GetNetLogic().EnqueueResult(message);
		}

		public override int DetectPacketSize(byte[] receivedData, int dataLen)
		{
			Logger.DEBUG(string.Empty);
			if (dataLen < 4)
			{
				return 0;
			}
			uint num = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(receivedData, 0));
			return (int)(num + 4u);
		}

		public override void OnReceived(Packet thePacket)
		{
			Logger.DEBUG(string.Empty);
		}

		public override void OnClose()
		{
			Logger.DEBUG(string.Empty);
			Message message = new Message();
			message.status = -1;
			message.content.set_Item("uniqueSocketId", base.GetUniqueSocketId());
			message.action = this.statusChangedAction;
			Pandora.Instance.GetNetLogic().EnqueueResult(message);
		}
	}
}
