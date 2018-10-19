using System;
using UnityEngine;

public class AkTriggerCollisionExit : AkTriggerBase
{
	private void OnCollisionExit(Collision in_other)
	{
		if (this.triggerDelegate != null)
		{
			this.triggerDelegate(in_other.gameObject);
		}
	}
}
