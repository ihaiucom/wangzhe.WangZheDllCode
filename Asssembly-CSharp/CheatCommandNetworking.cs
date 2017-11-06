using Assets.Scripts.Framework;
using CSProtocol;
using System;

public abstract class CheatCommandNetworking : CheatCommandCommon
{
	private static CSDT_CHEATCMD_DETAIL DummyDetail = new CSDT_CHEATCMD_DETAIL();

	public override string StartProcess(string[] InArguments)
	{
		string result;
		if (!this.CheckArguments(InArguments, out result))
		{
			return result;
		}
		if (this.messageID != 0)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1012u);
			cSPkg.stPkgData.stCheatCmd.iCheatCmd = this.messageID;
			cSPkg.stPkgData.stCheatCmd.stCheatCmdDetail = new CSDT_CHEATCMD_DETAIL();
			string text = this.Execute(InArguments, ref cSPkg.stPkgData.stCheatCmd.stCheatCmdDetail);
			if (text == CheatCommandBase.Done)
			{
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
			return text;
		}
		return this.Execute(InArguments, ref CheatCommandNetworking.DummyDetail);
	}

	protected sealed override string Execute(string[] InArguments)
	{
		throw new NotImplementedException();
	}

	protected abstract string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef);
}
