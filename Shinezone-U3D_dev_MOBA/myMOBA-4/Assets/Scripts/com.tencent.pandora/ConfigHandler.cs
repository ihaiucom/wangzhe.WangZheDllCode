using com.tencent.pandora.MiniJSON;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.tencent.pandora
{
	public class ConfigHandler : TCPSocketHandler
	{
		private static int kMaxHttpHeaderLength = 8192;

		private static byte[] httpHeaderEnd = Encoding.UTF8.GetBytes("\r\n\r\n");

		private static byte[] successStatusLine1_1 = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK");

		private static byte[] contentLengthField = Encoding.UTF8.GetBytes("Content-Length");

		private int detectedHeaderLength = -1;

		private Action<int, Dictionary<string, object>> theAction;

		private string configUrl = string.Empty;

		public ConfigHandler(Action<int, Dictionary<string, object>> action, string configUrl)
		{
			this.theAction = action;
			this.configUrl = configUrl;
		}

		public override void OnConnected()
		{
			Logger.DEBUG(string.Empty);
			long uniqueSocketId = base.GetUniqueSocketId();
			Action<int, Dictionary<string, object>> action = delegate(int status, Dictionary<string, object> content)
			{
				try
				{
					UserData userData = Pandora.Instance.GetUserData();
					string text = string.Concat(new string[]
					{
						"openid=",
						userData.sOpenId,
						"&partition=",
						userData.sPartition,
						"&gameappversion=",
						userData.sGameVer,
						"&areaid=",
						userData.sArea,
						"&appid=",
						userData.sAppId,
						"&acctype=",
						userData.sAcountType,
						"&platid=",
						userData.sPlatID,
						"&sdkversion=",
						Pandora.Instance.GetSDKVersion(),
						"&_pdr_time=",
						Utils.NowSeconds().ToString()
					});
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary["openid"] = userData.sOpenId;
					dictionary["partition"] = userData.sPartition;
					dictionary["gameappversion"] = userData.sGameVer;
					dictionary["areaid"] = userData.sArea;
					dictionary["acctype"] = userData.sAcountType;
					dictionary["platid"] = userData.sPlatID;
					dictionary["appid"] = userData.sAppId;
					dictionary["sdkversion"] = Pandora.Instance.GetSDKVersion();
					string rawData = Json.Serialize(dictionary);
					string str = MsdkTea.Encode(rawData);
					string text2 = "{\"data\":\"" + str + "\",\"encrypt\" : \"true\"}";
					Uri uri = new Uri(this.configUrl);
					string text3 = string.Concat(new string[]
					{
						"POST ",
						uri.AbsolutePath,
						"?",
						text,
						" HTTP/1.1\r\nHost:",
						uri.Host,
						"\r\nAccept:*/*\r\nUser-Agent:Pandora(",
						Pandora.Instance.GetSDKVersion(),
						")\r\nContent-Length:",
						text2.Length.ToString(),
						"\r\nConnection: keep-alive\r\n\r\n",
						text2
					});
					Logger.DEBUG_LOGCAT(text3);
					byte[] bytes = Encoding.UTF8.GetBytes(text3);
					Pandora.Instance.GetNetLogic().SendPacket(uniqueSocketId, bytes);
				}
				catch (Exception ex)
				{
					Logger.ERROR(ex.Message);
					Pandora.Instance.GetNetLogic().Close(uniqueSocketId);
				}
			};
			Message message = new Message();
			message.status = 0;
			message.action = action;
			Pandora.Instance.GetNetLogic().EnqueueResult(message);
		}

		public override int DetectPacketSize(byte[] receivedData, int dataLen)
		{
			Logger.DEBUG(string.Empty);
			int num = -1;
			for (int i = 0; i < dataLen - ConfigHandler.httpHeaderEnd.Length + 1; i++)
			{
				bool flag = true;
				for (int j = 0; j < ConfigHandler.httpHeaderEnd.Length; j++)
				{
					if (receivedData[i + j] != ConfigHandler.httpHeaderEnd[j])
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					this.detectedHeaderLength = i + ConfigHandler.httpHeaderEnd.Length;
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				if (dataLen > ConfigHandler.kMaxHttpHeaderLength)
				{
					Logger.ERROR(string.Empty);
					return -1;
				}
				return 0;
			}
			else
			{
				int num2 = 0;
				while (num2 < dataLen && num2 < ConfigHandler.successStatusLine1_1.Length)
				{
					if (num2 >= dataLen || receivedData[num2] != ConfigHandler.successStatusLine1_1[num2])
					{
						Logger.ERROR(string.Empty);
						return -2;
					}
					num2++;
				}
				int num3 = -1;
				int num4 = -1;
				int num5 = -1;
				for (int k = 0; k < dataLen - ConfigHandler.contentLengthField.Length + 1; k++)
				{
					bool flag2 = true;
					for (int l = 0; l < ConfigHandler.contentLengthField.Length; l++)
					{
						if (receivedData[k + l] != ConfigHandler.contentLengthField[l])
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						num3 = k;
						break;
					}
				}
				if (num3 < 0)
				{
					Logger.ERROR(string.Empty);
					return -3;
				}
				for (int m = num3 + ConfigHandler.contentLengthField.Length; m < dataLen; m++)
				{
					if (receivedData[m] >= 48 && receivedData[m] <= 57)
					{
						if (num4 < 0)
						{
							num4 = m;
						}
					}
					else if (num4 >= 0 && num5 < 0)
					{
						num5 = m;
					}
				}
				if (num4 < 0 || num5 < 0 || num4 >= num5)
				{
					Logger.ERROR(string.Empty);
					return -3;
				}
				int result;
				try
				{
					string @string = Encoding.UTF8.GetString(receivedData, num4, num5 - num4);
					int num6 = Convert.ToInt32(@string);
					if (num6 > 0)
					{
						result = this.detectedHeaderLength + num6;
					}
					else
					{
						Logger.ERROR(string.Empty);
						result = -3;
					}
				}
				catch (Exception ex)
				{
					Logger.ERROR(ex.StackTrace);
					result = -3;
				}
				return result;
			}
		}

		public override void OnReceived(Packet thePacket)
		{
			Logger.DEBUG(string.Empty);
			try
			{
				int num = thePacket.theContent.Length - this.detectedHeaderLength;
				byte[] array = new byte[num];
				Array.Copy(thePacket.theContent, this.detectedHeaderLength, array, 0, num);
				string @string = Encoding.UTF8.GetString(array);
				Dictionary<string, object> dictionary = Json.Deserialize(@string) as Dictionary<string, object>;
				string s = dictionary["data"] as string;
				byte[] encodedDataBytes = Convert.FromBase64String(s);
				string text = MsdkTea.Decode(encodedDataBytes);
				if (text.Length > 0)
				{
					Logger.DEBUG_LOGCAT(text);
					Dictionary<string, object> content = Json.Deserialize(text) as Dictionary<string, object>;
					Message message = new Message();
					message.status = 0;
					message.action = this.theAction;
					message.content = content;
					Pandora.Instance.GetNetLogic().EnqueueResult(message);
					this.theAction = null;
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
			}
			long uniqueSocketId = base.GetUniqueSocketId();
			Pandora.Instance.GetNetLogic().Close(uniqueSocketId);
		}

		public override void OnClose()
		{
			Logger.DEBUG(string.Empty);
			Message message = new Message();
			message.status = -1;
			message.action = this.theAction;
			Pandora.Instance.GetNetLogic().EnqueueResult(message);
		}
	}
}
