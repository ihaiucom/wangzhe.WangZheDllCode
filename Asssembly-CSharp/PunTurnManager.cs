using ExitGames.Client.Photon;
using Photon;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PunTurnManager : PunBehaviour
{
	public const byte TurnManagerEventOffset = 0;

	public const byte EvMove = 1;

	public const byte EvFinalMove = 2;

	public float TurnDuration = 20f;

	public IPunTurnManagerCallbacks TurnManagerListener;

	private readonly HashSet<PhotonPlayer> finishedPlayers = new HashSet<PhotonPlayer>();

	private bool _isOverCallProcessed;

	public int Turn
	{
		get
		{
			return PhotonNetwork.room.GetTurn();
		}
		private set
		{
			this._isOverCallProcessed = false;
			PhotonNetwork.room.SetTurn(value, true);
		}
	}

	public float ElapsedTimeInTurn
	{
		get
		{
			return (float)(PhotonNetwork.ServerTimestamp - PhotonNetwork.room.GetTurnStart()) / 1000f;
		}
	}

	public float RemainingSecondsInTurn
	{
		get
		{
			return Mathf.Max(0f, this.TurnDuration - this.ElapsedTimeInTurn);
		}
	}

	public bool IsCompletedByAll
	{
		get
		{
			return PhotonNetwork.room != null && this.Turn > 0 && this.finishedPlayers.get_Count() == PhotonNetwork.room.PlayerCount;
		}
	}

	public bool IsFinishedByMe
	{
		get
		{
			return this.finishedPlayers.Contains(PhotonNetwork.player);
		}
	}

	public bool IsOver
	{
		get
		{
			return this.RemainingSecondsInTurn <= 0f;
		}
	}

	private void Start()
	{
		PhotonNetwork.OnEventCall = new PhotonNetwork.EventCallback(this.OnEvent);
	}

	private void Update()
	{
		if (this.Turn > 0 && this.IsOver && !this._isOverCallProcessed)
		{
			this._isOverCallProcessed = true;
			this.TurnManagerListener.OnTurnTimeEnds(this.Turn);
		}
	}

	public void BeginTurn()
	{
		this.Turn++;
	}

	public void SendMove(object move, bool finished)
	{
		if (this.IsFinishedByMe)
		{
			Debug.LogWarning("Can't SendMove. Turn is finished by this player.");
			return;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add("turn", this.Turn);
		hashtable.Add("move", move);
		byte eventCode = (!finished) ? 1 : 2;
		PhotonNetwork.RaiseEvent(eventCode, hashtable, true, new RaiseEventOptions
		{
			CachingOption = EventCaching.AddToRoomCache
		});
		if (finished)
		{
			PhotonNetwork.player.SetFinishedTurn(this.Turn);
		}
		this.OnEvent(eventCode, hashtable, PhotonNetwork.player.ID);
	}

	public bool GetPlayerFinishedTurn(PhotonPlayer player)
	{
		return player != null && this.finishedPlayers != null && this.finishedPlayers.Contains(player);
	}

	public void OnEvent(byte eventCode, object content, int senderId)
	{
		PhotonPlayer photonPlayer = PhotonPlayer.Find(senderId);
		if (eventCode != 1)
		{
			if (eventCode == 2)
			{
				Hashtable hashtable = content as Hashtable;
				int num = (int)hashtable.get_Item("turn");
				object move = hashtable.get_Item("move");
				if (num == this.Turn)
				{
					this.finishedPlayers.Add(photonPlayer);
					this.TurnManagerListener.OnPlayerFinished(photonPlayer, num, move);
				}
				if (this.IsCompletedByAll)
				{
					this.TurnManagerListener.OnTurnCompleted(this.Turn);
				}
			}
		}
		else
		{
			Hashtable hashtable2 = content as Hashtable;
			int turn = (int)hashtable2.get_Item("turn");
			object move2 = hashtable2.get_Item("move");
			this.TurnManagerListener.OnPlayerMove(photonPlayer, turn, move2);
		}
	}

	public override void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.ContainsKey("Turn"))
		{
			this._isOverCallProcessed = false;
			this.finishedPlayers.Clear();
			this.TurnManagerListener.OnTurnBegins(this.Turn);
		}
	}
}
