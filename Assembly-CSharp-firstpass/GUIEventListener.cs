using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class GUIEventListener : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
	public delegate void VoidDelegate(GameObject go);

	public event GUIEventListener.VoidDelegate onClick
	{
		[MethodImpl(32)]
		add
		{
			this.onClick = (GUIEventListener.VoidDelegate)Delegate.Combine(this.onClick, value);
		}
		[MethodImpl(32)]
		remove
		{
			this.onClick = (GUIEventListener.VoidDelegate)Delegate.Remove(this.onClick, value);
		}
	}

	public event GUIEventListener.VoidDelegate onDown
	{
		[MethodImpl(32)]
		add
		{
			this.onDown = (GUIEventListener.VoidDelegate)Delegate.Combine(this.onDown, value);
		}
		[MethodImpl(32)]
		remove
		{
			this.onDown = (GUIEventListener.VoidDelegate)Delegate.Remove(this.onDown, value);
		}
	}

	public event GUIEventListener.VoidDelegate onUp
	{
		[MethodImpl(32)]
		add
		{
			this.onUp = (GUIEventListener.VoidDelegate)Delegate.Combine(this.onUp, value);
		}
		[MethodImpl(32)]
		remove
		{
			this.onUp = (GUIEventListener.VoidDelegate)Delegate.Remove(this.onUp, value);
		}
	}

	public event GUIEventListener.VoidDelegate onEnter
	{
		[MethodImpl(32)]
		add
		{
			this.onEnter = (GUIEventListener.VoidDelegate)Delegate.Combine(this.onEnter, value);
		}
		[MethodImpl(32)]
		remove
		{
			this.onEnter = (GUIEventListener.VoidDelegate)Delegate.Remove(this.onEnter, value);
		}
	}

	public event GUIEventListener.VoidDelegate onExit
	{
		[MethodImpl(32)]
		add
		{
			this.onExit = (GUIEventListener.VoidDelegate)Delegate.Combine(this.onExit, value);
		}
		[MethodImpl(32)]
		remove
		{
			this.onExit = (GUIEventListener.VoidDelegate)Delegate.Remove(this.onExit, value);
		}
	}

	public event GUIEventListener.VoidDelegate onSelect
	{
		[MethodImpl(32)]
		add
		{
			this.onSelect = (GUIEventListener.VoidDelegate)Delegate.Combine(this.onSelect, value);
		}
		[MethodImpl(32)]
		remove
		{
			this.onSelect = (GUIEventListener.VoidDelegate)Delegate.Remove(this.onSelect, value);
		}
	}

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
