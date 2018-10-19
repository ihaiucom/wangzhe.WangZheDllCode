using System;
using UnityEngine;

public class AkTriggerEnter : AkTriggerBase
{
	private void OnTriggerEnter(Collider in_other)
	{
		if (this.triggerDelegate != null)
		{
			this.triggerDelegate(in_other.gameObject);
		}
	}
}
