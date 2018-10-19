using Assets.Scripts.Framework;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public struct MonsterDropItemCreator
	{
		private MonsterWrapper MonsterRef;

		public void MakeDropItemIfNeed(MonsterWrapper InMonster, ObjWrapper InTarget)
		{
			DebugHelper.Assert(InMonster != null);
			ResMonsterCfgInfo cfgInfo = InMonster.cfgInfo;
			if (cfgInfo == null || cfgInfo.iBufDropID == 0)
			{
				return;
			}
			this.MonsterRef = InMonster;
			ushort num = FrameRandom.Random(10000u);
			if ((int)num < cfgInfo.iBufDropRate)
			{
				this.SpawnBuf(cfgInfo.iBufDropID);
			}
		}

		private void SpawnBuf(int BufID)
		{
			ResBufDropInfo dataByKey = GameDataMgr.bufDropInfoDatabin.GetDataByKey((uint)BufID);
			DebugHelper.Assert(dataByKey != null, "找不到Buf，id={0}", new object[]
			{
				BufID
			});
			if (dataByKey == null)
			{
				return;
			}
			int num = 0;
			uint num2 = 0u;
			for (int i = 0; i < 10; i++)
			{
				if (dataByKey.astBufs[i].dwBufID == 0u)
				{
					break;
				}
				num++;
				num2 += dataByKey.astBufs[i].dwProbability;
			}
			if (num <= 0)
			{
				return;
			}
			int num3 = (int)FrameRandom.Random(num2);
			ResBufConfigInfo resBufConfigInfo = null;
			for (int j = 0; j < num; j++)
			{
				if ((long)num3 < (long)((ulong)dataByKey.astBufs[j].dwProbability))
				{
					resBufConfigInfo = dataByKey.astBufs[j];
					break;
				}
				num3 -= (int)dataByKey.astBufs[j].dwProbability;
			}
			DebugHelper.Assert(resBufConfigInfo != null);
			SimpleParabolaEffect inDropdownEffect = new SimpleParabolaEffect(this.MonsterRef.actor.location, this.TraceOnTerrain(this.MonsterRef.actor.location));
			PickupBufEffect inPickupEffect = new PickupBufEffect(resBufConfigInfo);
			Singleton<DropItemMgr>.instance.CreateItem(Utility.UTF8Convert(resBufConfigInfo.szPrefab), inDropdownEffect, inPickupEffect);
		}

		private VInt3 TraceOnTerrain(VInt3 InLocation)
		{
			Ray ray = new Ray((Vector3)InLocation, new Vector3(0f, -1f, 0f));
			RaycastHit raycastHit;
			if (!Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Scene")))
			{
				return InLocation;
			}
			return new VInt3(raycastHit.point);
		}
	}
}
