using ExitGames.Client.Photon;
using System;

public static class ScoreExtensions
{
	public static void SetScore(this PhotonPlayer player, int newScore)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.set_Item("score", newScore);
		player.SetCustomProperties(hashtable, null, false);
	}

	public static void AddScore(this PhotonPlayer player, int scoreToAddToCurrent)
	{
		int num = player.GetScore();
		num += scoreToAddToCurrent;
		Hashtable hashtable = new Hashtable();
		hashtable.set_Item("score", num);
		player.SetCustomProperties(hashtable, null, false);
	}

	public static int GetScore(this PhotonPlayer player)
	{
		object obj;
		if (player.CustomProperties.TryGetValue("score", ref obj))
		{
			return (int)obj;
		}
		return 0;
	}
}
