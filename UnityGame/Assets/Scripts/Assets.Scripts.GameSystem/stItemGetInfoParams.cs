using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public struct stItemGetInfoParams
	{
		public byte getType;

		public ResLevelCfgInfo levelInfo;

		public bool isCanDo;

		public string errorStr;
	}
}
