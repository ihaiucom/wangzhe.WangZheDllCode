using System;

namespace Apollo
{
	public interface IApolloQuickLoginService : IApolloServiceBase
	{
		void SwitchUser(bool useExternalAccount);

		void SetQuickLoginNotify(ApolloQuickLoginNotify callback);
	}
}
