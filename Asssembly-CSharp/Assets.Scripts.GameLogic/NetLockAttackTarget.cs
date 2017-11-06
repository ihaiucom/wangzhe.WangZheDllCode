using Assets.Scripts.Framework;
using System;

namespace Assets.Scripts.GameLogic
{
	internal class NetLockAttackTarget : Singleton<NetLockAttackTarget>
	{
		public void SendLockAttackTarget(uint _targetID)
		{
			FrameCommand<LockAttackTargetCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<LockAttackTargetCommand>();
			frameCommand.cmdData.LockAttackTarget = _targetID;
			frameCommand.Send();
		}
	}
}
