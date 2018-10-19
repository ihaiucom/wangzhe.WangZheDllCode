using com.tencent.pandora.MiniJSON;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace com.tencent.pandora
{
	public class WeChatLink
	{
		private class StateObject
		{
			public const int BUFFER_SIZE = 1024;

			public Socket workSocket;

			public byte[] buffer = new byte[1024];
		}

		public static readonly WeChatLink Instance = new WeChatLink();

		private string kSDKVersion = "YXZJ-0.1";

		private Action<Dictionary<string, string>> callbackForGame;

		private Dictionary<string, string> userData = new Dictionary<string, string>();

		private bool firstReceive = true;

		private int packetDataLen;

		private int haveRcvdLen;

		private byte[] receivedByte;

		private List<string> urlList = new List<string>();

		private int connectTimeOut = 400;

		private int sendRcvTimeOut = 3000;

		public void BeginGetGameZoneUrl(Dictionary<string, string> userDataDict, Action<Dictionary<string, string>> action)
		{
			this.userData = userDataDict;
			this.callbackForGame = action;
			string text = "wzry.broker.tplay.qq.com";
			int port = 5692;
			List<IPAddress> list = new List<IPAddress>();
			if (text.Length > 0)
			{
				try
				{
					IPAddress item = null;
					if (IPAddress.TryParse(text, out item))
					{
						list.Add(item);
					}
					else
					{
						IPHostEntry hostEntry = Dns.GetHostEntry(text);
						IPAddress[] addressList = hostEntry.AddressList;
						for (int i = 0; i < addressList.Length; i++)
						{
							IPAddress item2 = addressList[i];
							list.Add(item2);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log(ex.Message);
					this.NotifyShowZone(false);
				}
			}
			if (list.Count > 0)
			{
				int index = new System.Random().Next(list.Count);
				IPEndPoint end_point = new IPEndPoint(list[index], port);
				Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, this.sendRcvTimeOut);
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, this.sendRcvTimeOut);
				Debug.Log("Begin Connet");
				IAsyncResult asyncResult = socket.BeginConnect(end_point, null, null);
				WaitHandle asyncWaitHandle = asyncResult.AsyncWaitHandle;
				try
				{
					if (!asyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double)this.connectTimeOut)))
					{
						Debug.Log("Connect Time Out:" + this.connectTimeOut.ToString());
						this.CloseSocket(socket);
					}
					else
					{
						try
						{
							socket.EndConnect(asyncResult);
							if (asyncResult.IsCompleted)
							{
								string actReqJson = this.GetActReqJson();
								this.AsynCallBroker(socket, actReqJson);
							}
						}
						catch (Exception ex2)
						{
							Debug.Log(ex2.Message);
							this.CloseSocket(socket);
						}
					}
				}
				catch (Exception ex3)
				{
					Debug.Log(ex3.Message);
					this.CloseSocket(socket);
				}
				finally
				{
					asyncWaitHandle.Close();
				}
			}
		}

		private void AsynCallBroker(Socket socket, string message)
		{
			if (socket == null || message == string.Empty)
			{
				return;
			}
			Debug.Log(string.Empty);
			try
			{
				int num = 9000;
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["seq_id"] = 1;
				dictionary["cmd_id"] = num;
				dictionary["type"] = 1;
				dictionary["from_ip"] = "10.0.0.108";
				dictionary["process_id"] = 1;
				dictionary["mod_id"] = 10;
				dictionary["version"] = this.kSDKVersion;
				dictionary["body"] = message;
				dictionary["app_id"] = this.userData["sAppId"];
				string text = Json.Serialize(dictionary);
				string s = MinizLib.Compress(text.Length, text);
				byte[] array = Convert.FromBase64String(s);
				int value = IPAddress.HostToNetworkOrder(array.Length);
				byte[] bytes = BitConverter.GetBytes(value);
				byte[] array2 = new byte[bytes.Length + array.Length];
				Array.Copy(bytes, 0, array2, 0, bytes.Length);
				Array.Copy(array, 0, array2, bytes.Length, array.Length);
				IAsyncResult asyncResult = socket.BeginSend(array2, 0, array2.Length, SocketFlags.None, null, null);
				WaitHandle asyncWaitHandle = asyncResult.AsyncWaitHandle;
				try
				{
					if (!asyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double)this.sendRcvTimeOut)))
					{
						Debug.Log("Send Time Out:" + this.sendRcvTimeOut.ToString());
						this.CloseSocket(socket);
					}
					else
					{
						try
						{
							int num2 = socket.EndSend(asyncResult);
							if (asyncResult.IsCompleted && num2 == array2.Length)
							{
								Debug.Log(string.Format("客户端发送消息:{0}", text));
								this.AsynRecive(socket);
							}
							else
							{
								this.CloseSocket(socket);
							}
						}
						catch (Exception ex)
						{
							Debug.Log(ex.Message);
							this.CloseSocket(socket);
						}
					}
				}
				catch (Exception ex2)
				{
					Debug.Log(ex2.Message);
					this.CloseSocket(socket);
				}
				finally
				{
					asyncWaitHandle.Close();
				}
			}
			catch (Exception ex3)
			{
				Debug.Log(string.Format("异常信息：{0}", ex3.Message));
				this.CloseSocket(socket);
			}
		}

		private void AsynRecive(Socket socket)
		{
			Debug.Log(string.Empty);
			WeChatLink.StateObject stateObject = new WeChatLink.StateObject();
			stateObject.workSocket = socket;
			try
			{
				IAsyncResult asyncResult = socket.BeginReceive(stateObject.buffer, 0, 1024, SocketFlags.None, null, stateObject);
				WaitHandle asyncWaitHandle = asyncResult.AsyncWaitHandle;
				try
				{
					if (!asyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double)this.sendRcvTimeOut)))
					{
						Debug.Log("Receive Time Out:" + this.sendRcvTimeOut.ToString());
						this.CloseSocket(socket);
					}
					else
					{
						this.ReceiveCallback(asyncResult);
					}
				}
				catch (Exception ex)
				{
					Debug.Log(ex.Message);
					this.CloseSocket(socket);
				}
				finally
				{
					asyncWaitHandle.Close();
				}
			}
			catch (Exception ex2)
			{
				Debug.Log(ex2.Message);
				this.CloseSocket(socket);
			}
		}

		private void ReceiveCallback(IAsyncResult asyncResult)
		{
			Debug.Log(string.Empty);
			WeChatLink.StateObject stateObject = (WeChatLink.StateObject)asyncResult.AsyncState;
			Socket workSocket = stateObject.workSocket;
			try
			{
				int num = workSocket.EndReceive(asyncResult);
				if (num > 0)
				{
					if (this.firstReceive)
					{
						if (num > 4)
						{
							this.firstReceive = false;
							byte[] array = new byte[4];
							Array.Copy(stateObject.buffer, array, 4);
							this.packetDataLen = BitConverter.ToInt32(array, 0);
							this.packetDataLen = IPAddress.NetworkToHostOrder(this.packetDataLen) + 4;
							this.receivedByte = new byte[this.packetDataLen];
							this.HandleRcvData(stateObject, num);
						}
						else
						{
							this.CloseSocket(workSocket);
						}
					}
					else
					{
						this.HandleRcvData(stateObject, num);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				this.CloseSocket(workSocket);
			}
		}

		private void HandleRcvData(WeChatLink.StateObject so, int readLen)
		{
			if (this.haveRcvdLen + readLen < this.packetDataLen)
			{
				this.haveRcvdLen += readLen;
				Array.Copy(so.buffer, this.receivedByte, readLen);
				this.AsynRecive(so.workSocket);
			}
			else
			{
				Array.Copy(so.buffer, this.receivedByte, this.packetDataLen - this.haveRcvdLen);
				this.haveRcvdLen = this.packetDataLen;
				this.ParseActData();
				this.CloseSocket(so.workSocket);
			}
		}

		private string GetActReqJson()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["seq_id"] = "1";
			dictionary["cmd_id"] = "10000";
			dictionary["msg_type"] = "1";
			dictionary["sdk_version"] = this.kSDKVersion;
			dictionary["game_app_id"] = this.userData["sAppId"];
			dictionary["channel_id"] = "23029";
			dictionary["plat_id"] = this.userData["sPlatID"];
			dictionary["area_id"] = this.userData["sArea"];
			dictionary["patition_id"] = this.userData["sPartition"];
			dictionary["open_id"] = this.userData["sOpenId"];
			dictionary["role_id"] = this.userData["sRoleId"];
			dictionary["act_style"] = "4";
			dictionary["timestamp"] = Utils.NowSeconds();
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2["md5_val"] = string.Empty;
			Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
			dictionary3["head"] = dictionary;
			dictionary3["body"] = dictionary2;
			string text = Json.Serialize(dictionary3);
			Debug.Log(text);
			return text;
		}

		private void ParseActData()
		{
			try
			{
				string text = Convert.ToBase64String(this.receivedByte, 4, this.receivedByte.Length - 4);
				string s = MinizLib.UnCompress(text.Length, text);
				byte[] bytes = Convert.FromBase64String(s);
				string @string = Encoding.UTF8.GetString(bytes);
				Debug.Log(@string);
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary = (Json.Deserialize(@string) as Dictionary<string, object>);
				if (dictionary != null && dictionary.ContainsKey("body"))
				{
					string text2 = dictionary["body"] as string;
					Debug.Log(text2);
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					dictionary2 = (Json.Deserialize(text2) as Dictionary<string, object>);
					if (dictionary2 != null && dictionary2.ContainsKey("ret"))
					{
						Debug.Log(string.Empty);
						string text3 = dictionary2["ret"].ToString();
						if (text3.Equals("0") && dictionary2.ContainsKey("resp"))
						{
							string text4 = dictionary2["resp"] as string;
							Debug.Log(text4);
							Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
							dictionary3 = (Json.Deserialize(text4) as Dictionary<string, object>);
							if (dictionary3 != null && dictionary3.ContainsKey("body"))
							{
								string text5 = dictionary3["body"] as string;
								Debug.Log(text5);
								Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
								dictionary4 = (Json.Deserialize(text5) as Dictionary<string, object>);
								if (dictionary4 != null && dictionary4.ContainsKey("online_msg_info"))
								{
									Dictionary<string, object> dictionary5 = dictionary4["online_msg_info"] as Dictionary<string, object>;
									if (dictionary5 != null && dictionary5.ContainsKey("act_list"))
									{
										List<object> list = dictionary5["act_list"] as List<object>;
										foreach (object current in list)
										{
											Dictionary<string, object> dictionary6 = current as Dictionary<string, object>;
											if (dictionary6 != null && dictionary6.ContainsKey("jump_url") && !string.IsNullOrEmpty(dictionary6["jump_url"].ToString()))
											{
												Debug.Log("获取到Url: " + dictionary6["jump_url"].ToString());
												this.urlList.Add(dictionary6["jump_url"].ToString());
											}
										}
										if (list.Count > 0)
										{
											this.NotifyShowZone(true);
										}
										else
										{
											this.NotifyShowZone(false);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.NotifyShowZone(false);
				Debug.Log(ex.StackTrace);
			}
		}

		private void Reset()
		{
			Debug.Log(string.Empty);
			this.callbackForGame = null;
			this.userData = new Dictionary<string, string>();
			this.firstReceive = true;
			this.packetDataLen = 0;
			this.haveRcvdLen = 0;
			this.receivedByte = null;
			this.urlList = new List<string>();
		}

		private void CloseSocket(Socket socket)
		{
			try
			{
				socket.Close();
			}
			catch (SocketException ex)
			{
				Debug.Log(ex.Message);
			}
			catch (ObjectDisposedException ex2)
			{
				Debug.Log(ex2.Message);
			}
			finally
			{
				this.NotifyShowZone(false);
			}
		}

		private void NotifyShowZone(bool bShow)
		{
			if (this.callbackForGame != null)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (bShow)
				{
					dictionary.Add("showGameZone", "1");
				}
				else
				{
					dictionary.Add("showGameZone", "0");
				}
				this.callbackForGame(dictionary);
			}
			this.Reset();
		}
	}
}
