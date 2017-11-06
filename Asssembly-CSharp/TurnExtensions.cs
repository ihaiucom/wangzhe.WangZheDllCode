using ExitGames.Client.Photon;
using System;

public static class TurnExtensions
{
	public static readonly string TurnPropKey = "Turn";

	public static readonly string TurnStartPropKey = "TStart";

	public static readonly string FinishedTurnPropKey = "FToA";

	public static void SetTurn(this Room room, int turn, bool setStartTime = false)
	{
		if (room == null || room.CustomProperties == null)
		{
			return;
		}
		Hashtable hashtable = new Hashtable();
		hashtable.set_Item(TurnExtensions.TurnPropKey, turn);
		if (setStartTime)
		{
			hashtable.set_Item(TurnExtensions.TurnStartPropKey, PhotonNetwork.ServerTimestamp);
		}
		room.SetCustomProperties(hashtable, null, false);
	}

	public static int GetTurn(this RoomInfo room)
	{
		if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnExtensions.TurnPropKey))
		{
			return 0;
		}
		return (int)room.CustomProperties.get_Item(TurnExtensions.TurnPropKey);
	}

	public static int GetTurnStart(this RoomInfo room)
	{
		if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnExtensions.TurnStartPropKey))
		{
			return 0;
		}
		return (int)room.CustomProperties.get_Item(TurnExtensions.TurnStartPropKey);
	}

	public static int GetFinishedTurn(this PhotonPlayer player)
	{
		Room room = PhotonNetwork.room;
		if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnExtensions.TurnPropKey))
		{
			return 0;
		}
		string text = TurnExtensions.FinishedTurnPropKey + player.ID;
		return (int)room.CustomProperties.get_Item(text);
	}

	public static void SetFinishedTurn(this PhotonPlayer player, int turn)
	{
		Room room = PhotonNetwork.room;
		if (room == null || room.CustomProperties == null)
		{
			return;
		}
		string text = TurnExtensions.FinishedTurnPropKey + player.ID;
		Hashtable hashtable = new Hashtable();
		hashtable.set_Item(text, turn);
		room.SetCustomProperties(hashtable, null, false);
	}
}
