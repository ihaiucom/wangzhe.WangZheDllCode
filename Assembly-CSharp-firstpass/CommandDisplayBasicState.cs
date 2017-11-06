using System;
using UnityEngine;

internal abstract class CommandDisplayBasicState : BaseState
{
	protected ConsoleWindow ParentWindow;

	protected ConsoleViewMobile ParentView;

	public static int SpaceHeight = 3;

	public IConsoleLogger logger
	{
		get
		{
			return this.ParentView.logger;
		}
	}

	public virtual bool canScroll
	{
		get
		{
			return true;
		}
	}

	public CommandDisplayBasicState(ConsoleWindow InParentWindow, ConsoleViewMobile InParentView)
	{
		this.ParentWindow = InParentWindow;
		this.ParentView = InParentView;
	}

	public abstract void OnGUI();

	public bool DrawButton(string InButtonText, string InToolTip = "")
	{
		GUIContent content = new GUIContent(InButtonText, InToolTip);
		Vector2 vector = this.ParentView.CustomButtonStyle.CalcSize(content);
		return GUILayout.Button(content, this.ParentView.CustomButtonStyle, new GUILayoutOption[]
		{
			GUILayout.Width(vector.x + 20f)
		});
	}

	public void DrawLabel(string InLabel)
	{
		GUIContent content = new GUIContent(InLabel);
		Vector2 vector = this.ParentView.CustomLabelStyle.CalcSize(content);
		GUILayout.Label(content, this.ParentView.CustomLabelStyle, new GUILayoutOption[]
		{
			GUILayout.Width(vector.x + 15f)
		});
	}

	public override void OnStateEnter()
	{
		base.OnStateEnter();
		this.OnResetSkipCount();
	}

	public override void OnStateResume()
	{
		base.OnStateResume();
		this.OnResetSkipCount();
	}

	protected virtual void OnResetSkipCount()
	{
	}
}
