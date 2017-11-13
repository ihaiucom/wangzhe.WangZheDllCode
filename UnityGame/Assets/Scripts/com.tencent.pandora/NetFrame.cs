using System;
using System.Net;

namespace com.tencent.pandora
{
	public class NetFrame
	{
		private static int kMaxPendingPacketsLength = 33554432;

		public void Init()
		{
			NetLib.Init();
		}

		public void Destroy()
		{
			NetLib.Destroy();
		}

		public void Reset()
		{
			NetLib.Reset();
		}

		public void Drive()
		{
			NetLib.Drive();
		}

		public long AsyncConnect(IPAddress address, ushort port, TCPSocketHandler handler)
		{
			Logger.DEBUG("Y1");
			string ipStr = address.ToString();
			long num = NetLib.PandoraNet_AsyncConnect(ipStr, port);
			if (num < 0L)
			{
				Logger.ERROR("NetLib.AsyncConnect ret=" + num.ToString());
				return -1L;
			}
			NetLib.AddCommand(new AddSocketCommand
			{
				theUniqueSocketId = num,
				theHandler = handler
			});
			return num;
		}

		public void Close(long uniqueSocketId)
		{
			Logger.DEBUG("Y5" + uniqueSocketId.ToString());
			NetLib.AddCommand(new CloseSocketCommand
			{
				theUniqueSocketId = uniqueSocketId
			});
		}

		public int SendPacket(long uniqueSocketId, byte[] content)
		{
			Logger.DEBUG("Y6" + uniqueSocketId.ToString());
			long pendingPacketsLength = NetLib.GetPendingPacketsLength();
			if ((long)NetFrame.kMaxPendingPacketsLength < pendingPacketsLength + (long)content.Length)
			{
				Logger.ERROR("Y7pending pakcets overflow");
				return -1;
			}
			if (content.Length == 0)
			{
				Logger.ERROR("Y8empty content is not allowed");
				return -2;
			}
			NetLib.AddCommand(new SendPacketCommand
			{
				theUniqueSocketId = uniqueSocketId,
				theContent = (content.Clone() as byte[]),
				theCreateTimeMS = (long)DateTime.get_Now().get_Millisecond()
			});
			return 0;
		}
	}
}
