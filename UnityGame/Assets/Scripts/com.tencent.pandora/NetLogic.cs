using com.tencent.pandora.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace com.tencent.pandora
{
	public class NetLogic
	{
		public class DownloadRequest
		{
			public string url = string.Empty;

			public int size;

			public string md5 = string.Empty;

			public string destFile = string.Empty;

			public int curRedirectionTimes;

			public Action<int, Dictionary<string, object>> action;
		}

		private static uint streamReportSeqNo;

		private static int brokerHeartbeatIntervalInSecs = 180;

		private static int brokerHeartbeatTimeoutInSecs = 10;

		private static int maxBrokerHeartbeatTimeoutRetryCnt = 2;

		private static ushort atmReportPort = 5692;

		private List<IPAddress> atmIPAddresses = new List<IPAddress>();

		private int lastTryAtmIPAddressIndex;

		private long atmUniqueSocketId = -1L;

		private bool isAtmConnecting;

		private int lastConnectAtmTime = -1;

		private Queue atmPendingRequests = Queue.Synchronized(new Queue());

		private ushort brokerPort;

		private string brokerHost = string.Empty;

		private string[] brokerAltIPs = new string[0];

		private List<IPAddress> brokerIPAddresses = new List<IPAddress>();

		private int lastTryBorkerIPAddressIndex;

		private uint brokerHeartbeatSeqNo;

		private int lastSendBrokerHeartbeatReqTime;

		private int lastRecvBrokerHeartbeatRspTime;

		private int brokerHeartbeatTimeoutRetryCnt;

		private bool hasLoginBroker;

		private long brokerUniqueSocketId = -1L;

		private bool isBrokerConnecting;

		private int lastConnectBrokerTime = -1;

		private Queue brokerPendingRequests = Queue.Synchronized(new Queue());

		private Queue resultQueue = Queue.Synchronized(new Queue());

		private bool bNeedBrokerHeart;

		private Queue downloadRequestQueue = Queue.Synchronized(new Queue());

		private Queue logicDriveQueue = Queue.Synchronized(new Queue());

		private Dictionary<string, IPHostEntry> dnsCache = new Dictionary<string, IPHostEntry>();

		private HashSet<string> hasUpdateEntryHosts = new HashSet<string>();

		private bool isDownloadingPaused;

		private NetFrame netFrame = new NetFrame();

		public void Init()
		{
			this.netFrame.Init();
			this.dnsCache = Utils.LoadDnsCacheFromFile();
		}

		public void Destroy()
		{
			Logger.DEBUG(string.Empty);
			this.netFrame.Destroy();
		}

		public void Logout()
		{
			Logger.DEBUG(string.Empty);
			this.netFrame.Reset();
			this.resultQueue.Clear();
			this.isDownloadingPaused = false;
			this.atmIPAddresses.Clear();
			this.lastTryAtmIPAddressIndex = 0;
			this.atmUniqueSocketId = -1L;
			this.lastConnectAtmTime = -1;
			this.isAtmConnecting = false;
			this.atmPendingRequests.Clear();
			this.brokerPort = 0;
			this.brokerHost = string.Empty;
			this.brokerAltIPs = new string[0];
			this.brokerIPAddresses.Clear();
			this.lastTryBorkerIPAddressIndex = 0;
			this.brokerUniqueSocketId = -1L;
			this.lastConnectBrokerTime = -1;
			this.isBrokerConnecting = false;
			this.brokerPendingRequests.Clear();
			this.ResetBrokerHeartbeatVars();
			this.downloadRequestQueue.Clear();
			this.logicDriveQueue.Clear();
		}

		public void EnqueueDrive(Message msg)
		{
			this.logicDriveQueue.Enqueue(msg);
		}

		public void EnqueueResult(Message msg)
		{
			this.resultQueue.Enqueue(msg);
		}

		public void Drive()
		{
			if (this.isDownloadingPaused)
			{
				return;
			}
			this.netFrame.Drive();
			while (this.resultQueue.get_Count() > 0)
			{
				Message message = this.resultQueue.Dequeue() as Message;
				if (message.action != null)
				{
					message.action.Invoke(message.status, message.content);
				}
			}
			int num = Utils.NowSeconds();
			if (this.atmUniqueSocketId < 0L && !this.isAtmConnecting && this.lastConnectAtmTime + 5 < num)
			{
				this.CheckAtmSession();
			}
			if (this.brokerUniqueSocketId < 0L && !this.isBrokerConnecting && this.lastConnectBrokerTime + 5 < num)
			{
				this.CheckBrokerSession();
			}
			else if (this.brokerUniqueSocketId > 0L)
			{
				if (this.bNeedBrokerHeart)
				{
					this.ProcessBrokerHeartbeat();
				}
				else
				{
					this.ProcessBrokerLogin();
				}
			}
			this.TrySendAtmReport();
			this.TrySendBrokerRequest();
			while (this.logicDriveQueue.get_Count() > 0)
			{
				Message message2 = this.logicDriveQueue.Dequeue() as Message;
				if (message2.action != null)
				{
					message2.action.Invoke(message2.status, message2.content);
				}
			}
			while (this.downloadRequestQueue.get_Count() > 0)
			{
				NetLogic.DownloadRequest request = this.downloadRequestQueue.Dequeue() as NetLogic.DownloadRequest;
				Action<int, Dictionary<string, object>> action = delegate(int downloadRet, Dictionary<string, object> content)
				{
					if (downloadRet == 100)
					{
						Logger.DEBUG(string.Empty);
						string url = content.get_Item("locationUrl") as string;
						this.AddDownload(url, request.size, request.md5, request.destFile, request.curRedirectionTimes + 1, request.action);
					}
					else
					{
						Logger.DEBUG(string.Empty);
						Message message3 = new Message();
						message3.status = downloadRet;
						message3.action = request.action;
						message3.content = content;
						this.EnqueueResult(message3);
					}
				};
				this.DownloadFile(request.url, request.size, request.md5, request.destFile, request.curRedirectionTimes, action);
			}
		}

		private void CheckAtmSession()
		{
			if (this.atmIPAddresses.get_Count() == 0)
			{
				string atmHost = "jsonatm.broker.tplay.qq.com";
				Action<IPHostEntry> resultAction = delegate(IPHostEntry entry)
				{
					Logger.DEBUG(atmHost + " dns done");
					if (entry != null && entry.get_AddressList().Length > 0)
					{
						IPAddress[] addressList = entry.get_AddressList();
						for (int i = 0; i < addressList.Length; i++)
						{
							IPAddress iPAddress = addressList[i];
							this.atmIPAddresses.Add(iPAddress);
						}
					}
					else
					{
						string[] array = new string[]
						{
							"101.226.129.205",
							"140.206.160.193",
							"182.254.10.86",
							"115.25.209.29",
							"117.144.245.201"
						};
						for (int j = 0; j < array.Length; j++)
						{
							string text = array[j];
							this.atmIPAddresses.Add(IPAddress.Parse(text));
						}
					}
					this.lastConnectAtmTime = 0;
				};
				this.lastConnectAtmTime = Utils.NowSeconds();
				this.AsyncGetHostEntry(atmHost, resultAction);
				return;
			}
			try
			{
				Action<int, Dictionary<string, object>> statusChangedAction = delegate(int status, Dictionary<string, object> content)
				{
					this.isAtmConnecting = false;
					if (status == 0)
					{
						this.atmUniqueSocketId = (long)content.get_Item("uniqueSocketId");
					}
					else
					{
						this.atmUniqueSocketId = -1L;
					}
				};
				this.lastConnectAtmTime = Utils.NowSeconds();
				this.lastTryAtmIPAddressIndex = (this.lastTryAtmIPAddressIndex + 1) % this.atmIPAddresses.get_Count();
				long num = this.SpawnTCPSession(this.atmIPAddresses.get_Item(this.lastTryAtmIPAddressIndex), NetLogic.atmReportPort, new ATMHandler(statusChangedAction));
				if (num > 0L)
				{
					Logger.DEBUG(string.Empty);
					this.isAtmConnecting = true;
				}
				else
				{
					Logger.ERROR(string.Empty);
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		private void CheckBrokerSession()
		{
			if (this.brokerIPAddresses.get_Count() == 0)
			{
				this.lastConnectBrokerTime = Utils.NowSeconds();
				return;
			}
			try
			{
				Action<int, Dictionary<string, object>> statusChangedAction = delegate(int status, Dictionary<string, object> content)
				{
					this.isBrokerConnecting = false;
					if (status == 0)
					{
						this.brokerUniqueSocketId = (long)content.get_Item("uniqueSocketId");
					}
					else
					{
						this.brokerUniqueSocketId = -1L;
						this.ResetBrokerHeartbeatVars();
					}
				};
				Action<int, Dictionary<string, object>> packetRecvdAction = delegate(int status, Dictionary<string, object> content)
				{
					this.ProcessBrokerResponse(status, content);
				};
				this.lastConnectBrokerTime = Utils.NowSeconds();
				this.lastTryBorkerIPAddressIndex = (this.lastTryBorkerIPAddressIndex + 1) % this.brokerIPAddresses.get_Count();
				long num = this.SpawnTCPSession(this.brokerIPAddresses.get_Item(this.lastTryBorkerIPAddressIndex), this.brokerPort, new BrokerHandler(statusChangedAction, packetRecvdAction));
				if (num > 0L)
				{
					Logger.DEBUG(this.brokerIPAddresses.get_Item(this.lastTryBorkerIPAddressIndex).ToString());
					this.isBrokerConnecting = true;
				}
				else
				{
					Logger.ERROR(string.Empty);
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		private void TrySendAtmReport()
		{
			if (this.atmUniqueSocketId > 0L)
			{
				while (this.atmPendingRequests.get_Count() > 0)
				{
					Logger.DEBUG(string.Empty);
					PendingRequest pendingRequest = this.atmPendingRequests.Dequeue() as PendingRequest;
					this.netFrame.SendPacket(this.atmUniqueSocketId, pendingRequest.data);
				}
			}
			else
			{
				int num = Utils.NowSeconds();
				while (this.atmPendingRequests.get_Count() > 0)
				{
					PendingRequest pendingRequest2 = this.atmPendingRequests.Peek() as PendingRequest;
					if (num <= pendingRequest2.createTime + 10)
					{
						break;
					}
					Logger.WARN(string.Empty);
					this.atmPendingRequests.Dequeue();
				}
			}
		}

		private void TrySendBrokerRequest()
		{
			if (this.brokerUniqueSocketId > 0L)
			{
				while (this.brokerPendingRequests.get_Count() > 0)
				{
					Logger.DEBUG(string.Empty);
					PendingRequest pendingRequest = this.brokerPendingRequests.Dequeue() as PendingRequest;
					this.netFrame.SendPacket(this.brokerUniqueSocketId, pendingRequest.data);
				}
			}
			else
			{
				int num = Utils.NowSeconds();
				while (this.brokerPendingRequests.get_Count() > 0)
				{
					PendingRequest pendingRequest2 = this.brokerPendingRequests.Peek() as PendingRequest;
					if (num <= pendingRequest2.createTime + 5)
					{
						break;
					}
					Logger.WARN(string.Empty);
					this.brokerPendingRequests.Dequeue();
				}
			}
		}

		public void SetBroker(ushort port, string host, string ip1, string ip2)
		{
			Logger.DEBUG(string.Concat(new string[]
			{
				"port=",
				port.ToString(),
				" host=",
				host,
				" ip1=",
				ip1,
				" ip2=",
				ip2
			}));
			this.brokerPort = port;
			this.brokerHost = host;
			this.brokerAltIPs = new string[]
			{
				ip1,
				ip2
			};
			Action<IPHostEntry> resultAction = delegate(IPHostEntry entry)
			{
				try
				{
					Logger.DEBUG(this.brokerHost + " dns done");
					if (entry != null && entry.get_AddressList().Length > 0)
					{
						IPAddress[] addressList = entry.get_AddressList();
						for (int i = 0; i < addressList.Length; i++)
						{
							IPAddress iPAddress = addressList[i];
							this.brokerIPAddresses.Add(iPAddress);
						}
					}
					else
					{
						string[] array = this.brokerAltIPs;
						for (int j = 0; j < array.Length; j++)
						{
							string text = array[j];
							if (text.get_Length() > 0)
							{
								this.brokerIPAddresses.Add(IPAddress.Parse(text));
							}
						}
					}
					this.lastConnectBrokerTime = 0;
				}
				catch (Exception ex)
				{
					Logger.ERROR(ex.get_Message() + ": " + ex.get_StackTrace());
				}
			};
			this.AsyncGetHostEntry(this.brokerHost, resultAction);
		}

		public void SetDownloadingPaused(bool status)
		{
			this.isDownloadingPaused = status;
		}

		public long SpawnTCPSession(IPAddress addr, ushort port, TCPSocketHandler handler)
		{
			Logger.DEBUG(string.Empty);
			return this.netFrame.AsyncConnect(addr, port, handler);
		}

		public void GetRemoteConfig(Action<int, Dictionary<string, object>> action)
		{
			Logger.DEBUG(string.Empty);
			try
			{
				string configUrl = "http://apps.game.qq.com/cgi-bin/api/tplay/" + Pandora.Instance.GetUserData().sAppId + "_cloud.cgi";
				Logger.DEBUG_LOGCAT(configUrl);
				Uri uri = new Uri(configUrl);
				Action<IPHostEntry> resultAction = delegate(IPHostEntry entry)
				{
					Logger.DEBUG(configUrl + " dns done");
					if (entry != null && entry.get_AddressList().Length > 0)
					{
						IPAddress[] addressList = entry.get_AddressList();
						int num = new Random().Next(addressList.Length);
						long num2 = this.SpawnTCPSession(addressList[num], (ushort)uri.get_Port(), new ConfigHandler(action, configUrl));
						if (num2 > 0L)
						{
							Logger.DEBUG(addressList[num].ToString());
						}
						else
						{
							Logger.ERROR(string.Empty);
							action.Invoke(-1, new Dictionary<string, object>());
						}
					}
					else
					{
						Logger.ERROR(string.Empty);
						action.Invoke(-1, new Dictionary<string, object>());
					}
				};
				this.AsyncGetHostEntry(uri.get_Host(), resultAction);
			}
			catch (Exception ex)
			{
				action.Invoke(-1, new Dictionary<string, object>());
				Logger.ERROR(ex.get_Message() + ":" + ex.get_StackTrace());
			}
		}

		public int SendPacket(long uniqueSocketId, byte[] content)
		{
			Logger.DEBUG(uniqueSocketId.ToString());
			return this.netFrame.SendPacket(uniqueSocketId, content);
		}

		public void Close(long uniqueSocketId)
		{
			Logger.DEBUG(uniqueSocketId.ToString());
			this.netFrame.Close(uniqueSocketId);
		}

		public void AddDownload(string url, int size, string md5, string destFile, int curRedirectionTimes, Action<int, Dictionary<string, object>> action)
		{
			Logger.DEBUG(url);
			NetLogic.DownloadRequest downloadRequest = new NetLogic.DownloadRequest();
			downloadRequest.url = url;
			downloadRequest.size = size;
			downloadRequest.md5 = md5;
			downloadRequest.destFile = destFile;
			downloadRequest.curRedirectionTimes = curRedirectionTimes;
			downloadRequest.action = action;
			this.downloadRequestQueue.Enqueue(downloadRequest);
		}

		private void DownloadFile(string url, int size, string md5, string destFile, int curRedirectionTimes, Action<int, Dictionary<string, object>> action)
		{
			Logger.DEBUG(url);
			try
			{
				Uri uri = new Uri(url);
				Action<IPHostEntry> resultAction = delegate(IPHostEntry entry)
				{
					Logger.DEBUG(url + " dns done");
					if (entry != null && entry.get_AddressList().Length > 0)
					{
						IPAddress[] addressList = entry.get_AddressList();
						int num = new Random().Next(addressList.Length);
						long num2 = this.SpawnTCPSession(addressList[num], (ushort)uri.get_Port(), new DownloadHandler(url, size, md5, destFile, curRedirectionTimes, action));
						if (num2 > 0L)
						{
							Logger.DEBUG(addressList[num].ToString());
						}
						else
						{
							Logger.ERROR(string.Empty);
							action.Invoke(-1, new Dictionary<string, object>());
						}
					}
					else
					{
						Logger.ERROR(string.Empty);
						action.Invoke(-1, new Dictionary<string, object>());
					}
				};
				this.AsyncGetHostEntry(uri.get_Host(), resultAction);
			}
			catch (Exception ex)
			{
				action.Invoke(-1, new Dictionary<string, object>());
				Logger.ERROR(ex.get_Message() + ":" + ex.get_StackTrace());
			}
		}

		public void AsyncGetHostEntry(string host, Action<IPHostEntry> resultAction)
		{
			Logger.DEBUG(host);
			try
			{
				IPAddress iPAddress = null;
				if (IPAddress.TryParse(host, ref iPAddress))
				{
					IPHostEntry iPHostEntry = new IPHostEntry();
					iPHostEntry.set_AddressList(new IPAddress[]
					{
						iPAddress
					});
					resultAction.Invoke(iPHostEntry);
				}
				else
				{
					if (this.dnsCache.ContainsKey(host))
					{
						IPHostEntry iPHostEntry2 = this.dnsCache.get_Item(host);
						resultAction.Invoke(iPHostEntry2);
						resultAction = null;
					}
					if (!this.hasUpdateEntryHosts.Contains(host))
					{
						Action<IAsyncResult> action = delegate(IAsyncResult ar)
						{
							try
							{
								string text = (string)ar.get_AsyncState();
								Logger.DEBUG(text + " end");
								IPHostEntry entry = Dns.EndGetHostEntry(ar);
								Logger.DEBUG(text + " end, entry.AddressList.Length=" + entry.get_AddressList().Length.ToString());
								Action<int, Dictionary<string, object>> action2 = delegate(int status, Dictionary<string, object> content)
								{
									string text2 = content.get_Item("host") as string;
									IPHostEntry iPHostEntry3 = content.get_Item("entry") as IPHostEntry;
									Action<IPHostEntry> action3 = content.get_Item("resultAction") as Action<IPHostEntry>;
									this.hasUpdateEntryHosts.Add(text2);
									this.dnsCache.set_Item(text2, entry);
									Utils.SaveDnsCacheToFile(this.dnsCache);
									if (action3 != null)
									{
										action3.Invoke(entry);
									}
								};
								Message message = new Message();
								message.status = 0;
								message.content.set_Item("host", text);
								message.content.set_Item("entry", entry);
								message.content.set_Item("resultAction", resultAction);
								message.action = action2;
								this.EnqueueDrive(message);
							}
							catch (Exception ex2)
							{
								Logger.ERROR(ex2.get_Message() + ":" + ex2.get_StackTrace());
							}
						};
						Logger.DEBUG(host + " begin");
						Dns.BeginGetHostEntry(host, new AsyncCallback(action.Invoke), host);
					}
				}
			}
			catch (Exception ex)
			{
				resultAction.Invoke(null);
				Logger.ERROR(ex.get_Message() + ":" + ex.get_StackTrace());
			}
		}

		public void StreamReport(string jsonData)
		{
			Logger.DEBUG(string.Empty);
			try
			{
				PendingRequest pendingRequest = new PendingRequest();
				pendingRequest.createTime = Utils.NowSeconds();
				pendingRequest.seqNo = NetLogic.streamReportSeqNo++;
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				UserData userData = Pandora.Instance.GetUserData();
				dictionary.set_Item("seq_id", pendingRequest.seqNo);
				dictionary.set_Item("cmd_id", 5000);
				dictionary.set_Item("type", 1);
				dictionary.set_Item("from_ip", "10.0.0.108");
				dictionary.set_Item("process_id", 1);
				dictionary.set_Item("mod_id", 10);
				dictionary.set_Item("version", Pandora.Instance.GetSDKVersion());
				dictionary.set_Item("body", jsonData);
				dictionary.set_Item("app_id", userData.sAppId);
				string text = Json.Serialize(dictionary);
				string text2 = MinizLib.Compress(text.get_Length(), text);
				byte[] array = Convert.FromBase64String(text2);
				int num = IPAddress.HostToNetworkOrder(array.Length);
				byte[] bytes = BitConverter.GetBytes(num);
				byte[] array2 = new byte[bytes.Length + array.Length];
				Array.Copy(bytes, 0, array2, 0, bytes.Length);
				Array.Copy(array, 0, array2, bytes.Length, array.Length);
				pendingRequest.data = array2;
				this.atmPendingRequests.Enqueue(pendingRequest);
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		public void CallBroker(uint callId, string jsonData, int cmdId, int type = 1, int modId = 10)
		{
			Logger.DEBUG(string.Empty);
			try
			{
				PendingRequest pendingRequest = new PendingRequest();
				pendingRequest.createTime = Utils.NowSeconds();
				pendingRequest.seqNo = callId;
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				UserData userData = Pandora.Instance.GetUserData();
				dictionary.set_Item("seq_id", callId);
				dictionary.set_Item("cmd_id", cmdId);
				dictionary.set_Item("type", type);
				dictionary.set_Item("from_ip", "10.0.0.108");
				dictionary.set_Item("process_id", 1);
				dictionary.set_Item("mod_id", modId);
				dictionary.set_Item("version", Pandora.Instance.GetSDKVersion());
				dictionary.set_Item("body", jsonData);
				dictionary.set_Item("app_id", userData.sAppId);
				string text = Json.Serialize(dictionary);
				string text2 = MinizLib.Compress(text.get_Length(), text);
				byte[] array = Convert.FromBase64String(text2);
				int num = IPAddress.HostToNetworkOrder(array.Length);
				byte[] bytes = BitConverter.GetBytes(num);
				byte[] array2 = new byte[bytes.Length + array.Length];
				Array.Copy(bytes, 0, array2, 0, bytes.Length);
				Array.Copy(array, 0, array2, bytes.Length, array.Length);
				pendingRequest.data = array2;
				this.brokerPendingRequests.Enqueue(pendingRequest);
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		private void ProcessBrokerLogin()
		{
			int num = Utils.NowSeconds();
			if (!this.hasLoginBroker && num >= this.lastSendBrokerHeartbeatReqTime + NetLogic.brokerHeartbeatIntervalInSecs && this.brokerHeartbeatTimeoutRetryCnt < NetLogic.maxBrokerHeartbeatTimeoutRetryCnt)
			{
				this.SendLoginToBroker();
				this.brokerHeartbeatTimeoutRetryCnt++;
			}
		}

		private void ProcessBrokerHeartbeat()
		{
			int num = Utils.NowSeconds();
			if (this.lastRecvBrokerHeartbeatRspTime >= this.lastSendBrokerHeartbeatReqTime)
			{
				if (num >= this.lastSendBrokerHeartbeatReqTime + NetLogic.brokerHeartbeatIntervalInSecs)
				{
					this.SendHeartbeatToBroker();
				}
			}
			else if (num >= this.lastSendBrokerHeartbeatReqTime + NetLogic.brokerHeartbeatTimeoutInSecs)
			{
				if (this.brokerHeartbeatTimeoutRetryCnt < NetLogic.maxBrokerHeartbeatTimeoutRetryCnt)
				{
					Logger.WARN("Broker heartbeat is timeout, try again later, retryCnt=" + this.brokerHeartbeatTimeoutRetryCnt);
					this.SendHeartbeatToBroker();
					this.brokerHeartbeatTimeoutRetryCnt++;
				}
				else
				{
					Logger.ERROR("Broker heartbeat is timeout, the connection to broker will be closed");
					this.Close(this.brokerUniqueSocketId);
					this.ResetBrokerHeartbeatVars();
				}
			}
		}

		private void SendHeartbeatToBroker()
		{
			if (!this.hasLoginBroker)
			{
				string brokerLoginReq = this.GetBrokerLoginReq();
				Logger.DEBUG("Send login request to broker, req=" + brokerLoginReq);
				this.CallBroker(this.brokerHeartbeatSeqNo++, brokerLoginReq, 1001, 1, 10);
			}
			else
			{
				string brokerHeartbeatReq = this.GetBrokerHeartbeatReq();
				Logger.DEBUG("Send heartbeat request to broker, req=" + brokerHeartbeatReq);
				this.CallBroker(this.brokerHeartbeatSeqNo++, brokerHeartbeatReq, 1002, 1, 10);
			}
			this.lastSendBrokerHeartbeatReqTime = Utils.NowSeconds();
		}

		private void SendLoginToBroker()
		{
			string brokerLoginReq = this.GetBrokerLoginReq();
			Logger.DEBUG("Send login request to broker, req=" + brokerLoginReq);
			this.CallBroker(this.brokerHeartbeatSeqNo++, brokerLoginReq, 1001, 1, 10);
			this.lastSendBrokerHeartbeatReqTime = Utils.NowSeconds();
		}

		private string GetBrokerHeartbeatReq()
		{
			UserData userData = Pandora.Instance.GetUserData();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.set_Item("open_id", userData.sOpenId);
			dictionary.set_Item("app_id", userData.sAppId);
			dictionary.set_Item("sarea", userData.sArea);
			dictionary.set_Item("splatid", userData.sPlatID);
			dictionary.set_Item("spartition", userData.sPartition);
			return Json.Serialize(dictionary);
		}

		private void FinishRecvBrokerHeartbeatRsp()
		{
			this.brokerHeartbeatTimeoutRetryCnt = 0;
			this.lastRecvBrokerHeartbeatRspTime = Utils.NowSeconds();
		}

		private void ResetBrokerHeartbeatVars()
		{
			this.lastSendBrokerHeartbeatReqTime = 0;
			this.lastRecvBrokerHeartbeatRspTime = 0;
			this.brokerHeartbeatTimeoutRetryCnt = 0;
			this.hasLoginBroker = false;
		}

		public void ProcessBrokerResponse(int status, Dictionary<string, object> content)
		{
			Logger.DEBUG(string.Empty);
			try
			{
				long num = (long)content.get_Item("type");
				if (num == 1L)
				{
					int num2 = (int)((long)content.get_Item("cmd_id"));
					uint callId = (uint)((long)content.get_Item("seq_id"));
					int num3 = num2;
					if (num3 != 1003)
					{
						Logger.ERROR("recv invalid type[" + num.ToString() + "] from broker");
					}
					else
					{
						Logger.DEBUG("recv push req from broker, req: " + Json.Serialize(content));
						string text = Json.Serialize(content.get_Item("body"));
						Dictionary<string, object> dictionary = Json.Deserialize(text) as Dictionary<string, object>;
						Dictionary<string, object> headDict = dictionary.get_Item("head") as Dictionary<string, object>;
						string brokerPushRsp = this.GetBrokerPushRsp(headDict);
						Logger.DEBUG("Send push rsp to broker, rsp=" + brokerPushRsp);
						this.CallBroker(callId, brokerPushRsp, 1003, 2, 9000);
						CSharpInterface.NotifyPushData(text);
					}
				}
				else if (num == 2L)
				{
					int num4 = (int)((long)content.get_Item("cmd_id"));
					uint callId2 = (uint)((long)content.get_Item("seq_id"));
					int num5 = num4;
					if (num5 != 1001)
					{
						if (num5 != 1002)
						{
							if (num5 != 5001)
							{
								if (num5 != 9000)
								{
									Logger.ERROR("recv invalid cmdId[" + num4.ToString() + "] from broker");
								}
								else
								{
									Logger.DEBUG("recv lua request rsp, seqId[" + callId2.ToString() + "]");
									Dictionary<string, object> dictionary2 = Json.Deserialize(content.get_Item("body") as string) as Dictionary<string, object>;
									dictionary2.set_Item("netRet", 0);
									string result = Json.Serialize(dictionary2);
									CSharpInterface.ExecCallback(callId2, result);
								}
							}
							else
							{
								Logger.DEBUG("recv statistics rsp, seqId[" + callId2.ToString() + "]");
								string json = content.get_Item("body") as string;
								Dictionary<string, object> dictionary3 = Json.Deserialize(json) as Dictionary<string, object>;
								int num6 = (int)((long)dictionary3.get_Item("ret"));
								if (num6 != 0)
								{
									Logger.ERROR(string.Concat(new object[]
									{
										"recv error statistics rsp, ret[",
										num6.ToString(),
										"] errMsg[",
										dictionary3.get_Item("err_msg")
									}) + "]");
								}
							}
						}
						else
						{
							Logger.DEBUG("recv heartbeat rsp from broker, rsp:" + Json.Serialize(content));
							this.FinishRecvBrokerHeartbeatRsp();
						}
					}
					else
					{
						Logger.DEBUG("recv login rsp from broker, rsp:" + Json.Serialize(content));
						Dictionary<string, object> dictionary4 = Json.Deserialize(content.get_Item("body") as string) as Dictionary<string, object>;
						if ((int)((long)dictionary4.get_Item("ret")) == 0)
						{
							this.hasLoginBroker = true;
							this.FinishRecvBrokerHeartbeatRsp();
						}
						else
						{
							Logger.ERROR("login failed, rsp: " + Json.Serialize(content));
						}
					}
				}
				else
				{
					Logger.ERROR("recv invalid type[" + num.ToString() + "] from broker");
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		private string GetBrokerLoginReq()
		{
			UserData userData = Pandora.Instance.GetUserData();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.set_Item("open_id", userData.sOpenId);
			dictionary.set_Item("app_id", userData.sAppId);
			dictionary.set_Item("sarea", userData.sArea);
			dictionary.set_Item("splatid", userData.sPlatID);
			dictionary.set_Item("spartition", userData.sPartition);
			dictionary.set_Item("access_token", userData.sAccessToken);
			dictionary.set_Item("acctype", userData.sAcountType);
			return Json.Serialize(dictionary);
		}

		private string GetBrokerPushRsp(Dictionary<string, object> headDict)
		{
			UserData userData = Pandora.Instance.GetUserData();
			headDict.set_Item("access_token", userData.sAccessToken);
			headDict.set_Item("acc_type", userData.sAcountType);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.set_Item("head", headDict);
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.set_Item("ret", 0);
			dictionary2.set_Item("err_msg", string.Empty);
			dictionary2.set_Item("resp", Json.Serialize(dictionary));
			return Json.Serialize(dictionary2);
		}
	}
}
