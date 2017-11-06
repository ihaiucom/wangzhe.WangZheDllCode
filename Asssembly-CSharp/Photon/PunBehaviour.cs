using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;

namespace Photon
{
	public class PunBehaviour : MonoBehaviour, IPunCallbacks
	{
		public virtual void OnConnectedToPhoton()
		{
		}

		public virtual void OnLeftRoom()
		{
		}

		public virtual void OnMasterClientSwitched(PhotonPlayer newMasterClient)
		{
		}

		public virtual void OnPhotonCreateRoomFailed(object[] codeAndMsg)
		{
		}

		public virtual void OnPhotonJoinRoomFailed(object[] codeAndMsg)
		{
		}

		public virtual void OnCreatedRoom()
		{
		}

		public virtual void OnJoinedLobby()
		{
		}

		public virtual void OnLeftLobby()
		{
		}

		public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
		{
		}

		public virtual void OnDisconnectedFromPhoton()
		{
		}

		public virtual void OnConnectionFail(DisconnectCause cause)
		{
		}

		public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
		{
		}

		public virtual void OnReceivedRoomListUpdate()
		{
		}

		public virtual void OnJoinedRoom()
		{
		}

		public virtual void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
		{
		}

		public virtual void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
		{
		}

		public virtual void OnPhotonRandomJoinFailed(object[] codeAndMsg)
		{
		}

		public virtual void OnConnectedToMaster()
		{
		}

		public virtual void OnPhotonMaxCccuReached()
		{
		}

		public virtual void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
		{
		}

		public virtual void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
		{
		}

		public virtual void OnUpdatedFriendList()
		{
		}

		public virtual void OnCustomAuthenticationFailed(string debugMessage)
		{
		}

		public virtual void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
		}

		public virtual void OnWebRpcResponse(OperationResponse response)
		{
		}

		public virtual void OnOwnershipRequest(object[] viewAndPlayer)
		{
		}

		public virtual void OnLobbyStatisticsUpdate()
		{
		}

		public virtual void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer)
		{
		}

		public virtual void OnOwnershipTransfered(object[] viewAndPlayers)
		{
		}
	}
}
