using Assets.Scripts.GameLogic.DataCenter;
using System;
using System.Collections.Generic;

public class ActorPreloadTab
{
	public ActorMeta theActor;

	public AssetLoadBase modelPrefab;

	public List<AssetLoadBase> ageActions;

	public List<AssetLoadBase> parPrefabs;

	public List<AssetLoadBase> mesPrefabs;

	public List<AssetLoadBase> spritePrefabs;

	public List<AssetLoadBase> soundBanks;

	public List<AssetLoadBase> behaviorXml;

	public int MarkID;

	public float spawnCnt;

	public void AddParticle(string path)
	{
		this.parPrefabs.Add(new AssetLoadBase
		{
			assetPath = path
		});
	}

	public void AddParticle(string path, int count)
	{
		this.parPrefabs.Add(new AssetLoadBase
		{
			assetPath = path,
			nInstantiate = count
		});
	}

	public void AddSprite(string path)
	{
		this.spritePrefabs.Add(new AssetLoadBase
		{
			assetPath = path
		});
	}

	public bool IsExistsSprite(string path)
	{
		return this.spritePrefabs.Exists((AssetLoadBase info) => info.assetPath == path);
	}

	public void AddAction(string path)
	{
		this.ageActions.Add(new AssetLoadBase
		{
			assetPath = path
		});
	}

	public void AddMesh(string path)
	{
		this.mesPrefabs.Add(new AssetLoadBase
		{
			assetPath = path
		});
	}
}
