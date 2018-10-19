using System;

public class AkTriggerEnable : AkTriggerBase
{
	private void OnEnable()
	{
		if (this.triggerDelegate != null)
		{
			this.triggerDelegate(null);
		}
	}
}
