using System;
using System.Collections.Generic;
using UnityEngine;

[CommandDisplay]
internal class CommandGroupDisplayState : CommandDisplayBasicState
{
	public override bool canScroll
	{
		get
		{
			return true;
		}
	}

	public CommandGroupDisplayState(ConsoleWindow InParentWindow, ConsoleViewMobile InParentView) : base(InParentWindow, InParentView)
	{
	}

	public override void OnGUI()
	{
		GUI.contentColor = Color.green;
		try
		{
			DictionaryView<string, CheatCommandGroup> repositories = Singleton<CheatCommandsRepository>.instance.repositories;
			DebugHelper.Assert(repositories != null);
			DictionaryView<string, CheatCommandGroup>.Enumerator enumerator = repositories.GetEnumerator();
			int num = 0;
			int num2 = 0;
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, CheatCommandGroup> current = enumerator.Current;
				string key = current.Key;
				if (num2++ >= this.ParentView.skipCount && base.DrawButton(key, string.Empty))
				{
					ConsoleViewMobile arg_80_0 = this.ParentView;
					KeyValuePair<string, CheatCommandGroup> current2 = enumerator.Current;
					arg_80_0.SelectGroup(current2.Value);
					break;
				}
				GUILayout.Space((float)CommandDisplayBasicState.SpaceHeight);
				num++;
			}
		}
		finally
		{
			GUI.contentColor = Color.white;
		}
	}

	protected override void OnResetSkipCount()
	{
		this.ParentView.UpdateSkipCount(Singleton<CheatCommandsRepository>.instance.repositories.Count);
	}
}
