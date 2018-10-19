using apollo_http_object;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Apollo
{
	public class ApolloHttpRequest : ApolloObject, IApolloHttpRequest
	{
		private byte[] URL = new byte[1];

		private byte[] version = Encoding.UTF8.GetBytes("HTTP/1.1");

		private byte[] method = Encoding.UTF8.GetBytes("GET");

		private byte[] data = new byte[1];

		private float timeout;

		private bool gotTimeout;

		private Dictionary<string, string> headers = new Dictionary<string, string>();

		private ListView<ApolloHttpResponse> responses = new ListView<ApolloHttpResponse>();

		private IApolloTalker talker;

		private IApolloConnector connector;

		public event OnRespondHandler ResponseEvent;

		public ApolloHttpRequest(IApolloConnector connector)
		{
			this.connector = connector;
			if (connector == null)
			{
				throw new Exception("Invalid Argument");
			}
			this.talker = IApollo.Instance.CreateTalker(connector);
			this.SetMethod("GET");
			this.SetHeader("apollo-server-ip", "127.0.0.1");
		}

		public IApolloTalker GetTalker()
		{
			return this.talker;
		}

		public string ByteArray2String(byte[] bytearray)
		{
			string @string = Encoding.UTF8.GetString(bytearray);
			if (string.IsNullOrEmpty(@string))
			{
				return string.Empty;
			}
			return @string.Trim(new char[1]);
		}

		public void EnableAutoUpdate(bool enable)
		{
			if (enable)
			{
				this.talker.AutoUpdate = true;
			}
			else
			{
				this.talker.AutoUpdate = false;
			}
		}

		public void SetTimeout(float timeout)
		{
			this.timeout = timeout;
		}

		public override void Update()
		{
			if (this.connector == null)
			{
				return;
			}
			this.talker.Update(1);
		}

		public ApolloResult SendRequest()
		{
			ADebug.Log("Send request to tconnd through talker");
			HttpReq httpReq = this.PackRequest();
			if (httpReq == null)
			{
				ADebug.Log("httpReq is null");
			}
			return this.talker.Send<HttpRsp>(httpReq, delegate(object req, TalkerEventArgs<HttpRsp> e)
			{
				ADebug.Log("Get Response form server");
				this.DealRsp(e.Response, e.Result);
			}, null, this.timeout);
		}

		private void DealRsp(HttpRsp rsp, ApolloResult rst)
		{
			if (rst == ApolloResult.Success)
			{
				ApolloHttpResponse apolloHttpResponse = new ApolloHttpResponse();
				string httpVersion = this.ByteArray2String(rsp.stResponseStatus.szHttpVersion);
				apolloHttpResponse.SetHttpVersion(httpVersion);
				string status = this.ByteArray2String(rsp.stResponseStatus.szStatusCode);
				apolloHttpResponse.SetStatus(status);
				string statusMessage = this.ByteArray2String(rsp.stResponseStatus.szReasonPhrase);
				apolloHttpResponse.SetStatusMessage(statusMessage);
				int num = 0;
				while ((long)num < (long)((ulong)rsp.stHttpHeaders.dwHeaderCount))
				{
					HeaderUnit headerUnit = rsp.stHttpHeaders.astHeaderUnit[num];
					string name = this.ByteArray2String(headerUnit.szHeaderName);
					string value = this.ByteArray2String(headerUnit.szHeaderContent);
					apolloHttpResponse.SetHeader(name, value);
					num++;
				}
				apolloHttpResponse.SetData(rsp.stResponseContent.szData, rsp.stResponseContent.dwDataLen);
				ADebug.Log("Get Result Response :" + apolloHttpResponse.ToString());
				this.responses.Add(apolloHttpResponse);
				if (this.ResponseEvent != null)
				{
					IApolloHttpResponse rsp2 = apolloHttpResponse;
					this.ResponseEvent(rsp2, rst);
				}
			}
			else if (rst == ApolloResult.Timeout)
			{
				this.gotTimeout = true;
			}
			else
			{
				ADebug.LogError("Got recv error :" + rst);
			}
		}

		public HttpReq PackRequest()
		{
			RequestLine requestLine = new RequestLine();
			requestLine.szRequestMethod = this.method;
			requestLine.szRequestUri = this.URL;
			requestLine.szHttpVersion = this.version;
			ListLinqView<HeaderUnit> listLinqView = new ListLinqView<HeaderUnit>();
			foreach (KeyValuePair<string, string> current in this.headers)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(current.Key);
				byte[] bytes2 = Encoding.UTF8.GetBytes(current.Value);
				listLinqView.Add(new HeaderUnit
				{
					szHeaderName = bytes,
					szHeaderContent = bytes2
				});
			}
			HttpHeaders httpHeaders = new HttpHeaders();
			httpHeaders.astHeaderUnit = listLinqView.ToArray();
			httpHeaders.dwHeaderCount = (uint)httpHeaders.astHeaderUnit.Length;
			RequestContent requestContent = new RequestContent();
			requestContent.szData = this.data;
			requestContent.dwDataLen = (uint)this.data.Length;
			HttpReq httpReq = new HttpReq();
			httpReq.stRequestLine = requestLine;
			httpReq.stHttpHeaders = httpHeaders;
			httpReq.stRequestContent = requestContent;
			ADebug.Log("send request :" + this.ToString());
			return httpReq;
		}

		public ApolloResult SetHttpVersion(string version)
		{
			if (!ApolloHttpVersion.Valied(version))
			{
				return ApolloResult.HttpBadVersion;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(version);
			this.version = bytes;
			return ApolloResult.Success;
		}

		public string GetHttpVersion()
		{
			return this.ByteArray2String(this.version);
		}

		private bool ValiedURL(string URL)
		{
			return string.IsNullOrEmpty(URL) || (URL.Length > 1 && (URL.Substring(0, 1) == "/" || URL.Substring(0, 1) == "?")) || (URL.Length > 7 && URL.Substring(0, 7) == "http://");
		}

		public ApolloResult SetURL(string URL)
		{
			if (!this.ValiedURL(URL))
			{
				return ApolloResult.HttpBadURL;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(URL);
			this.URL = bytes;
			return ApolloResult.Success;
		}

		public string GetURL()
		{
			return this.ByteArray2String(this.URL);
		}

		public ApolloResult SetMethod(string method)
		{
			if (!ApolloHttpMethod.Valied(method))
			{
				return ApolloResult.HttpBadMethod;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(method);
			this.method = bytes;
			return ApolloResult.Success;
		}

		public string GetMethod()
		{
			return this.ByteArray2String(this.method);
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
				this.headers[name] = value;
			}
			return ApolloResult.Success;
		}

		public string GetHeader(string name)
		{
			if (!this.headers.ContainsKey(name))
			{
				return string.Empty;
			}
			return this.headers[name];
		}

		public ApolloResult SetData(byte[] data)
		{
			if (data == null)
			{
				return ApolloResult.Success;
			}
			if (data.Length == 0 || data.Length >= 8096)
			{
				return ApolloResult.HttpReqToLong;
			}
			this.data = data;
			return ApolloResult.Success;
		}

		public byte[] GetData()
		{
			return this.data;
		}

		public override string ToString()
		{
			string text = string.Empty;
			string text2 = this.ByteArray2String(this.method);
			string text3 = this.ByteArray2String(this.version);
			string text4 = this.ByteArray2String(this.URL);
			string text5 = text;
			text = string.Concat(new string[]
			{
				text5,
				text2,
				" ",
				text4,
				" ",
				text3,
				" \r\n\r\n"
			});
			foreach (KeyValuePair<string, string> current in this.headers)
			{
				text5 = text;
				text = string.Concat(new string[]
				{
					text5,
					current.Key,
					" : ",
					current.Value,
					"\n"
				});
			}
			text += "\n";
			text += this.data;
			return text;
		}

		public IApolloHttpResponse GetResponse()
		{
			if (this.responses.Count != 0)
			{
				IApolloHttpResponse result = this.responses[0];
				this.responses.RemoveAt(0);
				return result;
			}
			if (this.gotTimeout)
			{
				this.gotTimeout = false;
				throw new TimeoutException("ApolloHttpClient Get Response timeout!");
			}
			ADebug.Log("IApolloHttpResponse:Response is null");
			return null;
		}
	}
}
