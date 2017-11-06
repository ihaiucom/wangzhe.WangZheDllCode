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
			int num = 5692;
			List<IPAddress> list = new List<IPAddress>();
			if (text.get_Length() > 0)
			{
				try
				{
					IPAddress iPAddress = null;
					if (IPAddress.TryParse(text, ref iPAddress))
					{
						list.Add(iPAddress);
					}
					else
					{
						IPHostEntry hostEntry = Dns.GetHostEntry(text);
						IPAddress[] addressList = hostEntry.get_AddressList();
						for (int i = 0; i < addressList.Length; i++)
						{
							IPAddress iPAddress2 = addressList[i];
							list.Add(iPAddress2);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log(ex.get_Message());
					this.NotifyShowZone(false);
				}
			}
			if (list.get_Count() > 0)
			{
				int num2 = new Random().Next(list.get_Count());
				IPEndPoint iPEndPoint = new IPEndPoint(list.get_Item(num2), num);
				Socket socket = new Socket(2, 1, 6);
				socket.SetSocketOption(65535, 4101, this.sendRcvTimeOut);
				socket.SetSocketOption(65535, 4102, this.sendRcvTimeOut);
				Debug.Log("Begin Connet");
				IAsyncResult asyncResult = socket.BeginConnect(iPEndPoint, null, null);
				WaitHandle asyncWaitHandle = asyncResult.get_AsyncWaitHandle();
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
							if (asyncResult.get_IsCompleted())
							{
								string actReqJson = this.GetActReqJson();
								this.AsynCallBroker(socket, actReqJson);
							}
						}
						catch (Exception ex2)
						{
							Debug.Log(ex2.get_Message());
							this.CloseSocket(socket);
						}
					}
				}
				catch (Exception ex3)
				{
					Debug.Log(ex3.get_Message());
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
				dictionary.set_Item("seq_id", 1);
				dictionary.set_Item("cmd_id", num);
				dictionary.set_Item("type", 1);
				dictionary.set_Item("from_ip", "10.0.0.108");
				dictionary.set_Item("process_id", 1);
				dictionary.set_Item("mod_id", 10);
				dictionary.set_Item("version", this.kSDKVersion);
				dictionary.set_Item("body", message);
				dictionary.set_Item("app_id", this.userData.get_Item("sAppId"));
				string text = Json.Serialize(dictionary);
				string text2 = MinizLib.Compress(text.get_Length(), text);
				byte[] array = Convert.FromBase64String(text2);
				int num2 = IPAddress.HostToNetworkOrder(array.Length);
				byte[] bytes = BitConverter.GetBytes(num2);
				byte[] array2 = new byte[bytes.Length + array.Length];
				Array.Copy(bytes, 0, array2, 0, bytes.Length);
				Array.Copy(array, 0, array2, bytes.Length, array.Length);
				IAsyncResult asyncResult = socket.BeginSend(array2, 0, array2.Length, 0, null, null);
				WaitHandle asyncWaitHandle = asyncResult.get_AsyncWaitHandle();
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
							int num3 = socket.EndSend(asyncResult);
							if (asyncResult.get_IsCompleted() && num3 == array2.Length)
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
							Debug.Log(ex.get_Message());
							this.CloseSocket(socket);
						}
					}
				}
				catch (Exception ex2)
				{
					Debug.Log(ex2.get_Message());
					this.CloseSocket(socket);
				}
				finally
				{
					asyncWaitHandle.Close();
				}
			}
			catch (Exception ex3)
			{
				Debug.Log(string.Format("异常信息：{0}", ex3.get_Message()));
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
				IAsyncResult asyncResult = socket.BeginReceive(stateObject.buffer, 0, 1024, 0, null, stateObject);
				WaitHandle asyncWaitHandle = asyncResult.get_AsyncWaitHandle();
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
					Debug.Log(ex.get_Message());
					this.CloseSocket(socket);
				}
				finally
				{
					asyncWaitHandle.Close();
				}
			}
			catch (Exception ex2)
			{
				Debug.Log(ex2.get_Message());
				this.CloseSocket(socket);
			}
		}

		private void ReceiveCallback(IAsyncResult asyncResult)
		{
			Debug.Log(string.Empty);
			WeChatLink.StateObject stateObject = (WeChatLink.StateObject)asyncResult.get_AsyncState();
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
				Debug.Log(ex.get_Message());
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
			dictionary.set_Item("seq_id", "1");
			dictionary.set_Item("cmd_id", "10000");
			dictionary.set_Item("msg_type", "1");
			dictionary.set_Item("sdk_version", this.kSDKVersion);
			dictionary.set_Item("game_app_id", this.userData.get_Item("sAppId"));
			dictionary.set_Item("channel_id", "23029");
			dictionary.set_Item("plat_id", this.userData.get_Item("sPlatID"));
			dictionary.set_Item("area_id", this.userData.get_Item("sArea"));
			dictionary.set_Item("patition_id", this.userData.get_Item("sPartition"));
			dictionary.set_Item("open_id", this.userData.get_Item("sOpenId"));
			dictionary.set_Item("role_id", this.userData.get_Item("sRoleId"));
			dictionary.set_Item("act_style", "4");
			dictionary.set_Item("timestamp", Utils.NowSeconds());
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.set_Item("md5_val", string.Empty);
			Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
			dictionary3.set_Item("head", dictionary);
			dictionary3.set_Item("body", dictionary2);
			string text = Json.Serialize(dictionary3);
			Debug.Log(text);
			return text;
		}

		private void ParseActData()
		{
			try
			{
				string text = Convert.ToBase64String(this.receivedByte, 4, this.receivedByte.Length - 4);
				string text2 = MinizLib.UnCompress(text.get_Length(), text);
				byte[] array = Convert.FromBase64String(text2);
				string @string = Encoding.get_UTF8().GetString(array);
				Debug.Log(@string);
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary = (Json.Deserialize(@string) as Dictionary<string, object>);
				if (dictionary != null && dictionary.ContainsKey("body"))
				{
					string text3 = dictionary.get_Item("body") as string;
					Debug.Log(text3);
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					dictionary2 = (Json.Deserialize(text3) as Dictionary<string, object>);
					if (dictionary2 != null && dictionary2.ContainsKey("ret"))
					{
						Debug.Log(string.Empty);
						string text4 = dictionary2.get_Item("ret").ToString();
						if (text4.Equals("0") && dictionary2.ContainsKey("resp"))
						{
							string text5 = dictionary2.get_Item("resp") as string;
							Debug.Log(text5);
							Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
							dictionary3 = (Json.Deserialize(text5) as Dictionary<string, object>);
							if (dictionary3 != null && dictionary3.ContainsKey("body"))
							{
								string text6 = dictionary3.get_Item("body") as string;
								Debug.Log(text6);
								Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
								dictionary4 = (Json.Deserialize(text6) as Dictionary<string, object>);
								if (dictionary4 != null && dictionary4.ContainsKey("online_msg_info"))
								{
									Dictionary<string, object> dictionary5 = dictionary4.get_Item("online_msg_info") as Dictionary<string, object>;
									if (dictionary5 != null && dictionary5.ContainsKey("act_list"))
									{
										List<object> list = dictionary5.get_Item("act_list") as List<object>;
										using (List<object>.Enumerator enumerator = list.GetEnumerator())
										{
											while (enumerator.MoveNext())
											{
												object current = enumerator.get_Current();
												Dictionary<string, object> dictionary6 = current as Dictionary<string, object>;
												if (dictionary6 != null && dictionary6.ContainsKey("jump_url") && !string.IsNullOrEmpty(dictionary6.get_Item("jump_url").ToString()))
												{
													Debug.Log("获取到Url: " + dictionary6.get_Item("jump_url").ToString());
													this.urlList.Add(dictionary6.get_Item("jump_url").ToString());
												}
											}
										}
										if (list.get_Count() > 0)
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
				Debug.Log(ex.get_StackTrace());
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
				Debug.Log(ex.get_Message());
			}
			catch (ObjectDisposedException ex2)
			{
				Debug.Log(ex2.get_Message());
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
				this.callbackForGame.Invoke(dictionary);
			}
			this.Reset();
		}
	}
}
