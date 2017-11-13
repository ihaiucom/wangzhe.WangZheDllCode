using System;

namespace behaviac
{
	[Flags]
	public enum EActionResult
	{
		EAR_none = 0,
		EAR_success = 1,
		EAR_failure = 2,
		EAR_all = 3
	}
}
