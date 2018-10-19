using System;

namespace Assets.Scripts.GameSystem
{
	public class CSignalTipsElement
	{
		public enum EType
		{
			None,
			Signal,
			InBattleMsg
		}

		public CSignalTipsElement.EType type;

		public CSignalTipsElement(CSignalTipsElement.EType type)
		{
			this.type = type;
		}
	}
}
