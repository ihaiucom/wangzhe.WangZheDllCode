using CSProtocol;
using System;

[ArgumentDescription(0, typeof(int), "ID", new object[]
{

}), CheatCommand("通用/头像框/DelHeadImage", "删除头像框", 66)]
internal class DelHeadImageCommand : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stDelHeadImage = new CSDT_CHEAT_HEADIMAGE_DEL();
		CheatCmdRef.stDelHeadImage.dwHeadImgID = CheatCommandBase.SmartConvert<uint>(InArguments[0]);
		return CheatCommandBase.Done;
	}
}
