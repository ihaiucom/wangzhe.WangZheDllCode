using System;
using System.Collections.Generic;
using System.Text;

namespace Apollo
{
	public class ApolloHttpResponse : ApolloObject, IApolloHttpResponse
	{
		private string version;

		private string status;

		private string statusMsg;

		private byte[] data = new byte[1048576];

		private uint datalen;

		private Dictionary<string, string> headers = new Dictionary<string, string>();

		public string GetHttpVersion()
		{
			return this.version;
		}

		public ApolloResult SetHttpVersion(string version)
		{
			this.version = version;
			return ApolloResult.Success;
		}

		public string GetStatus()
		{
			return this.status;
		}

		public ApolloResult SetStatus(string status)
		{
			this.status = status;
			return ApolloResult.Success;
		}

		public string GetStatusMessage()
		{
			return this.statusMsg;
		}

		public ApolloResult SetStatusMessage(string msg)
		{
			this.statusMsg = msg;
			return ApolloResult.Success;
		}

		public string GetHeader(string name)
		{
			if (!this.headers.ContainsKey(name))
			{
				return string.Empty;
			}
			return this.headers.get_Item(name);
		}

		public ApolloResult SetHeader(string name, string value)
		{
			if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
			{
				return ApolloResult.HttpBadHeader;
			}
			try
			{
				this.headers.Add(name, value);
			}
			catch (ArgumentException)
			{
				this.headers.set_Item(name, value);
			}
			return ApolloResult.Success;
		}

		public ApolloResult SetData(byte[] data, uint datalen)
		{
			if (datalen >= 1048576u)
			{
				ADebug.Log("ApolloHttpResponse data is too long, data is to cut off");
				Array.Copy(this.data, 0, data, 0, 1048576);
				this.datalen = datalen;
			}
			else
			{
				this.data = data;
				this.datalen = datalen;
			}
			return ApolloResult.Success;
		}

		public byte[] GetData()
		{
			byte[] array = new byte[this.datalen];
			Array.Copy(this.data, array, (long)((ulong)this.datalen));
			return array;
		}

		public override string ToString()
		{
			string text = string.Empty;
			text += this.version;
			text += " ";
			text += this.status;
			text += " ";
			text += this.statusMsg;
			text += "\r\n";
			text += "\r\n";
			using (Dictionary<string, string>.Enumerator enumerator = this.headers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> current = enumerator.get_Current();
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						current.get_Key(),
						" : ",
						current.get_Value(),
						"\n"
					});
				}
			}
			text += "\n";
			string @string = Encoding.get_ASCII().GetString(this.data, 0, Convert.ToInt32(this.datalen));
			text += @string;
			return text;
		}
	}
}
