using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameSystem
{
	public class ACTOR_INFO : PooledClassObject
	{
		private const int DataSize_ = 6;

		private const int DataSizeBytes_ = 1;

		public VInt3[] location;

		public int[] camps;

		public bool bDistOnly;

		public override void OnUse()
		{
			base.OnUse();
			this.location = null;
			this.camps = null;
			this.bDistOnly = false;
		}

		public override void OnRelease()
		{
			this.location = null;
			this.camps = null;
			this.bDistOnly = false;
			base.OnRelease();
		}

		public static int GetDataSize()
		{
			return 6;
		}

		public static int GetDataSizeBytes()
		{
			return 1;
		}
	}
}
