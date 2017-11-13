using System;
using UnityEngine;

[Serializable]
public class LevelResAsset : ScriptableObject
{
	public TextAsset levelDom;

	public Texture2D[] lightmapFar;

	public Texture2D[] lightmapNear;
}
