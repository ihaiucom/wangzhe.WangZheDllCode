using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using System;

namespace Assets.Scripts.GameLogic
{
	internal class LockModeUISelector : Singleton<LockModeUISelector>
	{
		public void OnClickBattleUI(uint _targetID)
		{
			OperateMode operateMode = OperateMode.DefaultMode;
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer != null)
			{
				operateMode = hostPlayer.GetOperateMode();
			}
			if (operateMode == OperateMode.DefaultMode)
			{
				return;
			}
			if (_targetID != 0u)
			{
				Singleton<NetLockAttackTarget>.GetInstance().SendLockAttackTarget(_targetID);
			}
		}
	}
}
