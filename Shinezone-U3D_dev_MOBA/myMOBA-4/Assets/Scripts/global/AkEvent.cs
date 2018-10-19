using System;
using UnityEngine;

[AddComponentMenu("Wwise/AkEvent")]
public class AkEvent : AkUnityEventHandler
{
	public int eventID;

	public GameObject soundEmitterObject;

	public bool enableActionOnEvent;

	public AkActionOnEventType actionOnEventType;

	public AkCurveInterpolation curveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear;

	public float transitionDuration;

	public AkEventCallbackData m_callbackData;

	private void Callback(object in_cookie, AkCallbackType in_type, object in_info)
	{
		for (int i = 0; i < this.m_callbackData.callbackFunc.Count; i++)
		{
			if ((in_type & (AkCallbackType)this.m_callbackData.callbackFlags[i]) != (AkCallbackType)0 && this.m_callbackData.callbackGameObj[i] != null)
			{
				AkEventCallbackMsg akEventCallbackMsg = default(AkEventCallbackMsg);
				akEventCallbackMsg.type = in_type;
				akEventCallbackMsg.sender = base.gameObject;
				akEventCallbackMsg.info = in_info;
				this.m_callbackData.callbackGameObj[i].SendMessage(this.m_callbackData.callbackFunc[i], akEventCallbackMsg);
			}
		}
	}

	public override void HandleEvent(GameObject in_gameObject)
	{
		GameObject in_gameObjectID = (!this.useOtherObject || !(in_gameObject != null)) ? base.gameObject : in_gameObject;
		this.soundEmitterObject = in_gameObjectID;
		if (this.enableActionOnEvent)
		{
			AkSoundEngine.ExecuteActionOnEvent((uint)this.eventID, this.actionOnEventType, in_gameObjectID, (int)this.transitionDuration * 1000, this.curveInterpolation);
		}
		else if (this.m_callbackData != null)
		{
			AkSoundEngine.PostEvent((uint)this.eventID, in_gameObjectID, (uint)this.m_callbackData.uFlags, new AkCallbackManager.EventCallback(this.Callback), null, 0u, null, 0u);
		}
		else
		{
			AkSoundEngine.PostEvent((uint)this.eventID, in_gameObjectID);
		}
	}

	public void Stop(int _transitionDuration, AkCurveInterpolation _curveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear)
	{
		AkSoundEngine.ExecuteActionOnEvent((uint)this.eventID, AkActionOnEventType.AkActionOnEventType_Stop, this.soundEmitterObject, _transitionDuration, _curveInterpolation);
	}
}
