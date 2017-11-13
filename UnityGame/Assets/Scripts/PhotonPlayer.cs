using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PhotonPlayer : IComparable<PhotonPlayer>, IComparable<int>, IEquatable<PhotonPlayer>, IEquatable<int>
{
	private int actorID = -1;

	private string nameField = string.Empty;

	public readonly bool IsLocal;

	public object TagObject;

	public int ID
	{
		get
		{
			return this.actorID;
		}
	}

	public string NickName
	{
		get
		{
			return this.nameField;
		}
		set
		{
			if (!this.IsLocal)
			{
				Debug.LogError("Error: Cannot change the name of a remote player!");
				return;
			}
			if (string.IsNullOrEmpty(value) || value.Equals(this.nameField))
			{
				return;
			}
			this.nameField = value;
			PhotonNetwork.playerName = value;
		}
	}

	public string UserId
	{
		get;
		internal set;
	}

	public bool IsMasterClient
	{
		get
		{
			return PhotonNetwork.networkingPeer.mMasterClientId == this.ID;
		}
	}

	public bool IsInactive
	{
		get;
		set;
	}

	public Hashtable CustomProperties
	{
		get;
		internal set;
	}

	public Hashtable AllProperties
	{
		get
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Merge(this.CustomProperties);
			hashtable.set_Item(255, this.NickName);
			return hashtable;
		}
	}

	[Obsolete("Please use NickName (updated case for naming).")]
	public string name
	{
		get
		{
			return this.NickName;
		}
		set
		{
			this.NickName = value;
		}
	}

	[Obsolete("Please use UserId (updated case for naming).")]
	public string userId
	{
		get
		{
			return this.UserId;
		}
		internal set
		{
			this.UserId = value;
		}
	}

	[Obsolete("Please use IsLocal (updated case for naming).")]
	public bool isLocal
	{
		get
		{
			return this.IsLocal;
		}
	}

	[Obsolete("Please use IsMasterClient (updated case for naming).")]
	public bool isMasterClient
	{
		get
		{
			return this.IsMasterClient;
		}
	}

	[Obsolete("Please use IsInactive (updated case for naming).")]
	public bool isInactive
	{
		get
		{
			return this.IsInactive;
		}
		set
		{
			this.IsInactive = value;
		}
	}

	[Obsolete("Please use CustomProperties (updated case for naming).")]
	public Hashtable customProperties
	{
		get
		{
			return this.CustomProperties;
		}
		internal set
		{
			this.CustomProperties = value;
		}
	}

	[Obsolete("Please use AllProperties (updated case for naming).")]
	public Hashtable allProperties
	{
		get
		{
			return this.AllProperties;
		}
	}

	public PhotonPlayer(bool isLocal, int actorID, string name)
	{
		this.CustomProperties = new Hashtable();
		this.IsLocal = isLocal;
		this.actorID = actorID;
		this.nameField = name;
	}

	protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
	{
		this.CustomProperties = new Hashtable();
		this.IsLocal = isLocal;
		this.actorID = actorID;
		this.InternalCacheProperties(properties);
	}

	public override bool Equals(object p)
	{
		PhotonPlayer photonPlayer = p as PhotonPlayer;
		return photonPlayer != null && this.GetHashCode() == photonPlayer.GetHashCode();
	}

	public override int GetHashCode()
	{
		return this.ID;
	}

	internal void InternalChangeLocalID(int newID)
	{
		if (!this.IsLocal)
		{
			Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
			return;
		}
		this.actorID = newID;
	}

	internal void InternalCacheProperties(Hashtable properties)
	{
		if (properties == null || properties.get_Count() == 0 || this.CustomProperties.Equals(properties))
		{
			return;
		}
		if (properties.ContainsKey(255))
		{
			this.nameField = (string)properties.get_Item(255);
		}
		if (properties.ContainsKey(253))
		{
			this.UserId = (string)properties.get_Item(253);
		}
		if (properties.ContainsKey(254))
		{
			this.IsInactive = (bool)properties.get_Item(254);
		}
		this.CustomProperties.MergeStringKeys(properties);
		this.CustomProperties.StripKeysWithNullValues();
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
		bool flag2 = this.actorID > 0 && !PhotonNetwork.offlineMode;
		if (flag)
		{
			this.CustomProperties.Merge(hashtable);
			this.CustomProperties.StripKeysWithNullValues();
		}
		if (flag2)
		{
			PhotonNetwork.networkingPeer.OpSetPropertiesOfActor(this.actorID, hashtable, hashtable2, webForward);
		}
		if (!flag2 || flag)
		{
			this.InternalCacheProperties(hashtable);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, new object[]
			{
				this,
				hashtable
			});
		}
	}

	public static PhotonPlayer Find(int ID)
	{
		if (PhotonNetwork.networkingPeer != null)
		{
			return PhotonNetwork.networkingPeer.GetPlayerWithId(ID);
		}
		return null;
	}

	public PhotonPlayer Get(int id)
	{
		return PhotonPlayer.Find(id);
	}

	public PhotonPlayer GetNext()
	{
		return this.GetNextFor(this.ID);
	}

	public PhotonPlayer GetNextFor(PhotonPlayer currentPlayer)
	{
		if (currentPlayer == null)
		{
			return null;
		}
		return this.GetNextFor(currentPlayer.ID);
	}

	public PhotonPlayer GetNextFor(int currentPlayerId)
	{
		if (PhotonNetwork.networkingPeer == null || PhotonNetwork.networkingPeer.mActors == null || PhotonNetwork.networkingPeer.mActors.get_Count() < 2)
		{
			return null;
		}
		Dictionary<int, PhotonPlayer> mActors = PhotonNetwork.networkingPeer.mActors;
		int num = 2147483647;
		int num2 = currentPlayerId;
		using (Dictionary<int, PhotonPlayer>.KeyCollection.Enumerator enumerator = mActors.get_Keys().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				int current = enumerator.get_Current();
				if (current < num2)
				{
					num2 = current;
				}
				else if (current > currentPlayerId && current < num)
				{
					num = current;
				}
			}
		}
		return (num == 2147483647) ? mActors.get_Item(num2) : mActors.get_Item(num);
	}

	public int CompareTo(PhotonPlayer other)
	{
		if (other == null)
		{
			return 0;
		}
		return this.GetHashCode().CompareTo(other.GetHashCode());
	}

	public int CompareTo(int other)
	{
		return this.GetHashCode().CompareTo(other);
	}

	public bool Equals(PhotonPlayer other)
	{
		return other != null && this.GetHashCode().Equals(other.GetHashCode());
	}

	public bool Equals(int other)
	{
		return this.GetHashCode().Equals(other);
	}

	public override string ToString()
	{
		if (string.IsNullOrEmpty(this.NickName))
		{
			return string.Format("#{0:00}{1}{2}", this.ID, (!this.IsInactive) ? " " : " (inactive)", (!this.IsMasterClient) ? string.Empty : "(master)");
		}
		return string.Format("'{0}'{1}{2}", this.NickName, (!this.IsInactive) ? " " : " (inactive)", (!this.IsMasterClient) ? string.Empty : "(master)");
	}

	public string ToStringFull()
	{
		return string.Format("#{0:00} '{1}'{2} {3}", new object[]
		{
			this.ID,
			this.NickName,
			(!this.IsInactive) ? string.Empty : " (inactive)",
			this.CustomProperties.ToStringFull()
		});
	}
}
