using System;

public class TGALive
{
	public static int AccountType_QQ = 1;

	public static int AccountType_WeChat = 2;

	public static int AccountType_Tourist = 3;

	public static void init(string jsonString)
	{
		TGALiveSDK.init(jsonString);
	}

	public static void start(string token, int postion)
	{
		TGALiveSDK.start(token, postion);
	}

	public static bool available()
	{
		return TGALiveSDK.available();
	}

	public static void battleInvitation(string jsonString)
	{
		TGALiveSDK.battleInvitation(jsonString);
	}
}
