using CSProtocol;
using System;

[ArgumentDescription(1, typeof(ulong), "ullUid", new object[]
{

}), ArgumentDescription(2, typeof(uint), "worldID", new object[]
{

}), ArgumentDescription(3, typeof(byte), "招募关系", new object[]
{

}), CheatCommand("工具/SetRecruitment", "设置招募关系", 89)]
internal class SetRecruitment : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		CheatCmdRef.stRecruitmentRelation = new CSDT_CHEAT_RECRUITMENTRELATION();
		CheatCmdRef.stRecruitmentRelation.stUin.ullUid = CheatCommandBase.SmartConvert<ulong>(InArguments[0]);
		CheatCmdRef.stRecruitmentRelation.stUin.dwLogicWorldId = CheatCommandBase.SmartConvert<uint>(InArguments[1]);
		CheatCmdRef.stRecruitmentRelation.bRecruitmentType = CheatCommandBase.SmartConvert<byte>(InArguments[2]);
		return CheatCommandBase.Done;
	}
}
