using Assets.Scripts.Common;
using System;

namespace tsf4g_tdr_csharp
{
	public class TdrBufBase : PooledClassObject
	{
		public TdrBufBase()
		{
			this.bChkReset = false;
		}
	}
}
