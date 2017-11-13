using System;
using UnityEngine;

public struct AkEventCallbackMsg
{
	public AkCallbackType type;

	public GameObject sender;

	public object info;
}
