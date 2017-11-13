using ExitGames.UtilityScripts;
using Photon;
using System;
using UnityEngine;

public class ColorPerPlayer : PunBehaviour
{
	public const string ColorProp = "pc";

	public Color[] Colors = new Color[]
	{
		Color.red,
		Color.blue,
		Color.yellow,
		Color.green
	};

	public bool ShowColorLabel;

	public Rect ColorLabelArea = new Rect(0f, 50f, 100f, 200f);

	public Texture2D img;

	public Color MyColor = Color.get_grey();

	private bool isInitialized;

	public bool ColorPicked
	{
		get;
		set;
	}

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
			expr_20.OnRoomIndexingChanged = (PlayerRoomIndexing.RoomIndexingChanged)Delegate.Combine(expr_20.OnRoomIndexingChanged, new PlayerRoomIndexing.RoomIndexingChanged(this.Refresh));
			this.isInitialized = true;
		}
	}

	private void OnDisable()
	{
		PlayerRoomIndexing expr_05 = PlayerRoomIndexing.instance;
		expr_05.OnRoomIndexingChanged = (PlayerRoomIndexing.RoomIndexingChanged)Delegate.Remove(expr_05.OnRoomIndexingChanged, new PlayerRoomIndexing.RoomIndexingChanged(this.Refresh));
	}

	private void Refresh()
	{
		int roomIndex = PhotonNetwork.player.GetRoomIndex();
		if (roomIndex == -1)
		{
			this.Reset();
		}
		else
		{
			this.MyColor = this.Colors[roomIndex];
			this.ColorPicked = true;
		}
	}

	public override void OnJoinedRoom()
	{
		if (!this.isInitialized)
		{
			this.Init();
		}
	}

	public override void OnLeftRoom()
	{
		this.Reset();
	}

	public void Reset()
	{
		this.MyColor = Color.get_grey();
		this.ColorPicked = false;
	}

	private void OnGUI()
	{
		if (!this.ColorPicked || !this.ShowColorLabel)
		{
			return;
		}
		GUILayout.BeginArea(this.ColorLabelArea);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		Color color = GUI.color;
		GUI.color = this.MyColor;
		GUILayout.Label(this.img, new GUILayoutOption[0]);
		GUI.color = color;
		string text = (!PhotonNetwork.isMasterClient) ? "is your color" : "is your color\nyou are the Master Client";
		GUILayout.Label(text, new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
