using System;

namespace Apollo
{
	public interface IApolloAccountService : IApolloServiceBase
	{
		event AccountInitializeHandle InitializeEvent;

		event AccountLoginHandle LoginEvent;

		event AccountLogoutHandle LogoutEvent;

		event RefreshAccessTokenHandler RefreshAtkEvent;

		event RealNameAuthHandler RealNameAutEvent;

		bool Initialize(ApolloBufferBase initInfo);

		void Login(ApolloPlatform platform);

		void Logout();

		ApolloResult GetRecord(ref ApolloAccountInfo accountInfo);

		void RefreshAccessToken();

		bool IsPlatformInstalled(ApolloPlatform platform);

		bool IsPlatformSupportApi(ApolloPlatform platform);

		[Obsolete("Obsolete since 1.1.6, use Initialize instead")]
		void SetPermission(uint permission);

		void Reset();

		void RealNameAuth(ApolloRealNameAuthInfo info);
	}
}
