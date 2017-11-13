using ExitGames.UtilityScripts;
using Photon;
using System;
using UnityEngine;

public class ColorPerPlayerApply : PunBehaviour
{
	private static ColorPerPlayer colorPickerCache;

	private Renderer rendererComponent;

	private bool isInitialized;

	private void OnEnable()
	{
		if (!this.isInitialized)
		{
			this.Init();
		}
	}

	private void Start()
	{
		if (!this.isInitialized)
		{
			this.Init();
		}
	}

	private void Init()
	{
		if (!this.isInitialized && PlayerRoomIndexing.instance != null)
		{
			PlayerRoomIndexing expr_20 = PlayerRoomIndexing.instance;
			expr_20.OnRoomIndexingChanged = (PlayerRoomIndexing.RoomIndexingChanged)Delegate.Combine(expr_20.OnRoomIndexingChanged, new PlayerRoomIndexing.RoomIndexingChanged(this.ApplyColor));
			this.isInitialized = true;
		}
	}

	private void OnDisable()
	{
		this.isInitialized = false;
		if (PlayerRoomIndexing.instance != null)
		{
			PlayerRoomIndexing expr_1C = PlayerRoomIndexing.instance;
			expr_1C.OnRoomIndexingChanged = (PlayerRoomIndexing.RoomIndexingChanged)Delegate.Remove(expr_1C.OnRoomIndexingChanged, new PlayerRoomIndexing.RoomIndexingChanged(this.ApplyColor));
		}
	}

	public void Awake()
	{
		if (ColorPerPlayerApply.colorPickerCache == null)
		{
			ColorPerPlayerApply.colorPickerCache = Object.FindObjectOfType<ColorPerPlayer>();
		}
		if (ColorPerPlayerApply.colorPickerCache == null)
		{
			base.enabled = false;
		}
		if (base.photonView.isSceneView)
		{
			base.enabled = false;
		}
		this.rendererComponent = base.GetComponent<Renderer>();
	}

	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		this.ApplyColor();
	}

	public void ApplyColor()
	{
		if (base.photonView.owner == null)
		{
			return;
		}
		int roomIndex = base.photonView.owner.GetRoomIndex();
		if (roomIndex >= 0 && roomIndex <= ColorPerPlayerApply.colorPickerCache.Colors.Length)
		{
			this.rendererComponent.material.color = ColorPerPlayerApply.colorPickerCache.Colors[roomIndex];
		}
	}
}
