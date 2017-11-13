using System;

namespace Apollo
{
	public interface INotice : IApolloServiceBase
	{
		void ShowNotice(APOLLO_NOTICETYPE type, string scene);

		void HideNotice();

		void GetNoticeData(APOLLO_NOTICETYPE type, string scene, ref ApolloNoticeInfo info);
	}
}
