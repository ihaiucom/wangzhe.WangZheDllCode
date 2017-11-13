using System;

namespace Apollo
{
	public enum ApolloPlatform
	{
		None,
		Wechat,
		[Obsolete("Obsolete since 1.1.13, using Wechat instead.")]
		Weixin = 1,
		QQ,
		WTLogin,
		Guest = 5,
		AutoLogin,
		QR = 256,
		QRWechat,
		QRQQ,
		Kakao = 1000
	}
}
