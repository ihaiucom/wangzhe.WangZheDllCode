using ExitGames.Client.Photon;
using System;

public class RoomOptions
{
	private bool isVisibleField = true;

	private bool isOpenField = true;

	public byte MaxPlayers;

	public int PlayerTtl;

	public int EmptyRoomTtl;

	private bool cleanupCacheOnLeaveField = PhotonNetwork.autoCleanUpPlayerObjects;

	public Hashtable CustomRoomProperties;

	public string[] CustomRoomPropertiesForLobby = new string[0];

	public string[] Plugins;

	private bool suppressRoomEventsField;

	private bool publishUserIdField;

	private bool deleteNullPropertiesField;

	public bool IsVisible
	{
		get
		{
			return this.isVisibleField;
		}
		set
		{
			this.isVisibleField = value;
		}
	}

	public bool IsOpen
	{
		get
		{
			return this.isOpenField;
		}
		set
		{
			this.isOpenField = value;
		}
	}

	public bool CleanupCacheOnLeave
	{
		get
		{
			return this.cleanupCacheOnLeaveField;
		}
		set
		{
			this.cleanupCacheOnLeaveField = value;
		}
	}

	public bool SuppressRoomEvents
	{
		get
		{
			return this.suppressRoomEventsField;
		}
	}

	public bool PublishUserId
	{
		get
		{
			return this.publishUserIdField;
		}
		set
		{
			this.publishUserIdField = value;
		}
	}

	public bool DeleteNullProperties
	{
		get
		{
			return this.deleteNullPropertiesField;
		}
		set
		{
			this.deleteNullPropertiesField = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public bool isVisible
	{
		get
		{
			return this.isVisibleField;
		}
		set
		{
			this.isVisibleField = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public bool isOpen
	{
		get
		{
			return this.isOpenField;
		}
		set
		{
			this.isOpenField = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public byte maxPlayers
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

	[Obsolete("Use property with uppercase naming instead.")]
	public bool cleanupCacheOnLeave
	{
		get
		{
			return this.cleanupCacheOnLeaveField;
		}
		set
		{
			this.cleanupCacheOnLeaveField = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public Hashtable customRoomProperties
	{
		get
		{
			return this.CustomRoomProperties;
		}
		set
		{
			this.CustomRoomProperties = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public string[] customRoomPropertiesForLobby
	{
		get
		{
			return this.CustomRoomPropertiesForLobby;
		}
		set
		{
			this.CustomRoomPropertiesForLobby = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public string[] plugins
	{
		get
		{
			return this.Plugins;
		}
		set
		{
			this.Plugins = value;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public bool suppressRoomEvents
	{
		get
		{
			return this.suppressRoomEventsField;
		}
	}

	[Obsolete("Use property with uppercase naming instead.")]
	public bool publishUserId
	{
		get
		{
			return this.publishUserIdField;
		}
		set
		{
			this.publishUserIdField = value;
		}
	}
}
