using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChannelSelector : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public string Channel;

	public void SetChannel(string channel)
	{
		this.Channel = channel;
		Text componentInChildren = base.GetComponentInChildren<Text>();
		componentInChildren.set_text(this.Channel);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ChatGui chatGui = Object.FindObjectOfType<ChatGui>();
		chatGui.ShowChannel(this.Channel);
	}
}
