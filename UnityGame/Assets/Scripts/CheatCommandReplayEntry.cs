using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

[CheatCommandEntry("性能")]
internal class CheatCommandReplayEntry
{
	public static bool heroPerformanceTest;

	public static bool commitFOWMaterial = true;

	[CheatCommandEntryMethod("单英雄测试 开", true, false)]
	public static string SingleHeroTestOn()
	{
		CheatCommandReplayEntry.heroPerformanceTest = true;
		return CheatCommandBase.Done;
	}

	[CheatCommandEntryMethod("单英雄测试 关", true, false)]
	public static string SingleHeroTestOff()
	{
		CheatCommandReplayEntry.heroPerformanceTest = false;
		return CheatCommandBase.Done;
	}

	[CheatCommandEntryMethod("PROFILE!", true, false)]
	public static string Profile()
	{
		MonoSingleton<ConsoleWindow>.instance.isVisible = false;
		MonoSingleton<SProfiler>.GetInstance().ToggleVisible();
		return CheatCommandBase.Done;
	}

	[CheatCommandEntryMethod("PROFILE 300->5000!", true, false)]
	public static string Profile5000Auto()
	{
		FrameCommand<SwitchActorAutoAICommand> frameCommand = FrameCommandFactory.CreateFrameCommand<SwitchActorAutoAICommand>();
		frameCommand.cmdData.IsAutoAI = 1;
		frameCommand.Send();
		MonoSingleton<ConsoleWindow>.instance.isVisible = false;
		MonoSingleton<SProfiler>.GetInstance().StartProfileNFrames(5000, 300u);
		return CheatCommandBase.Done;
	}

	[CheatCommandEntryMethod("PROFILE 5000!", true, false)]
	public static string Profile5000()
	{
		MonoSingleton<ConsoleWindow>.instance.isVisible = false;
		MonoSingleton<SProfiler>.GetInstance().StartProfileNFrames(5000, 0u);
		return CheatCommandBase.Done;
	}

	[CheatCommandEntryMethod("PROFILE 10000!", true, false)]
	public static string Profile10000()
	{
		MonoSingleton<ConsoleWindow>.instance.isVisible = false;
		MonoSingleton<SProfiler>.GetInstance().StartProfileNFrames(10000, 0u);
		return CheatCommandBase.Done;
	}

	[CheatCommandEntryMethod(" 锁帧模式", true, false)]
	public static string LockFPS()
	{
		GameFramework instance = MonoSingleton<GameFramework>.GetInstance();
		instance.LockFPS_SGame = !instance.LockFPS_SGame;
		return instance.LockFPS_SGame ? "SGAME" : "UNITY";
	}

	[CheatCommandEntryMethod("Battle Form模式", true, false)]
	public static string BattleFormCanvasMode()
	{
		Canvas component = Singleton<CBattleSystem>.GetInstance().FormScript.gameObject.GetComponent<Canvas>();
		if (component.renderMode == RenderMode.ScreenSpaceCamera)
		{
			component.renderMode = RenderMode.WorldSpace;
		}
		else if (component.renderMode == RenderMode.WorldSpace)
		{
			component.renderMode = RenderMode.ScreenSpaceCamera;
		}
		return component.renderMode.ToString();
	}

	[CheatCommandEntryMethod("小地图开关", true, false)]
	public static string MapPanelSwitch()
	{
		GameObject gameObject = Singleton<CBattleSystem>.GetInstance().FormScript.transform.Find("MapPanel").gameObject;
		gameObject.SetActive(!gameObject.activeSelf);
		return CheatCommandBase.Done;
	}

	[CheatCommandEntryMethod("迷雾材质提交开关", true, false)]
	public static string CommitFOWMaterialSwitch()
	{
		CheatCommandReplayEntry.commitFOWMaterial = !CheatCommandReplayEntry.commitFOWMaterial;
		return CheatCommandReplayEntry.commitFOWMaterial.ToString();
	}

	[CheatCommandEntryMethod(" 关闭战斗UI", true, false)]
	public static string CloseFormBattle()
	{
		CUIFormScript form = Singleton<CUIManager>.instance.GetForm(FightForm.s_battleUIForm);
		if (form != null)
		{
			form.gameObject.CustomSetActive(false);
			return "已关闭";
		}
		return "不在战斗中";
	}

	[CheatCommandEntryMethod(" 战斗UI停止绘制", true, false)]
	public static string StopFormBattleDraw()
	{
		CUIFormScript form = Singleton<CUIManager>.instance.GetForm(FightForm.s_battleUIForm);
		if (form != null)
		{
			form.gameObject.CustomSetActive(true);
			form.Hide(enFormHideFlag.HideByCustom, true);
			return "已停止绘制";
		}
		return "不在战斗中";
	}

	[CheatCommandEntryMethod(" 战斗UI恢复显示和绘制", true, false)]
	public static string ShowFormBattle()
	{
		CUIFormScript form = Singleton<CUIManager>.instance.GetForm(FightForm.s_battleUIForm);
		if (form != null)
		{
			form.gameObject.CustomSetActive(true);
			form.Appear(enFormHideFlag.HideByCustom, true);
			return "已恢复";
		}
		return "不在战斗中";
	}

	[CheatCommandEntryMethod("切换Raycast模式", true, false)]
	public static string RayCastWatch()
	{
		if (Singleton<WatchController>.instance.IsWatching)
		{
			CUIFormScript watchFormScript = Singleton<CBattleSystem>.instance.WatchFormScript;
			if (watchFormScript != null)
			{
				if (watchFormScript.m_sgameGraphicRaycaster.raycastMode == SGameGraphicRaycaster.RaycastMode.Unity)
				{
					watchFormScript.m_sgameGraphicRaycaster.raycastMode = SGameGraphicRaycaster.RaycastMode.Sgame_tile;
					return "切换到[RaycastMode.Sgame_tile]";
				}
				if (watchFormScript.m_sgameGraphicRaycaster.raycastMode == SGameGraphicRaycaster.RaycastMode.Sgame_tile)
				{
					watchFormScript.m_sgameGraphicRaycaster.raycastMode = SGameGraphicRaycaster.RaycastMode.Unity;
					return "切换到[RaycastMode.Unity]";
				}
			}
			return "观战但是没有找到WatchFormScript";
		}
		string[] array = new string[]
		{
			"Form_Battle_Part_Joystick.prefab",
			"Form_Battle_Part_CameraMove.prefab",
			"Form_Battle_Part_Scene.prefab",
			"Form_Battle_Part_SkillCursor.prefab",
			"Form_Battle_Part_SkillBtn.prefab",
			"Form_Battle_Part_EnemyHeroAtk.prefab",
			"Form_Battle_Part_CameraDragPanel.prefab"
		};
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < array.Length; i++)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm("UGUI/Form/Battle/Part/" + array[i]);
			if (form != null)
			{
				if (form.m_sgameGraphicRaycaster.raycastMode == SGameGraphicRaycaster.RaycastMode.Unity)
				{
					form.m_sgameGraphicRaycaster.raycastMode = SGameGraphicRaycaster.RaycastMode.Sgame_tile;
					num2++;
				}
				else if (form.m_sgameGraphicRaycaster.raycastMode == SGameGraphicRaycaster.RaycastMode.Sgame_tile)
				{
					form.m_sgameGraphicRaycaster.raycastMode = SGameGraphicRaycaster.RaycastMode.Unity;
					num++;
				}
			}
		}
		return string.Format("战斗中 u[{0}] s[{1}]", num, num2);
	}
}
