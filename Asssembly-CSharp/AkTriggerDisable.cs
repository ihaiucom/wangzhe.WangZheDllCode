using System;

public class AkTriggerDisable : AkTriggerBase
{
	private void OnDisable()
	{
		if (this.triggerDelegate != null)
		{
			this.triggerDelegate(null);
		}
	}
}
