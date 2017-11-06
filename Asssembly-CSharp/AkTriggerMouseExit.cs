using System;

public class AkTriggerMouseExit : AkTriggerBase
{
	private void OnMouseExit()
	{
		if (this.triggerDelegate != null)
		{
			this.triggerDelegate(null);
		}
	}
}
