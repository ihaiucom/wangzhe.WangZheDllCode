using ExitGames.Client.Photon;
using System;
using UnityEngine;

public class Room : RoomInfo
{
	public new string Name
	{
		get
		{
			return this.nameField;
		}
		internal set
		{
			this.nameField = value;
		}
	}

	public new bool IsOpen
	{
		get
		{
			return this.openField;
		}
		set
		{
			if (!this.Equals(PhotonNetwork.room))
			{
				Debug.LogWarning("Can't set open when not in that room.");
			}
			if (value != this.openField && !PhotonNetwork.offlineMode)
			{
				LoadBalancingPeer arg_54_0 = PhotonNetwork.networkingPeer;
				Hashtable hashtable = new Hashtable();
				hashtable.Add(253, value);
				arg_54_0.OpSetPropertiesOfRoom(hashtable, null, false);
			}
			this.openField = value;
		}
	}

	public new bool IsVisible
	{
		get
		{
			return this.visibleField;
		}
		set
		{
			if (!this.Equals(PhotonNetwork.room))
			{
				Debug.LogWarning("Can't set visible when not in that room.");
			}
			if (value != this.visibleField && !PhotonNetwork.offlineMode)
			{
				LoadBalancingPeer arg_54_0 = PhotonNetwork.networkingPeer;
				Hashtable hashtable = new Hashtable();
				hashtable.Add(254, value);
				arg_54_0.OpSetPropertiesOfRoom(hashtable, null, false);
			}
			this.visibleField = value;
		}
	}

	public string[] PropertiesListedInLobby
	{
		get;
		private set;
	}

	public bool AutoCleanUp
	{
		get
		{
			return this.autoCleanUpField;
		}
	}

	public new int MaxPlayers
	{
		get
		{
			return (int)this.maxPlayersField;
		}
		set
		{
			if (!this.Equals(PhotonNetwork.room))
			{
				Debug.LogWarning("Can't set MaxPlayers when not in that room.");
			}
			if (value > 255)
			{
				Debug.LogWarning("Can't set Room.MaxPlayers to: " + value + ". Using max value: 255.");
				value = 255;
			}
			if (value != (int)this.maxPlayersField && !PhotonNetwork.offlineMode)
			{
				LoadBalancingPeer arg_81_0 = PhotonNetwork.networkingPeer;
				Hashtable hashtable = new Hashtable();
				hashtable.Add(255, (byte)value);
				arg_81_0.OpSetPropertiesOfRoom(hashtable, null, false);
			}
			this.maxPlayersField = (byte)value;
		}
	}

	public new int PlayerCount
	{
		get
		{
			if (PhotonNetwork.playerList != null)
			{
				return PhotonNetwork.playerList.Length;
			}
			return 0;
		}
	}

	public string[] ExpectedUsers
	{
		get
		{
			return this.expectedUsersField;
		}
	}

	protected internal int MasterClientId
	{
		get
		{
			return this.masterClientIdField;
		}
		set
		{
			this.masterClientIdField = value;
		}
	}

	[Obsolete("Please use Name (updated case for naming).")]
	public new string name
	{
		get
		{
			return this.Name;
		}
		internal set
		{
			this.Name = value;
		}
	}

	[Obsolete("Please use IsOpen (updated case for naming).")]
	public new bool open
	{
		get
		{
			return this.IsOpen;
		}
		set
		{
			this.IsOpen = value;
		}
	}

	[Obsolete("Please use IsVisible (updated case for naming).")]
	public new bool visible
	{
		get
		{
			return this.IsVisible;
		}
		set
		{
			this.IsVisible = value;
		}
	}

	[Obsolete("Please use PropertiesListedInLobby (updated case for naming).")]
	public string[] propertiesListedInLobby
	{
		get
		{
			return this.PropertiesListedInLobby;
		}
		private set
		{
			this.PropertiesListedInLobby = value;
		}
	}

	[Obsolete("Please use AutoCleanUp (updated case for naming).")]
	public bool autoCleanUp
	{
		get
		{
			return this.AutoCleanUp;
		}
	}

	[Obsolete("Please use MaxPlayers (updated case for naming).")]
	public new int maxPlayers
	{
		get
		{
			return this.MaxPlayers;
		}
		set
		{
			this.MaxPlayers = value;
		}
	}

	[Obsolete("Please use PlayerCount (updated case for naming).")]
	public new int playerCount
	{
		get
		{
			return this.PlayerCount;
		}
	}

	[Obsolete("Please use ExpectedUsers (updated case for naming).")]
	public string[] expectedUsers
	{
		get
		{
			return this.ExpectedUsers;
		}
	}

	[Obsolete("Please use MasterClientId (updated case for naming).")]
	protected internal int masterClientId
	{
		get
		{
			return this.MasterClientId;
		}
		set
		{
			this.MasterClientId = value;
		}
	}

	internal Room(string roomName, RoomOptions options) : base(roomName, null)
	{
		if (options == null)
		{
			options = new RoomOptions();
		}
		this.visibleField = options.IsVisible;
		this.openField = options.IsOpen;
		this.maxPlayersField = options.MaxPlayers;
		this.autoCleanUpField = false;
		base.InternalCacheProperties(options.CustomRoomProperties);
		this.PropertiesListedInLobby = options.CustomRoomPropertiesForLobby;
	}

	public void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedValues = null, bool webForward = false)
	{
		if (propertiesToSet == null)
		{
			return;
		}
		Hashtable hashtable = propertiesToSet.StripToStringKeys();
		Hashtable hashtable2 = expectedValues.StripToStringKeys();
		bool flag = hashtable2 == null || hashtable2.get_Count() == 0;
		if (PhotonNetwork.offlineMode || flag)
		{
			base.CustomProperties.Merge(hashtable);
			base.CustomProperties.StripKeysWithNullValues();
		}
		if (!PhotonNetwork.offlineMode)
		{
			PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable, hashtable2, webForward);
		}
		if (PhotonNetwork.offlineMode || flag)
		{
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, new object[]
			{
				hashtable
			});
		}
	}

	public void SetPropertiesListedInLobby(string[] propsListedInLobby)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.set_Item(250, propsListedInLobby);
		PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable, null, false);
		this.PropertiesListedInLobby = propsListedInLobby;
	}

	public void ClearExpectedUsers()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.set_Item(247, new string[0]);
		Hashtable hashtable2 = new Hashtable();
		hashtable2.set_Item(247, this.ExpectedUsers);
		PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable, hashtable2, false);
	}

	public override string ToString()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", new object[]
		{
			this.nameField,
			(!this.visibleField) ? "hidden" : "visible",
			(!this.openField) ? "closed" : "open",
			this.maxPlayersField,
			this.PlayerCount
		});
	}

	public string ToStringFull()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", new object[]
		{
			this.nameField,
			(!this.visibleField) ? "hidden" : "visible",
			(!this.openField) ? "closed" : "open",
			this.maxPlayersField,
			this.PlayerCount,
			base.CustomProperties.ToStringFull()
		});
	}
}
