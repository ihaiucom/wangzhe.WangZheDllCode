using System;
using UnityEngine;

public class PickupTriggerForward : MonoBehaviour
{
	public void OnTriggerEnter(Collider other)
	{
		PickupItem component = base.transform.parent.GetComponent<PickupItem>();
		if (component != null)
		{
			component.OnTriggerEnter(other);
		}
	}
}
