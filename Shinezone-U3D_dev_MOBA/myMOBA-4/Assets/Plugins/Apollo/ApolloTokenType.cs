using System;

namespace Apollo
{
	public enum ApolloTokenType
	{
		None,
		Access,
		Refresh,
		Pay,
		STSignature,
		[Obsolete("Obsolete since 1.1.13, using Access instead.")]
		QQAccess = 1,
		[Obsolete("Obsolete since 1.1.13, using Pay instead.")]
		QQPay = 3,
		[Obsolete("Obsolete since 1.1.13, using Access instead.")]
		WXAccess = 1,
		[Obsolete("Obsolete since 1.1.13, using Refresh instead.")]
		WXRefresh,
		[Obsolete("Obsolete since 1.1.13, using Access instead.")]
		GuestAccess = 1
	}
}
