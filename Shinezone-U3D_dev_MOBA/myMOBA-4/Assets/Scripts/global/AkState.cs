using System;
using UnityEngine;

[AddComponentMenu("Wwise/AkState")]
public class AkState : AkUnityEventHandler
{
	public int groupID;

	public int valueID;

	public override void HandleEvent(GameObject in_gameObject)
	{
		AkSoundEngine.SetState((uint)this.groupID, (uint)this.valueID);
	}
}
