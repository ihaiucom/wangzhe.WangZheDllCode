using System;
using UnityEngine;

public class ClickDetector : MonoBehaviour
{
	public void Update()
	{
		if (PhotonNetwork.player.ID != PunGameLogic.playerWhoIsIt)
		{
			return;
		}
		if (Input.GetButton("Fire1"))
		{
			GameObject gameObject = this.RaycastObject(Input.mousePosition);
			if (gameObject != null && gameObject != base.gameObject && gameObject.name.Equals("monsterprefab(Clone)", 5))
			{
				PhotonView component = gameObject.transform.root.GetComponent<PhotonView>();
				PunGameLogic.TagPlayer(component.owner.ID);
			}
		}
	}

	private GameObject RaycastObject(Vector2 screenPos)
	{
		Camera main = Camera.main;
		RaycastHit raycastHit;
		if (Physics.Raycast(main.ScreenPointToRay(screenPos), ref raycastHit, 200f))
		{
			return raycastHit.collider.gameObject;
		}
		return null;
	}
}
