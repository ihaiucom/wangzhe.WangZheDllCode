using System;
using UnityEngine;

public class SceneImporter : MonoBehaviour
{
	private static GameSerializer s_serializer = new GameSerializer();

	public LevelResAsset level;

	public void Import()
	{
		SceneImporter.s_serializer.Load(this.level);
	}
}
