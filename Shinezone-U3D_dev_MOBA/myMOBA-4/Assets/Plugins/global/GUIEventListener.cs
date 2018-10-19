using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class GUIEventListener : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
	public delegate void VoidDelegate(GameObject go);

	public event GUIEventListener.VoidDelegate onClick;

	public event GUIEventListener.VoidDelegate onDown;

	public event GUIEventListener.VoidDelegate onUp;

	public event GUIEventListener.VoidDelegate onEnter;

	public event GUIEventListener.VoidDelegate onExit;

	public event GUIEventListener.VoidDelegate onSelect;

	public object userData
	{
		get;
		set;
	}

	public static GUIEventListener Get(GameObject go)
	{
		GUIEventListener gUIEventListener = go.GetComponent<GUIEventListener>();
		if (gUIEventListener == null)
		{
			gUIEventListener = go.AddComponent<GUIEventListener>();
		}
		return gUIEventListener;
	}

	public void SetUserData(object data)
	{
		this.userData = data;
	}

	public object GetUserData()
	{
		return this.userData;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.onClick != null)
		{
			this.onClick(base.gameObject);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (this.onDown != null)
		{
			this.onDown(base.gameObject);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (this.onUp != null)
		{
			this.onUp(base.gameObject);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (this.onEnter != null)
		{
			this.onEnter(base.gameObject);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (this.onExit != null)
		{
			this.onExit(base.gameObject);
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (this.onSelect != null)
		{
			this.onSelect(base.gameObject);
		}
	}
}
