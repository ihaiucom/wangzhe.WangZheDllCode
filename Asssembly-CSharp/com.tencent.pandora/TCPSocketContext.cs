using System;
using System.Collections;

namespace com.tencent.pandora
{
	public class TCPSocketContext
	{
		private long theUniqueSocketId;

		private TCPSocketHandler theHandler;

		private int createTime;

		private bool isOnConnectCalled;

		private Queue pendingPackets = new Queue();

		private int peekPacketSentSize;

		private static int kMaxPakcetSize = 4194304;

		private static int kDefaultReceiveBufferCapacity = 131072;

		private int receiveBufferCapacity = TCPSocketContext.kDefaultReceiveBufferCapacity;

		private byte[] receiveBuffer = new byte[TCPSocketContext.kDefaultReceiveBufferCapacity];

		private int receivedLength;

		private int detectedPacketSize;

		public TCPSocketContext(long uniqueSocketId, TCPSocketHandler handler)
		{
			this.theUniqueSocketId = uniqueSocketId;
			this.theHandler = handler;
			this.theHandler.SetUniqueSocketId(uniqueSocketId);
			this.createTime = TCPSocketContext.NowSeconds();
		}

		public static int NowSeconds()
		{
			return (int)DateTime.get_UtcNow().Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).get_TotalSeconds();
		}

		public bool isConnectionTimeout()
		{
			return !this.isOnConnectCalled && this.createTime + 10 < TCPSocketContext.NowSeconds();
		}

		public long GetUniqueSocketId()
		{
			return this.theUniqueSocketId;
		}

		public TCPSocketHandler GetHandler()
		{
			return this.theHandler;
		}

		private void AdjustReceiveBuffer()
		{
			int num = this.receiveBufferCapacity;
			if (this.detectedPacketSize > 0)
			{
				if (this.detectedPacketSize > this.receiveBufferCapacity)
				{
					num = Math.Min(this.detectedPacketSize, TCPSocketContext.kMaxPakcetSize);
				}
				else if ((double)this.detectedPacketSize < (double)this.receiveBufferCapacity * 0.5)
				{
					num = Math.Max((int)((double)this.receiveBufferCapacity * 0.5), TCPSocketContext.kDefaultReceiveBufferCapacity);
				}
			}
			else if ((double)this.receivedLength > (double)this.receiveBufferCapacity * 0.8 && this.receiveBufferCapacity < TCPSocketContext.kMaxPakcetSize)
			{
				num = Math.Min(TCPSocketContext.kMaxPakcetSize, 2 * this.receiveBufferCapacity);
			}
			if (num != this.receiveBufferCapacity)
			{
				byte[] array = new byte[num];
				Array.Copy(this.receiveBuffer, array, this.receivedLength);
				this.receiveBuffer = array;
				this.receiveBufferCapacity = num;
			}
		}

		private int SplitReceiveDataToPackets()
		{
			int num = 0;
			while (this.receivedLength > 0)
			{
				int num2 = this.theHandler.DetectPacketSize(this.receiveBuffer, this.receivedLength);
				if (num2 > 0)
				{
					if (num2 > TCPSocketContext.kMaxPakcetSize)
					{
						Logger.ERROR("Z1detected packet size overflow " + num2.ToString());
						return -1;
					}
					this.detectedPacketSize = num2;
					if (this.detectedPacketSize > this.receivedLength)
					{
						return num;
					}
					Packet packet = new Packet();
					packet.theCreateTimeMS = (long)DateTime.get_Now().get_Millisecond();
					packet.theContent = new byte[this.detectedPacketSize];
					Array.Copy(this.receiveBuffer, 0, packet.theContent, 0, this.detectedPacketSize);
					this.theHandler.OnReceived(packet);
					num++;
					Array.Copy(this.receiveBuffer, this.detectedPacketSize, this.receiveBuffer, 0, this.receivedLength - this.detectedPacketSize);
					this.receivedLength -= this.detectedPacketSize;
					this.detectedPacketSize = 0;
				}
				else
				{
					if (num2 != 0)
					{
						Logger.ERROR("Z3detect packet size error");
						return -3;
					}
					if (this.receivedLength == TCPSocketContext.kMaxPakcetSize)
					{
						Logger.ERROR("Z2format error");
						return -2;
					}
					break;
				}
			}
			return num;
		}

		public void ReadDataCallback(int encodedDataLen, string encodedData)
		{
			Logger.DEBUG("theUniqueSocketId=" + this.theUniqueSocketId.ToString() + " encodedDataLen=" + encodedDataLen.ToString());
			byte[] array = Convert.FromBase64String(encodedData);
			Logger.DEBUG("decodedData.Length=[" + array.Length + "]");
			Array.Copy(array, 0, this.receiveBuffer, this.receivedLength, array.Length);
		}

		public int HandleInputEvent()
		{
			Logger.DEBUG("Z4");
			if (!this.isOnConnectCalled)
			{
				this.theHandler.OnConnected();
				this.isOnConnectCalled = true;
				return 0;
			}
			int num;
			while (true)
			{
				this.AdjustReceiveBuffer();
				int leftBufferLen = this.receiveBufferCapacity - this.receivedLength;
				num = NetLib.PandoraNet_AsyncRead(this.theUniqueSocketId, leftBufferLen);
				Logger.DEBUG("theUniqueSocketId=" + this.theUniqueSocketId.ToString() + " recvdLen=" + num.ToString());
				if (num < 0)
				{
					break;
				}
				this.receivedLength += num;
				int num2 = this.SplitReceiveDataToPackets();
				if (num2 > 0)
				{
					goto Block_3;
				}
				if (num2 < 0)
				{
					goto Block_4;
				}
			}
			if (num != -100)
			{
				return -1;
			}
			Block_3:
			return 0;
			Block_4:
			Logger.ERROR("Z6received data error");
			return -2;
		}

		public int HandleOutputEvent(out int sentDataSize)
		{
			if (!this.isOnConnectCalled)
			{
				this.theHandler.OnConnected();
				this.isOnConnectCalled = true;
			}
			sentDataSize = 0;
			if (this.pendingPackets.get_Count() == 0)
			{
				return 0;
			}
			Packet packet = this.pendingPackets.Peek() as Packet;
			while (this.peekPacketSentSize < packet.theContent.Length)
			{
				int num = packet.theContent.Length - this.peekPacketSentSize;
				string text = Convert.ToBase64String(packet.theContent, this.peekPacketSentSize, num);
				int num2 = NetLib.PandoraNet_AsyncWrite(text.get_Length(), text, this.theUniqueSocketId);
				if (num2 < 0)
				{
					if (num2 == -100)
					{
						break;
					}
					return -1;
				}
				else
				{
					this.peekPacketSentSize += num2;
				}
			}
			if (this.peekPacketSentSize == packet.theContent.Length)
			{
				sentDataSize = this.peekPacketSentSize;
				this.peekPacketSentSize = 0;
				Packet thePacket = this.pendingPackets.Dequeue() as Packet;
				this.theHandler.OnSent(thePacket);
			}
			return this.pendingPackets.get_Count();
		}

		public int HandleClose(out int discardDataSize)
		{
			Logger.DEBUG("Z12");
			discardDataSize = 0;
			using (IEnumerator enumerator = this.pendingPackets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Packet packet = (Packet)enumerator.get_Current();
					discardDataSize += packet.theContent.Length;
				}
			}
			this.pendingPackets.Clear();
			this.theHandler.OnClose();
			return 0;
		}

		public void Enqueue(Packet packet)
		{
			this.pendingPackets.Enqueue(packet);
		}
	}
}
