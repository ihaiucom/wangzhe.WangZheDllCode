using System;
using UnityEngine;

internal class ConsoleDragHandler
{
	private bool bDragging;

	private Vector2 DeltaPosition;

	public bool isDragging
	{
		get
		{
			return this.bDragging;
		}
	}

	public Vector2 deltaPosition
	{
		get
		{
			return this.DeltaPosition;
		}
	}

	public float dragDelta
	{
		get
		{
			return this.DeltaPosition.y;
		}
	}

	public void OnUpdate()
	{
		if (Input.touchCount == 1)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.fingerId == 0)
			{
				this.bDragging = (touch.phase == TouchPhase.Moved);
				if (this.bDragging)
				{
					this.DeltaPosition = new Vector2(touch.deltaPosition.x / (float)Screen.width, touch.deltaPosition.y / (float)Screen.height);
				}
			}
		}
	}
}
