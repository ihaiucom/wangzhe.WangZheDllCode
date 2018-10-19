using System;
using UnityEngine;

[AddComponentMenu("Wwise/AkSwitch")]
public class AkSwitch : AkUnityEventHandler
{
	public int groupID;

	public int valueID;

	public override void HandleEvent(GameObject in_gameObject)
	{
		AkSoundEngine.SetSwitch((uint)this.groupID, (uint)this.valueID, (!this.useOtherObject || !(in_gameObject != null)) ? base.gameObject : in_gameObject);
	}
}
