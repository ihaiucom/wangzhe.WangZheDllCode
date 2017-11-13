using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;

internal class LoadBalancingPeer : PhotonPeer
{
	private enum RoomOptionBit
	{
		CheckUserOnJoin = 1,
		DeleteCacheOnLeave,
		SuppressRoomEvents = 4,
		PublishUserId = 8,
		DeleteNullProps = 16,
		BroadcastPropsChangeToAll = 32
	}

	private readonly Dictionary<byte, object> opParameters = new Dictionary<byte, object>();

	internal bool IsProtocolSecure
	{
		get
		{
			return base.get_UsedProtocol() == 5;
		}
	}

	public LoadBalancingPeer(ConnectionProtocol protocolType) : base(protocolType)
	{
	}

	public LoadBalancingPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType) : this(protocolType)
	{
		base.set_Listener(listener);
	}

	public virtual bool OpGetRegions(string appId)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.set_Item(224, appId);
		return this.OpCustom(220, dictionary, true, 0, true);
	}

	public virtual bool OpJoinLobby(TypedLobby lobby = null)
	{
		if (this.DebugOut >= 3)
		{
			base.get_Listener().DebugReturn(3, "OpJoinLobby()");
		}
		Dictionary<byte, object> dictionary = null;
		if (lobby != null && !lobby.IsDefault)
		{
			dictionary = new Dictionary<byte, object>();
			dictionary.set_Item(213, lobby.Name);
			dictionary.set_Item(212, (byte)lobby.Type);
		}
		return this.OpCustom(229, dictionary, true);
	}

	public virtual bool OpLeaveLobby()
	{
		if (this.DebugOut >= 3)
		{
			base.get_Listener().DebugReturn(3, "OpLeaveLobby()");
		}
		return this.OpCustom(228, null, true);
	}

	private void RoomOptionsToOpParameters(Dictionary<byte, object> op, RoomOptions roomOptions)
	{
		if (roomOptions == null)
		{
			roomOptions = new RoomOptions();
		}
		Hashtable hashtable = new Hashtable();
		hashtable.set_Item(253, roomOptions.IsOpen);
		hashtable.set_Item(254, roomOptions.IsVisible);
		hashtable.set_Item(250, (roomOptions.CustomRoomPropertiesForLobby != null) ? roomOptions.CustomRoomPropertiesForLobby : new string[0]);
		hashtable.MergeStringKeys(roomOptions.CustomRoomProperties);
		if (roomOptions.MaxPlayers > 0)
		{
			hashtable.set_Item(255, roomOptions.MaxPlayers);
		}
		op.set_Item(248, hashtable);
		int num = 0;
		op.set_Item(241, roomOptions.CleanupCacheOnLeave);
		if (roomOptions.CleanupCacheOnLeave)
		{
			num |= 2;
			hashtable.set_Item(249, true);
		}
		if (roomOptions.PlayerTtl > 0 || roomOptions.PlayerTtl == -1)
		{
			num |= 1;
			op.set_Item(232, true);
			op.set_Item(235, roomOptions.PlayerTtl);
		}
		if (roomOptions.EmptyRoomTtl > 0)
		{
			op.set_Item(236, roomOptions.EmptyRoomTtl);
		}
		if (roomOptions.SuppressRoomEvents)
		{
			num |= 4;
			op.set_Item(237, true);
		}
		if (roomOptions.Plugins != null)
		{
			op.set_Item(204, roomOptions.Plugins);
		}
		if (roomOptions.PublishUserId)
		{
			num |= 8;
			op.set_Item(239, true);
		}
		if (roomOptions.DeleteNullProperties)
		{
			num |= 16;
		}
		op.set_Item(191, num);
	}

	public virtual bool OpCreateRoom(EnterRoomParams opParams)
	{
		if (this.DebugOut >= 3)
		{
			base.get_Listener().DebugReturn(3, "OpCreateRoom()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (!string.IsNullOrEmpty(opParams.RoomName))
		{
			dictionary.set_Item(255, opParams.RoomName);
		}
		if (opParams.Lobby != null && !string.IsNullOrEmpty(opParams.Lobby.Name))
		{
			dictionary.set_Item(213, opParams.Lobby.Name);
			dictionary.set_Item(212, (byte)opParams.Lobby.Type);
		}
		if (opParams.ExpectedUsers != null && opParams.ExpectedUsers.Length > 0)
		{
			dictionary.set_Item(238, opParams.ExpectedUsers);
		}
		if (opParams.OnGameServer)
		{
			if (opParams.PlayerProperties != null && opParams.PlayerProperties.get_Count() > 0)
			{
				dictionary.set_Item(249, opParams.PlayerProperties);
				dictionary.set_Item(250, true);
			}
			this.RoomOptionsToOpParameters(dictionary, opParams.RoomOptions);
		}
		return this.OpCustom(227, dictionary, true);
	}

	public virtual bool OpJoinRoom(EnterRoomParams opParams)
	{
		if (this.DebugOut >= 3)
		{
			base.get_Listener().DebugReturn(3, "OpJoinRoom()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (!string.IsNullOrEmpty(opParams.RoomName))
		{
			dictionary.set_Item(255, opParams.RoomName);
		}
		if (opParams.CreateIfNotExists)
		{
			dictionary.set_Item(215, 1);
			if (opParams.Lobby != null)
			{
				dictionary.set_Item(213, opParams.Lobby.Name);
				dictionary.set_Item(212, (byte)opParams.Lobby.Type);
			}
		}
		if (opParams.RejoinOnly)
		{
			dictionary.set_Item(215, 3);
		}
		if (opParams.ExpectedUsers != null && opParams.ExpectedUsers.Length > 0)
		{
			dictionary.set_Item(238, opParams.ExpectedUsers);
		}
		if (opParams.OnGameServer)
		{
			if (opParams.PlayerProperties != null && opParams.PlayerProperties.get_Count() > 0)
			{
				dictionary.set_Item(249, opParams.PlayerProperties);
				dictionary.set_Item(250, true);
			}
			if (opParams.CreateIfNotExists)
			{
				this.RoomOptionsToOpParameters(dictionary, opParams.RoomOptions);
			}
		}
		return this.OpCustom(226, dictionary, true);
	}

	public virtual bool OpJoinRandomRoom(OpJoinRandomRoomParams opJoinRandomRoomParams)
	{
		if (this.DebugOut >= 3)
		{
			base.get_Listener().DebugReturn(3, "OpJoinRandomRoom()");
		}
		Hashtable hashtable = new Hashtable();
		hashtable.MergeStringKeys(opJoinRandomRoomParams.ExpectedCustomRoomProperties);
		if (opJoinRandomRoomParams.ExpectedMaxPlayers > 0)
		{
			hashtable.set_Item(255, opJoinRandomRoomParams.ExpectedMaxPlayers);
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (hashtable.get_Count() > 0)
		{
			dictionary.set_Item(248, hashtable);
		}
		if (opJoinRandomRoomParams.MatchingType != MatchmakingMode.FillRoom)
		{
			dictionary.set_Item(223, (byte)opJoinRandomRoomParams.MatchingType);
		}
		if (opJoinRandomRoomParams.TypedLobby != null && !string.IsNullOrEmpty(opJoinRandomRoomParams.TypedLobby.Name))
		{
			dictionary.set_Item(213, opJoinRandomRoomParams.TypedLobby.Name);
			dictionary.set_Item(212, (byte)opJoinRandomRoomParams.TypedLobby.Type);
		}
		if (!string.IsNullOrEmpty(opJoinRandomRoomParams.SqlLobbyFilter))
		{
			dictionary.set_Item(245, opJoinRandomRoomParams.SqlLobbyFilter);
		}
		if (opJoinRandomRoomParams.ExpectedUsers != null && opJoinRandomRoomParams.ExpectedUsers.Length > 0)
		{
			dictionary.set_Item(238, opJoinRandomRoomParams.ExpectedUsers);
		}
		return this.OpCustom(225, dictionary, true);
	}

	public virtual bool OpLeaveRoom(bool becomeInactive)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (becomeInactive)
		{
			dictionary.set_Item(233, becomeInactive);
		}
		return this.OpCustom(254, dictionary, true);
	}

	public virtual bool OpGetGameList(TypedLobby lobby, string queryData)
	{
		if (this.DebugOut >= 3)
		{
			base.get_Listener().DebugReturn(3, "OpGetGameList()");
		}
		if (lobby == null)
		{
			if (this.DebugOut >= 3)
			{
				base.get_Listener().DebugReturn(3, "OpGetGameList not sent. Lobby cannot be null.");
			}
			return false;
		}
		if (lobby.Type != LobbyType.SqlLobby)
		{
			if (this.DebugOut >= 3)
			{
				base.get_Listener().DebugReturn(3, "OpGetGameList not sent. LobbyType must be SqlLobby.");
			}
			return false;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.set_Item(213, lobby.Name);
		dictionary.set_Item(212, (byte)lobby.Type);
		dictionary.set_Item(245, queryData);
		return this.OpCustom(217, dictionary, true);
	}

	public virtual bool OpFindFriends(string[] friendsToFind)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (friendsToFind != null && friendsToFind.Length > 0)
		{
			dictionary.set_Item(1, friendsToFind);
		}
		return this.OpCustom(222, dictionary, true);
	}

	public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable actorProperties)
	{
		return this.OpSetPropertiesOfActor(actorNr, actorProperties.StripToStringKeys(), null, false);
	}

	protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, bool webForward = false)
	{
		if (this.DebugOut >= 3)
		{
			base.get_Listener().DebugReturn(3, "OpSetPropertiesOfActor()");
		}
		if (actorNr <= 0 || actorProperties == null)
		{
			if (this.DebugOut >= 3)
			{
				base.get_Listener().DebugReturn(3, "OpSetPropertiesOfActor not sent. ActorNr must be > 0 and actorProperties != null.");
			}
			return false;
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(251, actorProperties);
		dictionary.Add(254, actorNr);
		dictionary.Add(250, true);
		if (expectedProperties != null && expectedProperties.get_Count() != 0)
		{
			dictionary.Add(231, expectedProperties);
		}
		if (webForward)
		{
			dictionary.set_Item(234, true);
		}
		return this.OpCustom(252, dictionary, true, 0, false);
	}

	protected void OpSetPropertyOfRoom(byte propCode, object value)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.set_Item(propCode, value);
		this.OpSetPropertiesOfRoom(hashtable, null, false);
	}

	public bool OpSetCustomPropertiesOfRoom(Hashtable gameProperties, bool broadcast, byte channelId)
	{
		return this.OpSetPropertiesOfRoom(gameProperties.StripToStringKeys(), null, false);
	}

	protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, bool webForward = false)
	{
		if (this.DebugOut >= 3)
		{
			base.get_Listener().DebugReturn(3, "OpSetPropertiesOfRoom()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(251, gameProperties);
		dictionary.Add(250, true);
		if (expectedProperties != null && expectedProperties.get_Count() != 0)
		{
			dictionary.Add(231, expectedProperties);
		}
		if (webForward)
		{
			dictionary.set_Item(234, true);
		}
		return this.OpCustom(252, dictionary, true, 0, false);
	}

	public virtual bool OpAuthenticate(string appId, string appVersion, AuthenticationValues authValues, string regionCode, bool getLobbyStatistics)
	{
		if (this.DebugOut >= 3)
		{
			base.get_Listener().DebugReturn(3, "OpAuthenticate()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (getLobbyStatistics)
		{
			dictionary.set_Item(211, true);
		}
		if (authValues != null && authValues.Token != null)
		{
			dictionary.set_Item(221, authValues.Token);
			return this.OpCustom(230, dictionary, true, 0, false);
		}
		dictionary.set_Item(220, appVersion);
		dictionary.set_Item(224, appId);
		if (!string.IsNullOrEmpty(regionCode))
		{
			dictionary.set_Item(210, regionCode);
		}
		if (authValues != null)
		{
			if (!string.IsNullOrEmpty(authValues.UserId))
			{
				dictionary.set_Item(225, authValues.UserId);
			}
			if (authValues.AuthType != CustomAuthenticationType.None)
			{
				if (!this.IsProtocolSecure && !base.get_IsEncryptionAvailable())
				{
					base.get_Listener().DebugReturn(1, "OpAuthenticate() failed. When you want Custom Authentication encryption is mandatory.");
					return false;
				}
				dictionary.set_Item(217, (byte)authValues.AuthType);
				if (!string.IsNullOrEmpty(authValues.Token))
				{
					dictionary.set_Item(221, authValues.Token);
				}
				else
				{
					if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
					{
						dictionary.set_Item(216, authValues.AuthGetParameters);
					}
					if (authValues.AuthPostData != null)
					{
						dictionary.set_Item(214, authValues.AuthPostData);
					}
				}
			}
		}
		bool flag = this.OpCustom(230, dictionary, true, 0, base.get_IsEncryptionAvailable());
		if (!flag)
		{
			base.get_Listener().DebugReturn(1, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected.");
		}
		return flag;
	}

	public virtual bool OpAuthenticateOnce(string appId, string appVersion, AuthenticationValues authValues, string regionCode, EncryptionMode encryptionMode, ConnectionProtocol expectedProtocol)
	{
		if (this.DebugOut >= 3)
		{
			base.get_Listener().DebugReturn(3, "OpAuthenticate()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (authValues != null && authValues.Token != null)
		{
			dictionary.set_Item(221, authValues.Token);
			return this.OpCustom(231, dictionary, true, 0, false);
		}
		if (encryptionMode == EncryptionMode.DatagramEncryption && expectedProtocol != null)
		{
			Debug.LogWarning("Expected protocol set to UDP, due to encryption mode DatagramEncryption. Changing protocol in PhotonServerSettings from: " + PhotonNetwork.PhotonServerSettings.Protocol);
			PhotonNetwork.PhotonServerSettings.Protocol = 0;
			expectedProtocol = 0;
		}
		dictionary.set_Item(195, expectedProtocol);
		dictionary.set_Item(193, (byte)encryptionMode);
		dictionary.set_Item(220, appVersion);
		dictionary.set_Item(224, appId);
		if (!string.IsNullOrEmpty(regionCode))
		{
			dictionary.set_Item(210, regionCode);
		}
		if (authValues != null)
		{
			if (!string.IsNullOrEmpty(authValues.UserId))
			{
				dictionary.set_Item(225, authValues.UserId);
			}
			if (authValues.AuthType != CustomAuthenticationType.None)
			{
				dictionary.set_Item(217, (byte)authValues.AuthType);
				if (!string.IsNullOrEmpty(authValues.Token))
				{
					dictionary.set_Item(221, authValues.Token);
				}
				else
				{
					if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
					{
						dictionary.set_Item(216, authValues.AuthGetParameters);
					}
					if (authValues.AuthPostData != null)
					{
						dictionary.set_Item(214, authValues.AuthPostData);
					}
				}
			}
		}
		return this.OpCustom(231, dictionary, true, 0, base.get_IsEncryptionAvailable());
	}

	public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
	{
		if (this.DebugOut >= 5)
		{
			base.get_Listener().DebugReturn(5, "OpChangeGroups()");
		}
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		if (groupsToRemove != null)
		{
			dictionary.set_Item(239, groupsToRemove);
		}
		if (groupsToAdd != null)
		{
			dictionary.set_Item(238, groupsToAdd);
		}
		return this.OpCustom(248, dictionary, true, 0);
	}

	public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
	{
		this.opParameters.Clear();
		this.opParameters.set_Item(244, eventCode);
		if (customEventContent != null)
		{
			this.opParameters.set_Item(245, customEventContent);
		}
		if (raiseEventOptions == null)
		{
			raiseEventOptions = RaiseEventOptions.Default;
		}
		else
		{
			if (raiseEventOptions.CachingOption != EventCaching.DoNotCache)
			{
				this.opParameters.set_Item(247, (byte)raiseEventOptions.CachingOption);
			}
			if (raiseEventOptions.Receivers != ReceiverGroup.Others)
			{
				this.opParameters.set_Item(246, (byte)raiseEventOptions.Receivers);
			}
			if (raiseEventOptions.InterestGroup != 0)
			{
				this.opParameters.set_Item(240, raiseEventOptions.InterestGroup);
			}
			if (raiseEventOptions.TargetActors != null)
			{
				this.opParameters.set_Item(252, raiseEventOptions.TargetActors);
			}
			if (raiseEventOptions.ForwardToWebhook)
			{
				this.opParameters.set_Item(234, true);
			}
		}
		return this.OpCustom(253, this.opParameters, sendReliable, raiseEventOptions.SequenceChannel, raiseEventOptions.Encrypt);
	}

	public virtual bool OpSettings(bool receiveLobbyStats)
	{
		if (this.DebugOut >= 5)
		{
			base.get_Listener().DebugReturn(5, "OpSettings()");
		}
		this.opParameters.Clear();
		if (receiveLobbyStats)
		{
			this.opParameters.set_Item(0, receiveLobbyStats);
		}
		return this.opParameters.get_Count() == 0 || this.OpCustom(218, this.opParameters, true);
	}
}
