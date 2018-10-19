using System;

namespace Apollo
{
	public interface IApolloConnector : IApolloServiceBase
	{
		event ConnectEventHandler ConnectEvent;

		event ReconnectEventHandler ReconnectEvent;

		event DisconnectEventHandler DisconnectEvent;

		event RecvedDataHandler RecvedDataEvent;

		event RecvedUdpDataHandler RecvedUdpDataEvent;

		event ConnectorErrorEventHandler ErrorEvent;

		event RouteChangedEventHandler RouteChangedEvent;

		ApolloLoginInfo LoginInfo
		{
			get;
		}

		bool Connected
		{
			get;
		}

		ApolloResult SetSecurityInfo(ApolloEncryptMethod EncyptMethod, ApolloKeyMaking KeyMakingMethod, string DHP);

		ApolloResult SetRouteInfo(ApolloRouteInfoBase routeInfo);

		ApolloResult Connect();

		ApolloResult Connect(uint timeout);

		ApolloResult Reconnect();

		ApolloResult Reconnect(uint timeout);

		ApolloResult Disconnect();

		ApolloResult WriteData(byte[] data, int len = -1);

		ApolloResult WriteUdpData(byte[] data, int len = -1);

		ApolloResult WriteData(byte[] data, int len, ApolloRouteInfoBase routeInfo, bool allowLost = false);

		ApolloResult ReadData(out byte[] buffer, out int realLength);

		ApolloResult ReadUdpData(out byte[] buffer, out int realLength);

		ApolloResult GetSessionStopReason(ref int result, ref int reason, ref int excode);

		ApolloResult ReportAccessToken(string accessToken, ulong expire);

		void SetClientType(ClientType type);

		void SetProtocolVersion(int headVersion, int bodyVersion);
	}
}
