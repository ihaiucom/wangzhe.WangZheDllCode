using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Wwise/AkAmbient"), RequireComponent(typeof(AkGameObj))]
public class AkAmbient : AkEvent
{
	public MultiPositionTypeLabel multiPositionTypeLabel;

	public List<Vector3> multiPositionArray = new List<Vector3>();

	public static DictionaryView<int, AkMultiPosEvent> multiPosEventTree = new DictionaryView<int, AkMultiPosEvent>();

	public AkAmbient ParentAkAmbience
	{
		get;
		set;
	}

	private void OnEnable()
	{
		if (this.multiPositionTypeLabel == MultiPositionTypeLabel.Simple_Mode)
		{
			AkGameObj[] components = base.gameObject.GetComponents<AkGameObj>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = true;
			}
		}
		else if (this.multiPositionTypeLabel == MultiPositionTypeLabel.Large_Mode)
		{
			AkGameObj[] components2 = base.gameObject.GetComponents<AkGameObj>();
			for (int j = 0; j < components2.Length; j++)
			{
				components2[j].enabled = false;
			}
			AkPositionArray akPositionArray = this.BuildAkPositionArray();
			AkSoundEngine.SetMultiplePositions(base.gameObject, akPositionArray, (ushort)akPositionArray.Count, MultiPositionType.MultiPositionType_MultiSources);
		}
		else if (this.multiPositionTypeLabel == MultiPositionTypeLabel.MultiPosition_Mode)
		{
			AkGameObj[] components3 = base.gameObject.GetComponents<AkGameObj>();
			for (int k = 0; k < components3.Length; k++)
			{
				components3[k].enabled = false;
			}
			AkMultiPosEvent akMultiPosEvent;
			if (AkAmbient.multiPosEventTree.TryGetValue(this.eventID, out akMultiPosEvent))
			{
				if (!akMultiPosEvent.list.Contains(this))
				{
					akMultiPosEvent.list.Add(this);
				}
			}
			else
			{
				akMultiPosEvent = new AkMultiPosEvent();
				akMultiPosEvent.list.Add(this);
				AkAmbient.multiPosEventTree.Add(this.eventID, akMultiPosEvent);
			}
			AkPositionArray akPositionArray2 = this.BuildMultiDirectionArray(ref akMultiPosEvent);
			AkSoundEngine.SetMultiplePositions(akMultiPosEvent.list[0].gameObject, akPositionArray2, (ushort)akPositionArray2.Count, MultiPositionType.MultiPositionType_MultiSources);
		}
	}

	private void OnDisable()
	{
		if (this.multiPositionTypeLabel == MultiPositionTypeLabel.MultiPosition_Mode)
		{
			AkMultiPosEvent akMultiPosEvent = AkAmbient.multiPosEventTree[this.eventID];
			if (akMultiPosEvent.list.Count == 1)
			{
				AkAmbient.multiPosEventTree.Remove(this.eventID);
				return;
			}
			akMultiPosEvent.list.Remove(this);
			AkPositionArray akPositionArray = this.BuildMultiDirectionArray(ref akMultiPosEvent);
			AkSoundEngine.SetMultiplePositions(akMultiPosEvent.list[0].gameObject, akPositionArray, (ushort)akPositionArray.Count, MultiPositionType.MultiPositionType_MultiSources);
		}
	}

	public override void HandleEvent(GameObject in_gameObject)
	{
		if (this.multiPositionTypeLabel != MultiPositionTypeLabel.MultiPosition_Mode)
		{
			base.HandleEvent(in_gameObject);
		}
		else
		{
			AkMultiPosEvent akMultiPosEvent = AkAmbient.multiPosEventTree[this.eventID];
			if (akMultiPosEvent.eventIsPlaying)
			{
				return;
			}
			akMultiPosEvent.eventIsPlaying = true;
			this.soundEmitterObject = akMultiPosEvent.list[0].gameObject;
			if (this.enableActionOnEvent)
			{
				AkSoundEngine.ExecuteActionOnEvent((uint)this.eventID, this.actionOnEventType, akMultiPosEvent.list[0].gameObject, (int)this.transitionDuration * 1000, this.curveInterpolation);
			}
			else
			{
				AkSoundEngine.PostEvent((uint)this.eventID, akMultiPosEvent.list[0].gameObject, 1u, new AkCallbackManager.EventCallback(akMultiPosEvent.FinishedPlaying), null, 0u, null, 0u);
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (AkSoundEngine.IsInitialized())
		{
			AkSoundEngine.UnregisterGameObj(base.gameObject);
		}
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.DrawIcon(base.transform.position, "WwiseAudioSpeaker.png", false);
	}

	public AkPositionArray BuildMultiDirectionArray(ref AkMultiPosEvent eventPosList)
	{
		AkPositionArray akPositionArray = new AkPositionArray((uint)eventPosList.list.Count);
		for (int i = 0; i < eventPosList.list.Count; i++)
		{
			akPositionArray.Add(eventPosList.list[i].transform.position, eventPosList.list[i].transform.forward);
		}
		return akPositionArray;
	}

	private AkPositionArray BuildAkPositionArray()
	{
		AkPositionArray akPositionArray = new AkPositionArray((uint)this.multiPositionArray.Count);
		for (int i = 0; i < this.multiPositionArray.Count; i++)
		{
			akPositionArray.Add(base.transform.position + this.multiPositionArray[i], base.transform.forward);
		}
		return akPositionArray;
	}
}
