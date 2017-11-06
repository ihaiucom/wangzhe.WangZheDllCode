using Assets.Scripts.GameSystem;
using System;

[CheatCommand("工具/SetShowUID", "显示uid", 0)]
internal class SetShowUID : CheatCommandCommon
{
	protected override string Execute(string[] InArguments)
	{
		Singleton<CFriendContoller>.instance.model.bShowUID = !Singleton<CFriendContoller>.instance.model.bShowUID;
		Singleton<EventRouter>.GetInstance().BroadCastEvent("Friend_Game_State_Refresh");
		return CheatCommandBase.Done;
	}
}
