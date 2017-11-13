using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConsoleWindow : MonoSingleton<ConsoleWindow>
{
	private static int InternalID = 14408667;

	protected IConsoleView Viewer;

	private Rect WindowRect;

	private bool bShouldVisible;

	public bool bEnableCheatConsole;

	public IConsoleLogger externalLogger;

	protected EventSystem CachedEventSystem;

	public bool isVisible
	{
		get
		{
			return this.bShouldVisible;
		}
		set
		{
			this.bShouldVisible = value;
			if (this.Viewer != null)
			{
				this.Viewer.OnToggleVisible(this.bShouldVisible);
			}
			if (this.CachedEventSystem == null)
			{
				this.CachedEventSystem = EventSystem.get_current();
			}
			if (this.CachedEventSystem != null)
			{
				this.CachedEventSystem.enabled = !value;
			}
		}
	}

	protected override void Init()
	{
		Singleton<CheatCommandRegister>.GetInstance();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.Viewer != null)
		{
			this.Viewer.OnDestory();
		}
	}

	private void Update()
	{
		if (!this.bEnableCheatConsole)
		{
			return;
		}
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.fingerId == 4 && touch.phase == TouchPhase.Began)
			{
				this.ToggleVisible();
				break;
			}
		}
		if (this.Viewer != null)
		{
			this.Viewer.OnUpdate();
		}
	}

	public void ChangeToMobileView()
	{
	}

	public void ChangeToPCView()
	{
	}

	public bool ToggleVisible()
	{
		bool flag = !this.isVisible && this.bEnableCheatConsole;
		this.isVisible = flag;
		return flag;
	}

	protected override void Awake()
	{
		base.Awake();
		this.Viewer = new ConsoleViewMobile(this);
		if (this.Viewer != null)
		{
			this.Viewer.Awake();
		}
	}

	private void OnEnable()
	{
		if (this.Viewer != null)
		{
			this.Viewer.OnEnable();
		}
	}

	private void OnDisable()
	{
		if (this.Viewer != null)
		{
			this.Viewer.OnDisable();
		}
	}

	private void OnGUI()
	{
		if (this.Viewer == null || !this.isVisible)
		{
			return;
		}
		this.WindowRect = this.Viewer.SelectWindowRect();
		GUILayout.Window(ConsoleWindow.InternalID, this.WindowRect, new GUI.WindowFunction(this.OnConsole), "CheatConsole", new GUILayoutOption[0]);
	}

	private void OnConsole(int InWindowID)
	{
		DebugHelper.Assert(this.Viewer != null);
		if (this.Viewer != null)
		{
			this.Viewer.OnConsole(InWindowID);
		}
	}

	public void ClearLog()
	{
		if (this.Viewer != null && this.Viewer.logger != null)
		{
			this.Viewer.logger.Clear();
		}
		if (this.externalLogger != null)
		{
			this.externalLogger.Clear();
		}
	}

	public void AddMessage(string InMessage)
	{
		if (this.Viewer != null && this.Viewer.logger != null)
		{
			this.Viewer.logger.AddMessage(InMessage);
		}
		if (this.externalLogger != null)
		{
			this.externalLogger.AddMessage(InMessage);
		}
	}
}
