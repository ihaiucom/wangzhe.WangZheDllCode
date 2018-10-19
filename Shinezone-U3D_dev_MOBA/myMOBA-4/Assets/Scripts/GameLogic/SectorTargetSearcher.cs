using Assets.Scripts.Common;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class SectorTargetSearcher : Singleton<SectorTargetSearcher>
	{
		public ActorRoot GetEnemyTarget(ActorRoot InActor, int srchR, Vector3 useDirection, float srchAngle, uint filter)
		{
			ActorRoot result = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			float num2 = srchAngle;
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.Count;
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors[j].handle;
						if (((ulong)filter & (ulong)(1L << (int)(handle.TheActorMeta.ActorType & (ActorTypeDef)31))) <= 0uL && !handle.ActorControl.IsDeadState && handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp) && InActor.CanAttack(handle))
						{
							ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
							if (sqrMagnitudeLong2D < num)
							{
								float num3 = Mathf.Abs(Vector3.Angle(useDirection, ((Vector3)(handle.location - InActor.location)).normalized));
								if (num3 < num2)
								{
									num2 = num3;
									result = handle;
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
