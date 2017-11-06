using System;

public interface IPunTurnManagerCallbacks
{
	void OnTurnBegins(int turn);

	void OnTurnCompleted(int turn);

	void OnPlayerMove(PhotonPlayer player, int turn, object move);

	void OnPlayerFinished(PhotonPlayer player, int turn, object move);

	void OnTurnTimeEnds(int turn);
}
