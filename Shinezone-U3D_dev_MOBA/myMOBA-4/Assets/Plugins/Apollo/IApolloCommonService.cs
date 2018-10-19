using System;

namespace Apollo
{
	public interface IApolloCommonService : IApolloServiceBase
	{
		event OnFeedbackNotifyHandle onFeedbackEvent;

		event OnCrashExtMessageNotifyHandle onCrashExtMessageEvent;

		event OnReceivedPushNotifyHandle onReceivedPushEvent;

		void PushInit();

		void OpenUrl(string openUrl);

		void OpenUrl(string openUrl, APOLLO_SCREENDIR screendir);

		bool OpenAmsCenter(string param);

		void Feedback(string body);

		string GetChannelId();

		string GetRegisterChannelId();

		void OpenWeiXinDeeplink(string link);

		string GetEncodeUrl(string openUrl);
	}
}
