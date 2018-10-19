using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace com.tencent.pandora
{
	public class DownloadHandler : TCPSocketHandler
	{
		private static int kMaxHttpHeaderLength = 8192;

		private static byte[] httpHeaderEnd = Encoding.UTF8.GetBytes("\r\n\r\n");

		private static byte[] successStatusLine1_1 = Encoding.UTF8.GetBytes("HTTP/1.1 206 Partial Content");

		private static byte[] redirectionField = Encoding.UTF8.GetBytes("Location");

		private static byte[] contentLengthField = Encoding.UTF8.GetBytes("Content-Length");

		private int fileSizeBeforeDownload;

		private int detectedHeaderLength = -1;

		private int detecedContentLength = -1;

		private string locationUrl = string.Empty;

		private static int kMaxRedirectTimes = 3;

		private string url = string.Empty;

		private int size = -1;

		private string md5 = string.Empty;

		private string destFile = string.Empty;

		private int curRedirectTimes;

		private string tmpFile = string.Empty;

		private Action<int, Dictionary<string, object>> theAction;

		public DownloadHandler(string url, int size, string md5, string destFile, int redirectTimes, Action<int, Dictionary<string, object>> action)
		{
			this.url = url;
			this.size = size;
			this.md5 = md5;
			this.destFile = destFile;
			this.theAction = action;
			this.curRedirectTimes = redirectTimes;
			this.tmpFile = Pandora.Instance.GetTempPath() + "/" + Path.GetFileName(destFile);
		}

		public override void OnConnected()
		{
			Logger.DEBUG(string.Empty);
			long uniqueSocketId = base.GetUniqueSocketId();
			Action<int, Dictionary<string, object>> action = delegate(int status, Dictionary<string, object> content)
			{
				try
				{
					if (!File.Exists(this.tmpFile))
					{
						using (FileStream fileStream = File.Create(this.tmpFile))
						{
							fileStream.Close();
						}
					}
					FileInfo fileInfo = new FileInfo(this.tmpFile);
					this.fileSizeBeforeDownload = (int)fileInfo.Length;
					if (this.size != 0 && this.fileSizeBeforeDownload >= this.size)
					{
						File.Delete(this.tmpFile);
						using (FileStream fileStream2 = File.Create(this.tmpFile))
						{
							fileStream2.Close();
							this.fileSizeBeforeDownload = 0;
						}
					}
					Uri uri = new Uri(this.url);
					string text = string.Concat(new string[]
					{
						"GET ",
						uri.AbsolutePath,
						" HTTP/1.1\r\nHost:",
						uri.Host,
						"\r\nAccept:*/*\r\nUser-Agent:Pandora(",
						Pandora.Instance.GetSDKVersion(),
						")\r\nRange:bytes=",
						this.fileSizeBeforeDownload.ToString(),
						"-\r\nConnection: keep-alive\r\n\r\n"
					});
					Logger.DEBUG(text);
					byte[] bytes = Encoding.UTF8.GetBytes(text);
					Pandora.Instance.GetNetLogic().SendPacket(uniqueSocketId, bytes);
				}
				catch (Exception ex)
				{
					Pandora.Instance.GetNetLogic().Close(uniqueSocketId);
					Logger.ERROR(ex.StackTrace);
				}
			};
			Message message = new Message();
			message.status = 0;
			message.action = action;
			Pandora.Instance.GetNetLogic().EnqueueDrive(message);
		}

		public override int DetectPacketSize(byte[] receivedData, int dataLen)
		{
			if (this.detectedHeaderLength < 0)
			{
				for (int i = 0; i < dataLen - DownloadHandler.httpHeaderEnd.Length + 1; i++)
				{
					bool flag = true;
					for (int j = 0; j < DownloadHandler.httpHeaderEnd.Length; j++)
					{
						if (receivedData[i + j] != DownloadHandler.httpHeaderEnd[j])
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						this.detectedHeaderLength = i + DownloadHandler.httpHeaderEnd.Length;
						break;
					}
				}
			}
			if (this.detectedHeaderLength < 0)
			{
				if (dataLen > DownloadHandler.kMaxHttpHeaderLength)
				{
					Logger.ERROR(string.Empty);
					return -1;
				}
				return 0;
			}
			else
			{
				bool flag2 = true;
				for (int k = 0; k < DownloadHandler.successStatusLine1_1.Length; k++)
				{
					if (k >= dataLen || receivedData[k] != DownloadHandler.successStatusLine1_1[k])
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					if (this.detecedContentLength < 0)
					{
						this.detecedContentLength = this.GetContentLength(receivedData, this.detectedHeaderLength);
					}
					if (this.detecedContentLength > 0)
					{
						try
						{
							FileInfo fileInfo = new FileInfo(this.tmpFile);
							int num = (int)fileInfo.Length;
							int num2 = num - this.fileSizeBeforeDownload;
							int num3 = dataLen - this.detectedHeaderLength;
							if (num3 > num2)
							{
								using (FileStream fileStream = new FileStream(this.tmpFile, FileMode.Append, FileAccess.Write))
								{
									fileStream.Write(receivedData, this.detectedHeaderLength + num2, num3 - num2);
									fileStream.Close();
								}
							}
							int result;
							if (this.detectedHeaderLength + this.detecedContentLength <= dataLen)
							{
								result = this.detectedHeaderLength + this.detecedContentLength;
								return result;
							}
							result = 0;
							return result;
						}
						catch (Exception ex)
						{
							Logger.ERROR(ex.StackTrace);
							int result = -2;
							return result;
						}
					}
					Logger.ERROR(string.Empty);
					return -2;
				}
				this.locationUrl = this.GetLocationUrl(receivedData, this.detectedHeaderLength);
				if (this.locationUrl.Length > 0)
				{
					return this.detectedHeaderLength;
				}
				Logger.ERROR(string.Empty);
				return -3;
			}
		}

		public override void OnReceived(Packet thePacket)
		{
			Logger.DEBUG(string.Empty);
			if (this.locationUrl.Length > 0)
			{
				Logger.DEBUG(this.locationUrl);
				if (this.curRedirectTimes <= DownloadHandler.kMaxRedirectTimes)
				{
					Logger.DEBUG(this.locationUrl);
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					dictionary["locationUrl"] = this.locationUrl;
					Message message = new Message();
					message.status = 100;
					message.action = this.theAction;
					message.content = dictionary;
					Pandora.Instance.GetNetLogic().EnqueueDrive(message);
					this.theAction = null;
				}
			}
			else
			{
				try
				{
					Logger.DEBUG(this.url);
					FileInfo fileInfo = new FileInfo(this.tmpFile);
					int num = (int)fileInfo.Length;
					using (FileStream fileStream = new FileStream(this.tmpFile, FileMode.Open))
					{
						byte[] array = new MD5CryptoServiceProvider().ComputeHash(fileStream);
						fileStream.Close();
						StringBuilder stringBuilder = new StringBuilder();
						for (int i = 0; i < array.Length; i++)
						{
							stringBuilder.Append(array[i].ToString("X2"));
						}
						string text = stringBuilder.ToString();
						string str = "pandora20151019";
						string s = str + text;
						byte[] bytes = Encoding.UTF8.GetBytes(s);
						MemoryStream memoryStream = new MemoryStream();
						memoryStream.Seek(0L, SeekOrigin.Begin);
						memoryStream.Write(bytes, 0, bytes.Length);
						memoryStream.Seek(0L, SeekOrigin.Begin);
						array = new MD5CryptoServiceProvider().ComputeHash(memoryStream);
						memoryStream.Dispose();
						stringBuilder.Remove(0, text.Length);
						for (int j = 0; j < array.Length; j++)
						{
							stringBuilder.Append(array[j].ToString("X2"));
						}
						string b = stringBuilder.ToString();
						if ((this.size == 0 || this.size == num) && (this.md5 == string.Empty || this.md5 == b))
						{
							if (File.Exists(this.destFile))
							{
								File.Delete(this.destFile);
							}
							File.Move(this.tmpFile, this.destFile);
							Message message2 = new Message();
							message2.status = 0;
							message2.action = this.theAction;
							Pandora.Instance.GetNetLogic().EnqueueDrive(message2);
							this.theAction = null;
						}
					}
				}
				catch (Exception ex)
				{
					Logger.ERROR(ex.StackTrace);
				}
				try
				{
					if (File.Exists(this.tmpFile))
					{
						File.Delete(this.tmpFile);
					}
				}
				catch (Exception ex2)
				{
					Logger.ERROR(ex2.StackTrace);
				}
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
			Pandora.Instance.GetNetLogic().EnqueueDrive(message);
		}

		private int GetContentLength(byte[] receivedData, int dataLen)
		{
			Logger.DEBUG(string.Empty);
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			for (int i = 0; i < dataLen - DownloadHandler.contentLengthField.Length + 1; i++)
			{
				bool flag = true;
				for (int j = 0; j < DownloadHandler.contentLengthField.Length; j++)
				{
					if (receivedData[i + j] != DownloadHandler.contentLengthField[j])
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				Logger.WARN(string.Empty);
				return -1;
			}
			for (int k = num + DownloadHandler.contentLengthField.Length; k < dataLen; k++)
			{
				if (receivedData[k] >= 48 && receivedData[k] <= 57)
				{
					if (num2 < 0)
					{
						num2 = k;
					}
				}
				else if (num2 >= 0 && num3 < 0)
				{
					num3 = k;
					break;
				}
			}
			if (num2 < 0 || num3 < 0 || num2 >= num3)
			{
				Logger.ERROR(string.Empty);
				return -1;
			}
			int result;
			try
			{
				string @string = Encoding.UTF8.GetString(receivedData, num2, num3 - num2);
				int num4 = Convert.ToInt32(@string);
				if (num4 > 0)
				{
					result = num4;
				}
				else
				{
					Logger.ERROR(string.Empty);
					result = -1;
				}
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.StackTrace);
				result = -1;
			}
			return result;
		}

		private string GetLocationUrl(byte[] receivedData, int dataLen)
		{
			Logger.DEBUG(string.Empty);
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			for (int i = 0; i < dataLen - DownloadHandler.redirectionField.Length + 1; i++)
			{
				bool flag = true;
				for (int j = 0; j < DownloadHandler.redirectionField.Length; j++)
				{
					if (receivedData[i + j] != DownloadHandler.redirectionField[j])
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				Logger.WARN(string.Empty);
				return string.Empty;
			}
			for (int k = num + DownloadHandler.redirectionField.Length; k < dataLen; k++)
			{
				if (receivedData[k] != 58 && receivedData[k] != 32 && num2 < 0)
				{
					num2 = k;
				}
				if (num2 >= 0 && receivedData[k] == 13)
				{
					num3 = k;
					break;
				}
			}
			if (num2 < 0 || num3 < 0 || num2 >= num3)
			{
				Logger.ERROR(string.Empty);
				return string.Empty;
			}
			string @string = Encoding.UTF8.GetString(receivedData, num2, num3 - num2);
			Logger.DEBUG(@string);
			return @string;
		}
	}
}
