using System;
using UnityEngine;

public class TouchCameraMover : MonoBehaviour
{
	private int ScrollTouchID = -1;

	private void Update()
	{
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			if (touch.phase == TouchPhase.Began && this.ScrollTouchID == -1)
			{
				this.ScrollTouchID = touch.fingerId;
			}
			if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				this.ScrollTouchID = -1;
			}
			if (touch.phase == TouchPhase.Moved && touch.fingerId == this.ScrollTouchID)
			{
				Vector3 position = Camera.main.transform.position;
				Camera.main.transform.position = new Vector3(position.x + touch.deltaPosition.x * 0.2f, position.y, position.z + touch.deltaPosition.y * 0.2f);
			}
		}
	}
}
