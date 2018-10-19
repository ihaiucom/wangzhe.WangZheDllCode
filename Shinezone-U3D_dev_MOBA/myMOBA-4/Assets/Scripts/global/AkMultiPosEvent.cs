using System;

public class AkMultiPosEvent
{
	public ListView<AkAmbient> list = new ListView<AkAmbient>();

	public bool eventIsPlaying;

	public void FinishedPlaying(object in_cookie, AkCallbackType in_type, object in_info)
	{
		this.eventIsPlaying = false;
	}
}
