using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;

[CheatCommandEntry("关卡")]
internal class CheatCommandLevelEntry
{
	[CheatCommandEntryMethod("通过当前关卡", true, false)]
	public static string FinishLevel()
	{
		if (!Singleton<GameStateCtrl>.instance.isBattleState)
		{
			return "不在副本里面你通过个毛线？？";
		}
		if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaModeWithOutGuide())
		{
			return "单人游戏才能用";
		}
		Singleton<PVEReviveHeros>.instance.ClearTimeOutTimer();
		MonoSingleton<NewbieGuideManager>.GetInstance().StopCurrentGuide();
		Singleton<LobbyLogic>.instance.ReqSingleGameFinish(true, true);
		return CheatCommandBase.Done;
	}
}
