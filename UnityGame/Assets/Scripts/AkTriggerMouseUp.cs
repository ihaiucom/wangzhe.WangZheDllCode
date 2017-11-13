using System;

public class AkTriggerMouseUp : AkTriggerBase
{
	private void OnMouseUp()
	{
		if (this.triggerDelegate != null)
		{
			this.triggerDelegate(null);
		}
	}
}
