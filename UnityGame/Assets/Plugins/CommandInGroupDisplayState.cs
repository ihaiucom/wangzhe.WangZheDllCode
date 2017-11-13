using System;
using System.Collections.Generic;
using UnityEngine;

[CommandDisplay]
internal class CommandInGroupDisplayState : CommandDisplayBasicState
{
	private int Count;

	public CheatCommandGroup currentGroup
	{
		get;
		protected set;
	}

	public CommandInGroupDisplayState(ConsoleWindow InParentWindow, ConsoleViewMobile InParentView) : base(InParentWindow, InParentView)
	{
	}

	public void SetGroup(CheatCommandGroup InGroup)
	{
		this.currentGroup = InGroup;
		this.OnResetSkipCount();
	}

	protected override void OnResetSkipCount()
	{
		if (this.currentGroup != null)
		{
			int num = 0;
			DictionaryView<string, ICheatCommand>.Enumerator enumerator = this.currentGroup.Commands.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, ICheatCommand> current = enumerator.Current;
				if (!current.get_Value().isHiddenInMobile)
				{
					num++;
				}
			}
			this.ParentView.UpdateSkipCount(this.currentGroup.ChildrenGroups.Count + num);
		}
	}

	protected void DrawGroups()
	{
		DictionaryView<string, CheatCommandGroup>.Enumerator enumerator = this.currentGroup.ChildrenGroups.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, CheatCommandGroup> current = enumerator.Current;
			string key = current.get_Key();
			if (this.Count++ >= this.ParentView.skipCount && base.DrawButton(key, string.Empty))
			{
				ConsoleViewMobile parentView = this.ParentView;
				KeyValuePair<string, CheatCommandGroup> current2 = enumerator.Current;
				parentView.SelectGroup(current2.get_Value());
			}
			GUILayout.Space((float)CommandDisplayBasicState.SpaceHeight);
		}
	}

	protected void DrawCommands()
	{
		DictionaryView<string, ICheatCommand>.Enumerator enumerator = this.currentGroup.Commands.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, ICheatCommand> current = enumerator.Current;
			if (!current.get_Value().isHiddenInMobile)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				try
				{
					KeyValuePair<string, ICheatCommand> current2 = enumerator.Current;
					string comment = current2.get_Value().comment;
					if (this.Count++ >= this.ParentView.skipCount)
					{
						string inButtonText = comment;
						KeyValuePair<string, ICheatCommand> current3 = enumerator.Current;
						if (base.DrawButton(inButtonText, current3.get_Value().fullyHelper))
						{
							KeyValuePair<string, ICheatCommand> current4 = enumerator.Current;
							ICheatCommand value = current4.get_Value();
							if (value.argumentsTypes != null && value.argumentsTypes.Length != 0)
							{
								base.logger.Clear();
								ConsoleViewMobile parentView = this.ParentView;
								KeyValuePair<string, ICheatCommand> current5 = enumerator.Current;
								parentView.SelectionCommand(current5.get_Value());
								break;
							}
							base.logger.AddMessage(value.StartProcess(new string[]
							{
								string.Empty
							}));
						}
						GUILayout.Label(GUI.tooltip, this.ParentView.CustomLabelStyle, new GUILayoutOption[0]);
						GUI.tooltip = string.Empty;
					}
					num++;
				}
				finally
				{
					GUILayout.EndHorizontal();
				}
				GUILayout.Space((float)CommandDisplayBasicState.SpaceHeight);
			}
		}
	}

	public override void OnGUI()
	{
		if (this.currentGroup == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(base.logger.message))
		{
			GUI.contentColor = Color.yellow;
			GUILayout.Label(base.logger.message, this.ParentView.CustomLabelStyle, new GUILayoutOption[0]);
		}
		this.Count = 0;
		GUI.contentColor = Color.green;
		try
		{
			this.DrawGroups();
		}
		finally
		{
			GUI.contentColor = Color.white;
		}
		this.DrawCommands();
	}
}
