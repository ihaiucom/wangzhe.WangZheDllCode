using System;

public class AkTriggerMouseDown : AkTriggerBase
{
	private void OnMouseDown()
	{
		if (this.triggerDelegate != null)
		{
			this.triggerDelegate(null);
		}
	}
}
