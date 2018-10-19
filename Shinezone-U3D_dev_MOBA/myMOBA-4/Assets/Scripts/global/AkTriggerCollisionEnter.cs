using System;
using UnityEngine;

public class AkTriggerCollisionEnter : AkTriggerBase
{
	private void OnCollisionEnter(Collision in_other)
	{
		if (this.triggerDelegate != null)
		{
			this.triggerDelegate(in_other.gameObject);
		}
	}
}
