using System;
using UnityEngine;
using UnityEngine.UI;

public class InBattleMsgShower : MonoBehaviour
{
	public uint cfg_id;

	public Text text;

	public void Set(uint cfg_id, string content)
	{
		this.cfg_id = cfg_id;
		if (this.text != null)
		{
			this.text.text = content;
		}
	}
}
