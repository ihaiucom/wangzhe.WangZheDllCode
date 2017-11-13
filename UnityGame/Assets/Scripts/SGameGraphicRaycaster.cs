using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SGameGraphicRaycaster : GraphicRaycaster
{
	public enum RaycastMode
	{
		Unity,
		Sgame,
		Sgame_tile
	}

	private struct Coord
	{
		public int x;

		public int y;

		public int numX;

		public int numY;

		public static SGameGraphicRaycaster.Coord Invalid = new SGameGraphicRaycaster.Coord
		{
			x = -1,
			y = -1
		};

		public bool IsValid()
		{
			return this.x >= 0 && this.y >= 0;
		}

		public bool Equals(ref SGameGraphicRaycaster.Coord r)
		{
			return r.x == this.x && r.y == this.y && r.numX == this.numX && r.numY == this.numY;
		}
	}

	private class Item
	{
		public Image m_image;

		public RectTransform m_rectTransform;

		public SGameGraphicRaycaster.Coord m_coord = SGameGraphicRaycaster.Coord.Invalid;

		public static SGameGraphicRaycaster.Item Create(Image image)
		{
			if (image == null)
			{
				return null;
			}
			return new SGameGraphicRaycaster.Item
			{
				m_image = image,
				m_rectTransform = (image.gameObject.transform as RectTransform)
			};
		}

		public void Raycast(List<Graphic> raycastResults, Vector2 pointerPosition, Camera eventCamera)
		{
			if (this.m_image.enabled && this.m_rectTransform.gameObject.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(this.m_rectTransform, pointerPosition, eventCamera))
			{
				raycastResults.Add(this.m_image);
			}
		}
	}

	private class Tile
	{
		public ListView<SGameGraphicRaycaster.Item> items = new ListView<SGameGraphicRaycaster.Item>();
	}

	private const int TileCount = 4;

	public bool ignoreScrollRect = true;

	private int raycast_mask = 4;

	private Canvas canvas_;

	private ListView<SGameGraphicRaycaster.Item> m_allItems = new ListView<SGameGraphicRaycaster.Item>();

	private SGameGraphicRaycaster.Tile[] m_tiles;

	private int m_tileSizeX;

	private int m_tileSizeY;

	private int m_screenWidth;

	private int m_screenHeight;

	private Vector3[] corners = new Vector3[4];

	[HideInInspector]
	[NonSerialized]
	public bool tilesDirty;

	public SGameGraphicRaycaster.RaycastMode raycastMode = SGameGraphicRaycaster.RaycastMode.Sgame;

	[NonSerialized]
	private List<Graphic> m_RaycastResults = new List<Graphic>();

	private Canvas canvas
	{
		get
		{
			if (this.canvas_ != null)
			{
				return this.canvas_;
			}
			this.canvas_ = base.GetComponent<Canvas>();
			return this.canvas_;
		}
	}

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		switch (this.raycastMode)
		{
		case SGameGraphicRaycaster.RaycastMode.Unity:
			base.Raycast(eventData, resultAppendList);
			break;
		case SGameGraphicRaycaster.RaycastMode.Sgame:
			this.Raycast2(eventData, resultAppendList, false);
			break;
		case SGameGraphicRaycaster.RaycastMode.Sgame_tile:
			this.Raycast2(eventData, resultAppendList, true);
			break;
		}
	}

	private void CalcItemCoord(ref SGameGraphicRaycaster.Coord coord, SGameGraphicRaycaster.Item item)
	{
		item.m_rectTransform.GetWorldCorners(this.corners);
		int num = 2147483647;
		int num2 = -2147483648;
		int num3 = 2147483647;
		int num4 = -2147483648;
		Camera worldCamera = this.canvas.worldCamera;
		for (int i = 0; i < this.corners.Length; i++)
		{
			Vector3 vector = CUIUtility.WorldToScreenPoint(worldCamera, this.corners[i]);
			num = Mathf.Min((int)vector.x, num);
			num2 = Mathf.Max((int)vector.x, num2);
			num3 = Mathf.Min((int)vector.y, num3);
			num4 = Mathf.Max((int)vector.y, num4);
		}
		coord.x = Mathf.Clamp(num / this.m_tileSizeX, 0, 3);
		coord.numX = Mathf.Clamp(num2 / this.m_tileSizeX, 0, 3) - coord.x + 1;
		coord.y = Mathf.Clamp(num3 / this.m_tileSizeY, 0, 3);
		coord.numY = Mathf.Clamp(num4 / this.m_tileSizeY, 0, 3) - coord.y + 1;
	}

	private void AddToTileList(SGameGraphicRaycaster.Item item)
	{
		int num = item.m_coord.x + item.m_coord.y * 4;
		for (int i = 0; i < item.m_coord.numX; i++)
		{
			for (int j = 0; j < item.m_coord.numY; j++)
			{
				int num2 = j * 4 + i + num;
				this.m_tiles[num2].items.Add(item);
			}
		}
	}

	private void RemoveFromTileList(SGameGraphicRaycaster.Item item)
	{
		if (item.m_coord.IsValid())
		{
			int num = item.m_coord.x + item.m_coord.y * 4;
			for (int i = 0; i < item.m_coord.numX; i++)
			{
				for (int j = 0; j < item.m_coord.numY; j++)
				{
					int num2 = j * 4 + i + num;
					this.m_tiles[num2].items.Remove(item);
				}
			}
			item.m_coord = SGameGraphicRaycaster.Coord.Invalid;
		}
	}

	public void InitTiles()
	{
		if (this.m_tiles != null)
		{
			return;
		}
		this.m_tiles = new SGameGraphicRaycaster.Tile[16];
		for (int i = 0; i < this.m_tiles.Length; i++)
		{
			this.m_tiles[i] = new SGameGraphicRaycaster.Tile();
		}
		this.m_screenWidth = Screen.width;
		this.m_screenHeight = Screen.height;
		this.m_tileSizeX = this.m_screenWidth / 4;
		this.m_tileSizeY = this.m_screenHeight / 4;
	}

	private void UpdateTiles_Editor()
	{
		if ((this.m_screenWidth == Screen.width && this.m_screenHeight == Screen.height) || this.m_tiles == null || this.raycastMode != SGameGraphicRaycaster.RaycastMode.Sgame_tile)
		{
			return;
		}
		this.m_screenWidth = Screen.width;
		this.m_screenHeight = Screen.height;
		this.m_tileSizeX = this.m_screenWidth / 4;
		this.m_tileSizeY = this.m_screenHeight / 4;
		for (int i = 0; i < this.m_tiles.Length; i++)
		{
			this.m_tiles[i].items.Clear();
		}
		for (int j = 0; j < this.m_allItems.Count; j++)
		{
			SGameGraphicRaycaster.Item item = this.m_allItems[j];
			this.CalcItemCoord(ref item.m_coord, item);
			this.AddToTileList(item);
		}
	}

	public void UpdateTiles()
	{
		if (this.raycastMode != SGameGraphicRaycaster.RaycastMode.Sgame_tile)
		{
			return;
		}
		SGameGraphicRaycaster.Coord invalid = SGameGraphicRaycaster.Coord.Invalid;
		for (int i = 0; i < this.m_allItems.Count; i++)
		{
			SGameGraphicRaycaster.Item item = this.m_allItems[i];
			this.CalcItemCoord(ref invalid, item);
			if (!invalid.Equals(ref item.m_coord))
			{
				this.RemoveFromTileList(item);
				item.m_coord = invalid;
				this.AddToTileList(item);
			}
		}
	}

	private void Update()
	{
		this.UpdateTiles_Editor();
	}

	protected override void Start()
	{
		base.Start();
		this.InitializeAllItems();
	}

	private void InitializeAllItems()
	{
		if (this.raycastMode != SGameGraphicRaycaster.RaycastMode.Sgame && this.raycastMode != SGameGraphicRaycaster.RaycastMode.Sgame_tile)
		{
			return;
		}
		this.m_allItems.Clear();
		Image[] componentsInChildren = base.gameObject.GetComponentsInChildren<Image>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (this.IsGameObjectHandleInput(componentsInChildren[i].gameObject))
			{
				SGameGraphicRaycaster.Item item = SGameGraphicRaycaster.Item.Create(componentsInChildren[i]);
				if (item != null)
				{
					this.m_allItems.Add(item);
				}
			}
		}
		if (this.raycastMode == SGameGraphicRaycaster.RaycastMode.Sgame_tile)
		{
			this.InitTiles();
			for (int j = 0; j < this.m_allItems.Count; j++)
			{
				SGameGraphicRaycaster.Item item2 = this.m_allItems[j];
				this.CalcItemCoord(ref item2.m_coord, item2);
				this.AddToTileList(item2);
			}
		}
	}

	public void RemoveGameObject(GameObject go)
	{
		if (go == null || this.m_allItems == null)
		{
			return;
		}
		Image component = go.GetComponent<Image>();
		if (component != null && this.IsGameObjectHandleInput(go))
		{
			this.RemoveItem(component);
		}
	}

	public void RemoveItem(Image image)
	{
		if (image == null || this.m_allItems == null)
		{
			return;
		}
		for (int i = 0; i < this.m_allItems.Count; i++)
		{
			if (this.m_allItems[i].m_image == image)
			{
				if (this.raycastMode == SGameGraphicRaycaster.RaycastMode.Sgame_tile)
				{
					this.RemoveFromTileList(this.m_allItems[i]);
				}
				this.m_allItems.RemoveAt(i);
				break;
			}
		}
	}

	public void RefreshGameObject(GameObject go)
	{
		if (this.raycastMode != SGameGraphicRaycaster.RaycastMode.Sgame && this.raycastMode != SGameGraphicRaycaster.RaycastMode.Sgame_tile)
		{
			return;
		}
		if (go == null || this.m_allItems == null)
		{
			return;
		}
		Image[] componentsInChildren = go.GetComponentsInChildren<Image>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (this.IsGameObjectHandleInput(componentsInChildren[i].gameObject))
			{
				this.RefreshItem(componentsInChildren[i]);
			}
		}
	}

	public void RefreshItem(Image image)
	{
		if (image == null || this.m_allItems == null)
		{
			return;
		}
		SGameGraphicRaycaster.Item item = null;
		for (int i = 0; i < this.m_allItems.Count; i++)
		{
			if (this.m_allItems[i].m_image == image)
			{
				item = this.m_allItems[i];
				break;
			}
		}
		if (item != null)
		{
			if (this.raycastMode == SGameGraphicRaycaster.RaycastMode.Sgame_tile)
			{
				this.RemoveFromTileList(item);
			}
		}
		else
		{
			item = SGameGraphicRaycaster.Item.Create(image);
			if (item == null)
			{
				return;
			}
			this.m_allItems.Add(item);
		}
		if (this.raycastMode == SGameGraphicRaycaster.RaycastMode.Sgame_tile)
		{
			this.CalcItemCoord(ref item.m_coord, item);
			this.AddToTileList(item);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.m_allItems.Clear();
		this.m_RaycastResults.Clear();
		if (this.m_tiles != null)
		{
			for (int i = 0; i < this.m_tiles.Length; i++)
			{
				this.m_tiles[i].items.Clear();
			}
			this.m_tiles = null;
		}
	}

	private void Raycast2(PointerEventData eventData, List<RaycastResult> resultAppendList, bool useTiles)
	{
		if (this.canvas == null)
		{
			return;
		}
		Vector2 vector;
		if (this.get_eventCamera() == null)
		{
			vector = new Vector2(eventData.get_position().x / (float)Screen.width, eventData.get_position().y / (float)Screen.height);
		}
		else
		{
			vector = this.get_eventCamera().ScreenToViewportPoint(eventData.get_position());
		}
		if (vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f)
		{
			return;
		}
		float hitDistance = 3.40282347E+38f;
		Ray ray = default(Ray);
		if (this.get_eventCamera() != null)
		{
			ray = this.get_eventCamera().ScreenPointToRay(eventData.get_position());
		}
		this.m_RaycastResults.Clear();
		Vector2 position = eventData.get_position();
		ListView<SGameGraphicRaycaster.Item> listView;
		if (useTiles && this.m_tiles != null)
		{
			int num = Mathf.Clamp((int)position.x / this.m_tileSizeX, 0, 3);
			int num2 = Mathf.Clamp((int)position.y / this.m_tileSizeY, 0, 3);
			int num3 = num2 * 4 + num;
			listView = this.m_tiles[num3].items;
		}
		else
		{
			listView = this.m_allItems;
		}
		for (int i = 0; i < listView.Count; i++)
		{
			listView[i].Raycast(this.m_RaycastResults, position, this.get_eventCamera());
		}
		this.m_RaycastResults.Sort((Graphic g1, Graphic g2) => g2.get_depth().CompareTo(g1.get_depth()));
		this.AppendResultList(ref ray, hitDistance, resultAppendList, this.m_RaycastResults);
	}

	private bool IsGameObjectHandleInput(GameObject go)
	{
		return go.GetComponent<CUIEventScript>() != null || go.GetComponent<CUIMiniEventScript>() != null || go.GetComponent<CUIMiniEventWithDragScript>() != null || go.GetComponent<CUIToggleEventScript>() != null || go.GetComponent<CUIJoystickScript>() != null || (!this.ignoreScrollRect && go.GetComponent<ScrollRect>() != null);
	}

	private void AppendResultList(ref Ray ray, float hitDistance, List<RaycastResult> resultAppendList, List<Graphic> raycastResults)
	{
		for (int i = 0; i < raycastResults.get_Count(); i++)
		{
			GameObject gameObject = raycastResults.get_Item(i).gameObject;
			bool flag = true;
			if (base.get_ignoreReversedGraphics())
			{
				if (this.get_eventCamera() == null)
				{
					Vector3 rhs = gameObject.transform.rotation * Vector3.forward;
					flag = (Vector3.Dot(Vector3.forward, rhs) > 0f);
				}
				else
				{
					Vector3 lhs = this.get_eventCamera().transform.rotation * Vector3.forward;
					Vector3 rhs2 = gameObject.transform.rotation * Vector3.forward;
					flag = (Vector3.Dot(lhs, rhs2) > 0f);
				}
			}
			if (flag)
			{
				float num;
				if (this.get_eventCamera() == null || this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					num = 0f;
				}
				else
				{
					num = Vector3.Dot(gameObject.transform.forward, gameObject.transform.position - ray.origin) / Vector3.Dot(gameObject.transform.forward, ray.direction);
					if (num < 0f)
					{
						goto IL_1A7;
					}
				}
				if (num < hitDistance)
				{
					RaycastResult raycastResult = default(RaycastResult);
					raycastResult.set_gameObject(gameObject);
					raycastResult.module = this;
					raycastResult.distance = num;
					raycastResult.index = (float)resultAppendList.get_Count();
					raycastResult.depth = raycastResults.get_Item(i).get_depth();
					raycastResult.sortingLayer = this.canvas.sortingLayerID;
					raycastResult.sortingOrder = this.canvas.sortingOrder;
					RaycastResult raycastResult2 = raycastResult;
					resultAppendList.Add(raycastResult2);
				}
			}
			IL_1A7:;
		}
	}
}
