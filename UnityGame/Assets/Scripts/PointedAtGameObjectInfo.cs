using System;
using UnityEngine;

[RequireComponent(typeof(InputToEvent))]
public class PointedAtGameObjectInfo : MonoBehaviour
{
	private void OnGUI()
	{
		if (InputToEvent.goPointedAt != null)
		{
			PhotonView photonView = InputToEvent.goPointedAt.GetPhotonView();
			if (photonView != null)
			{
				GUI.Label(new Rect(Input.mousePosition.x + 5f, (float)Screen.height - Input.mousePosition.y - 15f, 300f, 30f), string.Format("ViewID {0} {1}{2}", photonView.viewID, (!photonView.isSceneView) ? string.Empty : "scene ", (!photonView.isMine) ? ("owner: " + photonView.ownerId) : "mine"));
			}
		}
	}
}
