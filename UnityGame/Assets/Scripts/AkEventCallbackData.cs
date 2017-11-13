using System;
using System.Collections.Generic;
using UnityEngine;

public class AkEventCallbackData : ScriptableObject
{
	public List<int> callbackFlags = new List<int>();

	public List<GameObject> callbackGameObj = new List<GameObject>();

	public List<string> callbackFunc = new List<string>();

	public int uFlags;
}
