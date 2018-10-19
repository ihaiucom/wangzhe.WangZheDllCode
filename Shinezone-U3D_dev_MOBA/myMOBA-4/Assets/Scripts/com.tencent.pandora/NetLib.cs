using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace com.tencent.pandora
{
	public class NetLib
	{
		public delegate void DataHandler(int encodedDataLen, [MarshalAs(UnmanagedType.LPStr)] string encodedData, long uniqueSocketId);

		public delegate void LogHandler(int level, [MarshalAs(UnmanagedType.LPStr)] string logMsg);

		private static long pendingPacketsLength = 0L;

		private static Dictionary<long, TCPSocketContext> dictSocketContext = new Dictionary<long, TCPSocketContext>();

		[MonoPInvokeCallback(typeof(NetLib.DataHandler))]
		public static void DataCallback(int encodedDataLen, [MarshalAs(UnmanagedType.LPStr)] string encodedData, long uniqueSocketId)
		{
			try
			{
				if (NetLib.dictSocketContext.ContainsKey(uniqueSocketId))
				{
					TCPSocketContext tCPSocketContext = NetLib.dictSocketContext[uniqueSocketId];
					tCPSocketContext.ReadDataCallback(encodedDataLen, encodedData);
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
		}

		[MonoPInvokeCallback(typeof(NetLib.LogHandler))]
		public static void LogCallback(int level, [MarshalAs(UnmanagedType.LPStr)] string logMsg)
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
			foreach (KeyValuePair<long, TCPSocketContext> current in NetLib.dictSocketContext)
			{
				long key = current.Key;
				NetLib.PandoraNet_Close(key);
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
			foreach (KeyValuePair<long, TCPSocketContext> current in NetLib.dictSocketContext)
			{
				list.Add(current.Key);
			}
			foreach (long current2 in list)
			{
				if (NetLib.dictSocketContext.ContainsKey(current2))
				{
					long num = current2;
					TCPSocketContext tCPSocketContext = NetLib.dictSocketContext[num];
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
			foreach (long current3 in list2)
			{
				NetLib.dictSocketContext.Remove(current3);
				NetLib.PandoraNet_Close(current3);
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
						TCPSocketContext tCPSocketContext2 = NetLib.dictSocketContext[theUniqueSocketId];
						Logger.ERROR("NetThread::ProcessCommands socket handle conflict " + tCPSocketContext.GetUniqueSocketId().ToString() + " VS " + tCPSocketContext2.GetUniqueSocketId().ToString());
					}
				}
				else if (cmd is SendPacketCommand)
				{
					SendPacketCommand sendPacketCommand = cmd as SendPacketCommand;
					long theUniqueSocketId2 = sendPacketCommand.theUniqueSocketId;
					if (NetLib.dictSocketContext.ContainsKey(theUniqueSocketId2))
					{
						TCPSocketContext tCPSocketContext3 = NetLib.dictSocketContext[theUniqueSocketId2];
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
						TCPSocketContext tCPSocketContext4 = NetLib.dictSocketContext[theUniqueSocketId3];
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
				Logger.ERROR(ex.StackTrace);
			}
		}

		public static long GetPendingPacketsLength()
		{
			return NetLib.pendingPacketsLength;
		}

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern long PandoraNet_AsyncConnect([MarshalAs(UnmanagedType.LPStr)] string ipStr, ushort port);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern void PandoraNet_Close(long uniqueSocketId);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern int PandoraNet_AsyncRead(long uniqueSocketId, int leftBufferLen);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern int PandoraNet_AsyncWrite(int encodedDataLen, [MarshalAs(UnmanagedType.LPStr)] string encodedData, long uniqueSocketId);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern void PandoraNet_RegisterDataHandler([MarshalAs(UnmanagedType.FunctionPtr)] NetLib.DataHandler handler);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern void PandoraNet_RegisterLogHandler([MarshalAs(UnmanagedType.FunctionPtr)] NetLib.LogHandler handler);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern int PandoraNet_Init();

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern int PandoraNet_Destroy();

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern int PandoraNet_DoSelect(long uniqueSocketId);
	}
}
