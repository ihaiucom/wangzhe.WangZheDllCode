using System;

public enum ClientState
{
	Uninitialized,
	PeerCreated,
	Queued,
	Authenticated,
	JoinedLobby,
	DisconnectingFromMasterserver,
	ConnectingToGameserver,
	ConnectedToGameserver,
	Joining,
	Joined,
	Leaving,
	DisconnectingFromGameserver,
	ConnectingToMasterserver,
	QueuedComingFromGameserver,
	Disconnecting,
	Disconnected,
	ConnectedToMaster,
	ConnectingToNameServer,
	ConnectedToNameServer,
	DisconnectingFromNameServer,
	Authenticating
}
