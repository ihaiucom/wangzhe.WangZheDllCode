using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using System;

[CheatCommandEntry("战斗")]
internal class CheatCommandBattleEntry : BoolenFlagConverter
{
	[CheatCommandEntryMethod("切换无敌状态", true, false)]
	public static string GodMode()
	{
		if (!LobbyMsgHandler.isHostGMAcnt)
		{
			return "没有gm权限";
		}
		Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
		if (hostPlayer != null && hostPlayer.Captain && hostPlayer.Captain.handle.ActorControl is HeroWrapper)
		{
			HeroWrapper heroWrapper = hostPlayer.Captain.handle.ActorControl as HeroWrapper;
			FrameCommand<SwitchActorSwitchGodMode> frameCommand = FrameCommandFactory.CreateFrameCommand<SwitchActorSwitchGodMode>();
			frameCommand.cmdData.IsGodMode = ((!heroWrapper.bGodMode) ? (sbyte)1 : (sbyte)0);
			frameCommand.Send();
			return CheatCommandBase.Done;
		}
		return "无法获取到正确的角色信息";
	}

	[CheatCommandEntryMethod("切换一击必杀", true, false)]
	public static string SuperKiller()
	{
		if (!LobbyMsgHandler.isHostGMAcnt)
		{
			return "没有gm权限";
		}
		Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
		if (hostPlayer != null && hostPlayer.Captain)
		{
			FrameCommand<SwitchActorSuperKillerCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<SwitchActorSuperKillerCommand>();
			frameCommand.cmdData.IsSuperKiller = ((!hostPlayer.Captain.handle.bOneKiller) ? (sbyte)1 : (sbyte)0);
			frameCommand.Send();
			return CheatCommandBase.Done;
		}
		return "无法获取到正确的角色信息";
	}

	[CheatCommandEntryMethod("自动战斗", true, false)]
	public static string ForceAutoAI()
	{
		FrameCommand<SwitchActorAutoAICommand> frameCommand = FrameCommandFactory.CreateFrameCommand<SwitchActorAutoAICommand>();
		frameCommand.cmdData.IsAutoAI = 1;
		frameCommand.Send();
		return CheatCommandBase.Done;
	}

	[CheatCommandEntryMethod("加1000局内金币", true, false)]
	public static string AddGoldCoinInBattle()
	{
		FrameCommand<PlayerAddGoldCoinInBattleCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerAddGoldCoinInBattleCommand>();
		frameCommand.cmdData.m_addValue = 50000u;
		frameCommand.Send();
		return CheatCommandBase.Done;
	}

	[CheatCommandEntryMethod("燃烧远征/刷新重置", true, false)]
	public static string BurnReset()
	{
		BurnExpeditionNetCore.Clear_ResetBurning_Limit();
		return CheatCommandBase.Done;
	}

	[CheatCommandEntryMethod("燃烧远征/刷新战力", true, false)]
	public static string BurnClearPower()
	{
		BurnExpeditionNetCore.Clear_ResetBurning_Power();
		return CheatCommandBase.Done;
	}

	[CheatCommandEntryMethod("升级", true, false)]
	public static string LevelUp()
	{
		return CheatCommandBattleEntry.SendCommand(EBattleCheatType.EBC_UpLevel);
	}

	[CheatCommandEntryMethod("重置等级", true, false)]
	public static string ResetLevel()
	{
		return CheatCommandBattleEntry.SendCommand(EBattleCheatType.EBC_ResetLevel);
	}

	[CheatCommandEntryMethod("开关冷却", true, false)]
	public static string ZeroCD()
	{
		return CheatCommandBattleEntry.SendCommand(EBattleCheatType.EBC_ZeroCD);
	}

	private static string SendCommand(EBattleCheatType InCheatType)
	{
		FrameCommand<PlayerCheatCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerCheatCommand>();
		frameCommand.cmdData.CheatType = (byte)InCheatType;
		frameCommand.Send();
		return CheatCommandBase.Done;
	}

	public static void ProcessCheat(byte InCheatType, ref PoolObjHandle<ActorRoot> Actor)
	{
		if (Actor)
		{
			switch (InCheatType)
			{
			case 0:
				if (Actor.handle.ValueComponent != null)
				{
					Actor.handle.ValueComponent.ForceSoulLevelUp();
				}
				break;
			case 1:
				if (Actor.handle.ValueComponent != null)
				{
					Actor.handle.ValueComponent.ForceSetSoulLevel(1);
				}
				break;
			case 2:
				if (Actor.handle.SkillControl != null)
				{
					Actor.handle.SkillControl.ToggleZeroCd();
				}
				break;
			}
		}
	}
}
