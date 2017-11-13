using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal class ApolloConnector : ApolloObject, IApolloConnector, IApolloServiceBase
	{
		private byte[] tempReadBuffer;

		public event ConnectEventHandler ConnectEvent
		{
			[MethodImpl(32)]
			add
			{
				this.ConnectEvent = (ConnectEventHandler)Delegate.Combine(this.ConnectEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.ConnectEvent = (ConnectEventHandler)Delegate.Remove(this.ConnectEvent, value);
			}
		}

		public event ReconnectEventHandler ReconnectEvent
		{
			[MethodImpl(32)]
			add
			{
				this.ReconnectEvent = (ReconnectEventHandler)Delegate.Combine(this.ReconnectEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.ReconnectEvent = (ReconnectEventHandler)Delegate.Remove(this.ReconnectEvent, value);
			}
		}

		public event DisconnectEventHandler DisconnectEvent
		{
			[MethodImpl(32)]
			add
			{
				this.DisconnectEvent = (DisconnectEventHandler)Delegate.Combine(this.DisconnectEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.DisconnectEvent = (DisconnectEventHandler)Delegate.Remove(this.DisconnectEvent, value);
			}
		}

		public event RecvedDataHandler RecvedDataEvent
		{
			[MethodImpl(32)]
			add
			{
				this.RecvedDataEvent = (RecvedDataHandler)Delegate.Combine(this.RecvedDataEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.RecvedDataEvent = (RecvedDataHandler)Delegate.Remove(this.RecvedDataEvent, value);
			}
		}

		public event RecvedUdpDataHandler RecvedUdpDataEvent
		{
			[MethodImpl(32)]
			add
			{
				this.RecvedUdpDataEvent = (RecvedUdpDataHandler)Delegate.Combine(this.RecvedUdpDataEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.RecvedUdpDataEvent = (RecvedUdpDataHandler)Delegate.Remove(this.RecvedUdpDataEvent, value);
			}
		}

		public event ConnectorErrorEventHandler ErrorEvent
		{
			[MethodImpl(32)]
			add
			{
				this.ErrorEvent = (ConnectorErrorEventHandler)Delegate.Combine(this.ErrorEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.ErrorEvent = (ConnectorErrorEventHandler)Delegate.Remove(this.ErrorEvent, value);
			}
		}

		public event RouteChangedEventHandler RouteChangedEvent
		{
			[MethodImpl(32)]
			add
			{
				this.RouteChangedEvent = (RouteChangedEventHandler)Delegate.Combine(this.RouteChangedEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.RouteChangedEvent = (RouteChangedEventHandler)Delegate.Remove(this.RouteChangedEvent, value);
			}
		}

		public bool Connected
		{
			get;
			private set;
		}

		public ApolloLoginInfo LoginInfo
		{
			get;
			private set;
		}

		public ApolloConnector()
		{
			this.Connected = false;
		}

		~ApolloConnector()
		{
			ADebug.Log(" ~ApolloConnector()");
			this.Disconnect();
		}

		public ApolloResult Initialize(ApolloPlatform platform, uint permission, string url)
		{
			ADebug.Log(string.Concat(new object[]
			{
				"Connector Initialize:",
				platform,
				" url:",
				url
			}));
			if (platform == ApolloPlatform.WTLogin)
			{
			}
			return (ApolloResult)ApolloConnector.apollo_connector_Initialize(base.ObjectId, platform, permission, url);
		}

		public ApolloResult SetSecurityInfo(ApolloEncryptMethod encyptMethod, ApolloKeyMaking keyMakingMethod, string dhp)
		{
			ADebug.Log(string.Concat(new object[]
			{
				"SetSecurityInfo encyptMethod:",
				encyptMethod,
				" keyMakingMethod:",
				keyMakingMethod,
				" dh:",
				dhp
			}));
			return ApolloConnector.apollo_connector_setSecurityInfo(base.ObjectId, encyptMethod, keyMakingMethod, dhp);
		}

		public ApolloResult SetRouteInfo(ApolloRouteInfoBase routeInfo)
		{
			if (routeInfo == null)
			{
				return ApolloResult.InvalidArgument;
			}
			byte[] array;
			routeInfo.Encode(out array);
			if (array == null)
			{
				ADebug.LogError("WriteData Encode error!");
				return ApolloResult.InnerError;
			}
			return ApolloConnector.apollo_connector_setRouteInfo(base.ObjectId, array, array.Length);
		}

		public ApolloResult Connect()
		{
			return this.Connect(30u);
		}

		public ApolloResult Connect(uint timeout)
		{
			ADebug.Log("Connect");
			return ApolloConnector.apollo_connector_connect(base.ObjectId, timeout);
		}

		public ApolloResult Reconnect()
		{
			ADebug.Log("Reconnect");
			return this.Reconnect(30u);
		}

		public ApolloResult Reconnect(uint timeout)
		{
			ADebug.Log("Reconnect");
			return ApolloConnector.apollo_connector_reconnect(base.ObjectId, timeout);
		}

		public ApolloResult Disconnect()
		{
			ADebug.Log("Disconnect");
			return ApolloConnector.apollo_connector_disconnect(base.ObjectId);
		}

		private void OnConnectProc(string msg)
		{
			ADebug.Log("c#:OnConnectProc: " + msg);
			if (string.IsNullOrEmpty(msg))
			{
				ADebug.LogError("OnConnectProc msg is null");
				return;
			}
			ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
			ApolloResult @int = (ApolloResult)apolloStringParser.GetInt("Result", 6);
			this.LoginInfo = apolloStringParser.GetObject<ApolloLoginInfo>("LoginInfo");
			if (@int == ApolloResult.Success)
			{
				this.Connected = true;
			}
			else
			{
				this.Connected = false;
			}
			ADebug.Log(string.Concat(new object[]
			{
				"c#:OnConnectProc: ",
				@int,
				" loginfo:",
				this.LoginInfo
			}));
			if (this.LoginInfo != null && this.LoginInfo.AccountInfo != null && this.LoginInfo.AccountInfo.TokenList != null)
			{
				ADebug.Log(string.Concat(new object[]
				{
					"C# logininfo| platform:",
					this.LoginInfo.AccountInfo.Platform,
					" openid:",
					this.LoginInfo.AccountInfo.OpenId,
					" tokensize:",
					this.LoginInfo.AccountInfo.TokenList.Count,
					" pf:",
					this.LoginInfo.AccountInfo.Pf,
					" pfkey:",
					this.LoginInfo.AccountInfo.PfKey
				}));
			}
			if (this.ConnectEvent != null)
			{
				try
				{
					this.ConnectEvent(@int, this.LoginInfo);
				}
				catch (Exception exception)
				{
					ADebug.LogException(exception);
				}
			}
			else
			{
				ADebug.Log("OnConnectProc ConnectEvent is null");
			}
		}

		private void OnReconnectProc(string msg)
		{
			ADebug.Log("c#:OnReconnectProc: " + msg);
			ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
			ApolloResult @int = (ApolloResult)apolloStringParser.GetInt("Result", 6);
			if (@int == ApolloResult.Success)
			{
				this.Connected = true;
			}
			else
			{
				this.Connected = false;
			}
			if (this.ReconnectEvent != null)
			{
				try
				{
					this.ReconnectEvent(@int);
				}
				catch (Exception exception)
				{
					ADebug.LogException(exception);
				}
			}
			else
			{
				ADebug.Log("OnReconnectProc ReconnectEvent is null");
			}
		}

		private void OnDisconnectProc(string msg)
		{
			ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
			ApolloResult @int = (ApolloResult)apolloStringParser.GetInt("Result");
			if (@int == ApolloResult.Success)
			{
				this.Connected = false;
			}
			if (this.DisconnectEvent != null)
			{
				try
				{
					this.DisconnectEvent(@int);
				}
				catch (Exception exception)
				{
					ADebug.LogException(exception);
				}
			}
		}

		private void OnRouteChangedProc(string msg)
		{
			ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
			ulong uInt = apolloStringParser.GetUInt64("serverId");
			if (this.RouteChangedEvent != null)
			{
				try
				{
					this.RouteChangedEvent(uInt);
				}
				catch (Exception exception)
				{
					ADebug.LogException(exception);
				}
			}
		}

		private void OnUdpDataRecvedProc(string msg)
		{
			if (this.RecvedUdpDataEvent != null)
			{
				try
				{
					this.RecvedUdpDataEvent();
				}
				catch (Exception exception)
				{
					ADebug.LogException(exception);
				}
			}
		}

		private void OnDataRecvedProc(string msg)
		{
			this.Connected = true;
			if (this.RecvedDataEvent != null)
			{
				try
				{
					this.RecvedDataEvent();
				}
				catch (Exception exception)
				{
					ADebug.LogException(exception);
				}
			}
		}

		private void OnConnectorErrorProc(string msg)
		{
			ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
			ApolloResult @int = (ApolloResult)apolloStringParser.GetInt("Result", 6);
			ADebug.LogError("OnConnectorErrorProc:" + @int);
			this.Connected = false;
			if (this.ErrorEvent != null)
			{
				try
				{
					this.ErrorEvent(@int);
				}
				catch (Exception exception)
				{
					ADebug.LogException(exception);
				}
			}
		}

		public ApolloResult WriteData(byte[] data, int len = -1)
		{
			if (!this.Connected)
			{
				return ApolloResult.NoConnection;
			}
			if (len == -1)
			{
				len = data.Length;
			}
			return ApolloConnector.apollo_connector_writeData(base.ObjectId, data, len);
		}

		public ApolloResult WriteUdpData(byte[] data, int len = -1)
		{
			if (!this.Connected)
			{
				return ApolloResult.NoConnection;
			}
			if (len == -1)
			{
				len = data.Length;
			}
			return ApolloConnector.apollo_connector_writeUdpData(base.ObjectId, data, len);
		}

		public ApolloResult WriteData(byte[] data, int len, ApolloRouteInfoBase routeInfo, bool allowLost = false)
		{
			if (routeInfo == null)
			{
				return ApolloResult.InvalidArgument;
			}
			if (!this.Connected)
			{
				return ApolloResult.NoConnection;
			}
			if (len == -1)
			{
				len = data.Length;
			}
			byte[] array;
			routeInfo.Encode(out array);
			if (array == null)
			{
				ADebug.LogError("WriteData Encode error!");
				return ApolloResult.InnerError;
			}
			return ApolloConnector.apollo_connector_writeData_with_route_info(base.ObjectId, data, len, array, array.Length, allowLost);
		}

		public ApolloResult ReadData(out byte[] buffer, out int realLength)
		{
			buffer = null;
			realLength = 0;
			if (!this.Connected)
			{
			}
			if (this.tempReadBuffer == null)
			{
				this.tempReadBuffer = new byte[ApolloCommon.ApolloInfo.MaxMessageBufferSize];
			}
			int num = this.tempReadBuffer.Length;
			ApolloResult apolloResult = ApolloConnector.apollo_connector_readData(base.ObjectId, this.tempReadBuffer, ref num);
			if (apolloResult == ApolloResult.Success)
			{
				if (num == 0)
				{
					ADebug.LogError("ReadData empty len==0");
					return ApolloResult.Empty;
				}
				buffer = this.tempReadBuffer;
				realLength = num;
			}
			return apolloResult;
		}

		public ApolloResult ReadUdpData(out byte[] buffer, out int realLength)
		{
			buffer = null;
			realLength = 0;
			if (!this.Connected)
			{
			}
			if (this.tempReadBuffer == null)
			{
				this.tempReadBuffer = new byte[ApolloCommon.ApolloInfo.MaxMessageBufferSize];
			}
			int num = this.tempReadBuffer.Length;
			ApolloResult apolloResult = ApolloConnector.apollo_connector_readUdpData(base.ObjectId, this.tempReadBuffer, ref num);
			if (apolloResult == ApolloResult.Success)
			{
				if (num == 0)
				{
					ADebug.LogError("ReadUdpData empty len==0");
					return ApolloResult.Empty;
				}
				buffer = this.tempReadBuffer;
				realLength = num;
			}
			return apolloResult;
		}

		public ApolloResult GetSessionStopReason(ref int result, ref int reason, ref int excode)
		{
			return ApolloConnector.apollo_connector_getstopreason(base.ObjectId, ref result, ref reason, ref excode);
		}

		public ApolloResult ReportAccessToken(string accessToken, ulong expire)
		{
			return ApolloConnector.apollo_connector_report_accesstoken(base.ObjectId, accessToken, (uint)expire);
		}

		public void SetClientType(ClientType type)
		{
			ApolloConnector.apollo_connector_set_clientType(base.ObjectId, type);
		}

		public void SetProtocolVersion(int headVersion, int bodyVersion)
		{
			ApolloConnector.apollo_connector_set_protocol_version(base.ObjectId, headVersion, bodyVersion);
		}

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern int apollo_connector_Initialize(ulong objId, ApolloPlatform platform, uint permission, [MarshalAs(20)] string pszSvrUrl);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_setSecurityInfo(ulong objId, ApolloEncryptMethod encyptMethod, ApolloKeyMaking keyMakingMethod, [MarshalAs(20)] string pszDHP);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_setRouteInfo(ulong objId, [MarshalAs(42)] byte[] buff, int size);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_connect(ulong objId, uint timeout);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_reconnect(ulong objId, uint timeout);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_disconnect(ulong objId);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_writeData(ulong objId, [MarshalAs(42)] byte[] buff, int size);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_writeUdpData(ulong objId, [MarshalAs(42)] byte[] buff, int size);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_writeData_with_route_info(ulong objId, [MarshalAs(42)] byte[] buff, int size, [MarshalAs(42)] byte[] routeData, int routeDataLen, bool allowLost);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_readData(ulong objId, [MarshalAs(42)] byte[] buff, ref int size);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_readUdpData(ulong objId, [MarshalAs(42)] byte[] buff, ref int size);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_getstopreason(ulong objId, ref int result, ref int reason, ref int excode);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_report_accesstoken(ulong objId, [MarshalAs(20)] string atk, uint expire);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_set_clientType(ulong objId, ClientType type);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern ApolloResult apollo_connector_set_protocol_version(ulong objId, int headVersion, int bodyVersion);
	}
}
