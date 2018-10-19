using Assets.Scripts.Framework;
using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class VoiceStateNetCore
	{
		public static void Send_Acnt_VoiceState(CS_VOICESTATE_TYPE type)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4850u);
			cSPkg.stPkgData.stAcntVoiceState.bVoiceState = (byte)type;
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
		}

		[MessageHandler(4851)]
		public static void On_NTF_VOICESTATE(CSPkg msg)
		{
			SCPKG_NTF_VOICESTATE stNtfVoiceState = msg.stPkgData.stNtfVoiceState;
			MonoSingleton<VoiceSys>.instance.SetVoiceState(stNtfVoiceState.ullAcntUid, (CS_VOICESTATE_TYPE)stNtfVoiceState.bVoiceState);
			if (Singleton<CBattleSystem>.instance.BattleStatView != null)
			{
				Singleton<CBattleSystem>.instance.BattleStatView.RefreshVoiceStateIfNess();
			}
		}
	}
}
