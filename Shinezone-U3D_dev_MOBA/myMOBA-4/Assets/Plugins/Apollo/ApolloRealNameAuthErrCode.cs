using System;

namespace Apollo
{
	public enum ApolloRealNameAuthErrCode
	{
		SystemError = 1,
		NoAuth,
		RequestFrequently,
		NoRecord,
		AlreadyRegisted,
		BindCountLimit,
		UserNoRegist,
		InvalidParam,
		InvalidIDCard,
		InvalidBirth,
		InvalidChineseName,
		InvalidToken
	}
}
