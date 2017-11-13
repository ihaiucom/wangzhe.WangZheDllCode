using Photon;
using System;
using UnityEngine;

public class ClickAndDrag : Photon.MonoBehaviour
{
	private Vector3 camOnPress;

	private bool following;

	private float factor = -0.1f;

	private void Update()
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		InputToEvent component = Camera.main.GetComponent<InputToEvent>();
		if (component == null)
		{
			return;
		}
		if (!this.following)
		{
			if (!component.Dragging)
			{
				return;
			}
			this.camOnPress = base.transform.position;
			this.following = true;
		}
		else if (component.Dragging)
		{
			Vector3 to = this.camOnPress - new Vector3(component.DragVector.x, 0f, component.DragVector.y) * this.factor;
			base.transform.position = Vector3.Lerp(base.transform.position, to, Time.deltaTime * 0.5f);
		}
		else
		{
			this.camOnPress = Vector3.zero;
			this.following = false;
		}
	}
}
