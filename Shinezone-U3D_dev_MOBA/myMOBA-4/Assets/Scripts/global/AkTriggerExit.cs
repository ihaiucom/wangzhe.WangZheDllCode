using System;
using UnityEngine;

public class AkTriggerExit : AkTriggerBase
{
	private void OnTriggerExit(Collider in_other)
	{
		if (this.triggerDelegate != null)
		{
			this.triggerDelegate(in_other.gameObject);
		}
	}
}
