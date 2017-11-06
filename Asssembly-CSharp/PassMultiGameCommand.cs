using CSProtocol;
using System;

[ArgumentDescription(0, typeof(EWinOrLose), "胜利或失败", new object[]
{

}), CheatCommand("关卡/PassMultiGame", "通过多人pvp", 30)]
internal class PassMultiGameCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stPassMultiGame = new CSDT_CHEAT_PASS_MULTI_GAME();
		EWinOrLose eWinOrLose = (EWinOrLose)CheatCommandBase.StringToEnum(InArguments[0], typeof(EWinOrLose));
		CheatCmdRef.stPassMultiGame.bGameResult = ((eWinOrLose == EWinOrLose.胜利) ? 1 : 2);
		return CheatCommandBase.Done;
	}
}
