using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;

[CheatCommandEntry("英雄")]
internal class CheatCommandHeroEntry
{
	[CheatCommandEntryMethod("技能/设置技能等级", true, false)]
	public static string SetHeroSkillLevel(SkillSlotType Slot, byte Level)
	{
		if (Singleton<GameStateCtrl>.instance.isBattleState)
		{
			FrameCommand<SetSkillLevelInBattleCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<SetSkillLevelInBattleCommand>();
			frameCommand.cmdData.SkillSlot = (byte)Slot;
			frameCommand.cmdData.SkillLevel = Level;
			frameCommand.Send();
			return "已发出指令";
		}
		return "仅限局内";
	}
}
