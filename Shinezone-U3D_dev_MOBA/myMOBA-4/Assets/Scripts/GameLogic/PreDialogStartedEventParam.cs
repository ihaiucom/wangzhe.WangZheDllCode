using System;

namespace Assets.Scripts.GameLogic
{
	public struct PreDialogStartedEventParam
	{
		public int PreDialogId;

		public PreDialogStartedEventParam(int inPreDialogId)
		{
			this.PreDialogId = inPreDialogId;
		}
	}
}
