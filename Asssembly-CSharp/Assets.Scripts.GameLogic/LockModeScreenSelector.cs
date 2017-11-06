using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	internal class LockModeScreenSelector : Singleton<LockModeScreenSelector>
	{
		private Ray screenRay;

		private Plane curPlane;

		public override void Init()
		{
			this.curPlane = new Plane(new Vector3(0f, 1f, 0f), 0f);
		}

		public void OnClickBattleScene(Vector2 _screenPos)
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
			uint num = 0u;
			Ray ray = Camera.main.ScreenPointToRay(_screenPos);
			Player hostPlayer2 = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			float distance;
			if (this.curPlane.Raycast(ray, out distance))
			{
				Vector3 point = ray.GetPoint(distance);
				if (hostPlayer2 != null)
				{
					num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref hostPlayer2.Captain, (VInt3)point, 3000);
				}
				if (num != 0u)
				{
					Singleton<NetLockAttackTarget>.GetInstance().SendLockAttackTarget(num);
				}
			}
		}
	}
}
