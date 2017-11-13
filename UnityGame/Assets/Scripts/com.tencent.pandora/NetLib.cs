using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace com.tencent.pandora
{
	public class NetLib
	{
		public delegate void DataHandler(int encodedDataLen, [MarshalAs(20)] string encodedData, long uniqueSocketId);

		public delegate void LogHandler(int level, [MarshalAs(20)] string logMsg);

		private static long pendingPacketsLength = 0L;

		private static Dictionary<long, TCPSocketContext> dictSocketContext = new Dictionary<long, TCPSocketContext>();

		[MonoPInvokeCallback(typeof(NetLib.DataHandler))]
		public static void DataCallback(int encodedDataLen, [MarshalAs(20)] string encodedData, long uniqueSocketId)
		{
			try
			{
				if (NetLib.dictSocketContext.ContainsKey(uniqueSocketId))
				{
					TCPSocketContext tCPSocketContext = NetLib.dictSocketContext.get_Item(uniqueSocketId);
					tCPSocketContext.ReadDataCallback(encodedDataLen, encodedData);
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		[MonoPInvokeCallback(typeof(NetLib.LogHandler))]
		public static void LogCallback(int level, [MarshalAs(20)] string logMsg)
		{
			try
			{
				switch (level)
				{
				case 0:
					Logger.DEBUG(logMsg);
					break;
				case 1:
					Logger.INFO(logMsg);
					break;
				case 2:
					Logger.WARN(logMsg);
					break;
				case 3:
					Logger.ERROR(logMsg);
					break;
				}
			}
			catch (Exception var_0_4E)
			{
			}
		}

		public static void Init()
		{
			NetLib.PandoraNet_RegisterDataHandler(new NetLib.DataHandler(NetLib.DataCallback));
			NetLib.PandoraNet_RegisterLogHandler(new NetLib.LogHandler(NetLib.LogCallback));
			NetLib.PandoraNet_Init();
		}

		public static void Reset()
		{
			using (Dictionary<long, TCPSocketContext>.Enumerator enumerator = NetLib.dictSocketContext.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<long, TCPSocketContext> current = enumerator.get_Current();
					long key = current.get_Key();
					NetLib.PandoraNet_Close(key);
				}
			}
			NetLib.dictSocketContext.Clear();
			NetLib.pendingPacketsLength = 0L;
		}

		public static void Destroy()
		{
			NetLib.PandoraNet_Destroy();
		}

		public static void Drive()
		{
			List<long> list = new List<long>();
			List<long> list2 = new List<long>();
			using (Dictionary<long, TCPSocketContext>.Enumerator enumerator = NetLib.dictSocketContext.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<long, TCPSocketContext> current = enumerator.get_Current();
					list.Add(current.get_Key());
				}
			}
			using (List<long>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					long current2 = enumerator2.get_Current();
					if (NetLib.dictSocketContext.ContainsKey(current2))
					{
						long num = current2;
						TCPSocketContext tCPSocketContext = NetLib.dictSocketContext.get_Item(num);
						int num2 = NetLib.PandoraNet_DoSelect(num);
						if (num2 == 0)
						{
							if (tCPSocketContext.isConnectionTimeout())
							{
								int num3 = 0;
								tCPSocketContext.HandleClose(out num3);
								NetLib.pendingPacketsLength -= (long)num3;
								list2.Add(num);
							}
						}
						else if (num2 == 1)
						{
							int num4 = tCPSocketContext.HandleInputEvent();
							if (num4 < 0)
							{
								int num5 = 0;
								tCPSocketContext.HandleClose(out num5);
								NetLib.pendingPacketsLength -= (long)num5;
								list2.Add(num);
							}
						}
						else if (num2 == 2)
						{
							int num6 = 0;
							int num7 = tCPSocketContext.HandleOutputEvent(out num6);
							NetLib.pendingPacketsLength -= (long)num6;
							if (num7 < 0)
							{
								int num8 = 0;
								tCPSocketContext.HandleClose(out num8);
								NetLib.pendingPacketsLength -= (long)num8;
								list2.Add(num);
							}
						}
						else
						{
							int num9 = 0;
							tCPSocketContext.HandleClose(out num9);
							NetLib.pendingPacketsLength -= (long)num9;
							list2.Add(num);
						}
					}
				}
			}
			using (List<long>.Enumerator enumerator3 = list2.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					long current3 = enumerator3.get_Current();
					NetLib.dictSocketContext.Remove(current3);
					NetLib.PandoraNet_Close(current3);
				}
			}
		}

		public static void AddCommand(Command cmd)
		{
			try
			{
				if (cmd is AddSocketCommand)
				{
					AddSocketCommand addSocketCommand = cmd as AddSocketCommand;
					long theUniqueSocketId = addSocketCommand.theUniqueSocketId;
					TCPSocketContext tCPSocketContext = new TCPSocketContext(addSocketCommand.theUniqueSocketId, addSocketCommand.theHandler);
					if (!NetLib.dictSocketContext.ContainsKey(theUniqueSocketId))
					{
						Logger.DEBUG(theUniqueSocketId.ToString() + " added");
						NetLib.dictSocketContext.Add(theUniqueSocketId, tCPSocketContext);
					}
					else
					{
						TCPSocketContext tCPSocketContext2 = NetLib.dictSocketContext.get_Item(theUniqueSocketId);
						Logger.ERROR("NetThread::ProcessCommands socket handle conflict " + tCPSocketContext.GetUniqueSocketId().ToString() + " VS " + tCPSocketContext2.GetUniqueSocketId().ToString());
					}
				}
				else if (cmd is SendPacketCommand)
				{
					SendPacketCommand sendPacketCommand = cmd as SendPacketCommand;
					long theUniqueSocketId2 = sendPacketCommand.theUniqueSocketId;
					if (NetLib.dictSocketContext.ContainsKey(theUniqueSocketId2))
					{
						TCPSocketContext tCPSocketContext3 = NetLib.dictSocketContext.get_Item(theUniqueSocketId2);
						Packet packet = new Packet();
						packet.theCreateTimeMS = sendPacketCommand.theCreateTimeMS;
						packet.theContent = (sendPacketCommand.theContent.Clone() as byte[]);
						tCPSocketContext3.Enqueue(packet);
						NetLib.pendingPacketsLength += (long)packet.theContent.Length;
					}
				}
				else
				{
					CloseSocketCommand closeSocketCommand = cmd as CloseSocketCommand;
					long theUniqueSocketId3 = closeSocketCommand.theUniqueSocketId;
					if (NetLib.dictSocketContext.ContainsKey(theUniqueSocketId3))
					{
						TCPSocketContext tCPSocketContext4 = NetLib.dictSocketContext.get_Item(theUniqueSocketId3);
						Logger.DEBUG(theUniqueSocketId3.ToString() + " removed");
						int num = 0;
						tCPSocketContext4.HandleClose(out num);
						NetLib.pendingPacketsLength -= (long)num;
						NetLib.dictSocketContext.Remove(theUniqueSocketId3);
						NetLib.PandoraNet_Close(theUniqueSocketId3);
					}
					else
					{
						Logger.WARN("uniqueSocketId=" + theUniqueSocketId3.ToString() + " alreay missing");
					}
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		public static long GetPendingPacketsLength()
		{
			return NetLib.pendingPacketsLength;
		}

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern long PandoraNet_AsyncConnect([MarshalAs(20)] string ipStr, ushort port);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern void PandoraNet_Close(long uniqueSocketId);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern int PandoraNet_AsyncRead(long uniqueSocketId, int leftBufferLen);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern int PandoraNet_AsyncWrite(int encodedDataLen, [MarshalAs(20)] string encodedData, long uniqueSocketId);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern void PandoraNet_RegisterDataHandler([MarshalAs(38)] NetLib.DataHandler handler);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern void PandoraNet_RegisterLogHandler([MarshalAs(38)] NetLib.LogHandler handler);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern int PandoraNet_Init();

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern int PandoraNet_Destroy();

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern int PandoraNet_DoSelect(long uniqueSocketId);
	}
}
