using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;

[ArgumentDescription(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, 4, typeof(int), "英雄ID(0表示所有英雄)", "0", new object[]
{

}), ArgumentDescription(5, typeof(uint), "该限免英雄需要的信用等级", new object[]
{

}), ArgumentDescription(3, typeof(byte), "时", new object[]
{

}), ArgumentDescription(2, typeof(byte), "日", new object[]
{

}), ArgumentDescription(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, 0, typeof(int), "年", "2016", new object[]
{

}), ArgumentDescription(1, typeof(EMonth), "月", new object[]
{

}), CheatCommand("英雄/解锁/SetFreeHero", "设置周免英雄", 32)]
internal class SetFreeHero : CheatCommandNetworking
{
	protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
	{
		string empty = string.Empty;
		if (this.CheckArguments(InArguments, out empty))
		{
			CheatCmdRef.stSetFreeHero = new CSDT_CHEAT_SET_FREE_HERO();
			CheatCmdRef.stSetFreeHero.wYear = CheatCommandBase.SmartConvert<ushort>(InArguments[0]);
			EMonth eMonth = (EMonth)CheatCommandBase.StringToEnum(InArguments[1], typeof(EMonth));
			CheatCmdRef.stSetFreeHero.bMonth = (byte)eMonth;
			CheatCmdRef.stSetFreeHero.bDay = CheatCommandBase.SmartConvert<byte>(InArguments[2]);
			CheatCmdRef.stSetFreeHero.bHour = CheatCommandBase.SmartConvert<byte>(InArguments[3]);
			CheatCmdRef.stSetFreeHero.dwHeroID = CheatCommandBase.SmartConvert<uint>(InArguments[4]);
			CheatCmdRef.stSetFreeHero.dwCreditLevel = CheatCommandBase.SmartConvert<uint>(InArguments[5]);
			return CheatCommandBase.Done;
		}
		return empty;
	}

	public override bool CheckArguments(string[] InArguments, out string OutMessage)
	{
		if (!base.CheckArguments(InArguments, out OutMessage))
		{
			return false;
		}
		if (CheatCommandBase.SmartConvert<ushort>(InArguments[0]) < 2014 || CheatCommandBase.SmartConvert<ushort>(InArguments[0]) > 2020)
		{
			OutMessage = "年份错误";
			return false;
		}
		if (CheatCommandBase.SmartConvert<byte>(InArguments[2]) < 1 || CheatCommandBase.SmartConvert<byte>(InArguments[2]) > 31)
		{
			OutMessage = "日期错误";
			return false;
		}
		uint HeroId = (uint)CheatCommandBase.SmartConvert<byte>(InArguments[4]);
		bool flag;
		if (HeroId == 0u)
		{
			flag = true;
		}
		else
		{
			ResHeroCfgInfo resHeroCfgInfo = GameDataMgr.heroDatabin.FindIf((ResHeroCfgInfo x) => x.dwCfgID == HeroId);
			flag = (resHeroCfgInfo != null);
		}
		if (!flag)
		{
			OutMessage = "错误的英雄ID";
			return false;
		}
		return true;
	}
}
