using System;
using System.Collections.Generic;
using UnityEngine;

[CommandDisplay]
internal class CommandDisplayState : CommandDisplayBasicState
{
	private ICheatCommand CheatCommand;

	public override bool canScroll
	{
		get
		{
			return false;
		}
	}

	public CommandDisplayState(ConsoleWindow InParentWindow, ConsoleViewMobile InParentView) : base(InParentWindow, InParentView)
	{
	}

	public void ResetCheatCommand(ICheatCommand InCommand)
	{
		this.CheatCommand = InCommand;
	}

	public override void OnGUI()
	{
		if (this.CheatCommand == null)
		{
			return;
		}
		GUI.contentColor = Color.green;
		GUILayout.Label(this.CheatCommand.fullyHelper, this.ParentView.CustomSmallLabelStyle, new GUILayoutOption[0]);
		GUI.contentColor = Color.yellow;
		GUILayout.Label(base.logger.message, this.ParentView.CustomSmallLabelStyle, new GUILayoutOption[0]);
		GUI.contentColor = Color.white;
		this.DrawArugments();
	}

	protected void DrawArugments()
	{
		ArgumentDescriptionAttribute[] argumentsTypes = this.CheatCommand.argumentsTypes;
		string[] arguments = this.CheatCommand.arguments;
		int num = 0;
		if (argumentsTypes != null && argumentsTypes.Length > 0)
		{
			DebugHelper.Assert(argumentsTypes.Length == arguments.Length);
			for (int i = 0; i < argumentsTypes.Length; i++)
			{
				ArgumentDescriptionAttribute inArgAttr = argumentsTypes[i];
				if (!this.DrawArgument(inArgAttr, i, argumentsTypes, ref arguments, ref arguments[i]))
				{
					break;
				}
				num++;
			}
		}
		if (base.DrawButton(this.CheatCommand.comment, string.Empty))
		{
			base.logger.AddMessage(this.CheatCommand.StartProcess(arguments));
		}
	}

	private bool DrawArgument(ArgumentDescriptionAttribute InArgAttr, int InIndex, ArgumentDescriptionAttribute[] InArgTypes, ref string[] OutValues, ref string OutValue)
	{
		if (InArgAttr.isOptional && this.ShouldSkip(InArgAttr, ref OutValues))
		{
			return false;
		}
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		base.DrawLabel(InArgAttr.name);
		string text = string.Format("Argument_{0}", GUIUtility.GetControlID(FocusType.Keyboard));
		GUI.SetNextControlName(text);
		OutValue = GUILayout.TextField(OutValue, this.ParentView.CustomTextFieldStyle, new GUILayoutOption[]
		{
			GUILayout.Width(200f)
		});
		string nameOfFocusedControl = GUI.GetNameOfFocusedControl();
		if (nameOfFocusedControl == text)
		{
			IArgumentDescription description = Singleton<ArgumentDescriptionRepository>.instance.GetDescription(InArgAttr.argumentType);
			DebugHelper.Assert(description != null);
			List<string> list = description.FilteredCandinates(InArgAttr.argumentType, OutValue);
			if (list != null && list.get_Count() > 0)
			{
				for (int i = 0; i < list.get_Count(); i++)
				{
					string text2 = list.get_Item(i);
					if (!text2.Equals(OutValue, 3) && base.DrawButton(text2, string.Empty))
					{
						OutValue = text2;
						break;
					}
				}
			}
		}
		GUILayout.EndHorizontal();
		return true;
	}

	private bool ShouldSkip(ArgumentDescriptionAttribute InArgAttr, ref string[] ExistsValues)
	{
		DebugHelper.Assert(InArgAttr.isOptional);
		DependencyDescription[] depends = InArgAttr.depends;
		for (int i = 0; i < depends.Length; i++)
		{
			DependencyDescription dependencyDescription = depends[i];
			string text = ExistsValues[dependencyDescription.dependsIndex];
			Type argumentType = this.CheatCommand.argumentsTypes[dependencyDescription.dependsIndex].argumentType;
			IArgumentDescription description = Singleton<ArgumentDescriptionRepository>.instance.GetDescription(argumentType);
			DebugHelper.Assert(description != null);
			text = description.GetValue(argumentType, text);
			if (dependencyDescription.ShouldBackOff(text))
			{
				return false;
			}
		}
		return true;
	}

	public override void OnStateEnter()
	{
		base.logger.Clear();
	}
}
