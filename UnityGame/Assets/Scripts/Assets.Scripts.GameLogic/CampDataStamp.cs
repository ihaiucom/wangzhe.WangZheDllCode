using Assets.Scripts.Common;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	internal class CampDataStamp
	{
		public CampData[] CampGolds = new CampData[3];

		public CampDataStamp()
		{
			for (int i = 0; i < this.CampGolds.Length; i++)
			{
				this.CampGolds[i] = new CampData();
			}
		}

		public void Clear()
		{
			for (int i = 0; i < this.CampGolds.Length; i++)
			{
				this.CampGolds[i].Clear();
			}
		}

		public void OnHeroGoldCoinChanged(ref PoolObjHandle<ActorRoot> InActor, int InChangedValue, bool bInIsIncome, PoolObjHandle<ActorRoot> target)
		{
			byte b = (byte)InActor.handle.TheActorMeta.ActorCamp;
			if ((int)b < this.CampGolds.Length)
			{
				this.CampGolds[(int)b].Golds += InChangedValue;
				this.RefreshFlags(b);
			}
		}

		public void GetMaxCampStat(COM_PLAYERCAMP InFrom, COM_PLAYERCAMP InTo, out int OutMaxPositive, out int OutMaxNegative)
		{
			OutMaxPositive = this.CampGolds[(int)InFrom].PositiveGolds[(int)InTo];
			OutMaxNegative = this.CampGolds[(int)InFrom].NegativeGolds[(int)InTo];
		}

		public int CalcCampStat(COM_PLAYERCAMP InFrom, COM_PLAYERCAMP InTo)
		{
			return this.CampGolds[(int)InFrom].Golds - this.CampGolds[(int)InTo].Golds;
		}

		private void RefreshFlags(byte InChangedIndex)
		{
			CampData campData = this.CampGolds[(int)InChangedIndex];
			for (int i = 0; i < this.CampGolds.Length; i++)
			{
				if ((int)InChangedIndex != i)
				{
					CampData campData2 = this.CampGolds[i];
					int num = campData.Golds - campData2.Golds;
					if (num < 0 && num < campData.NegativeGolds[i])
					{
						campData.NegativeGolds[i] = num;
						campData2.PositiveGolds[(int)InChangedIndex] = num * -1;
					}
					else if (num > 0 && num > campData.PositiveGolds[i])
					{
						campData.PositiveGolds[i] = num;
						campData2.NegativeGolds[(int)InChangedIndex] = num * -1;
					}
				}
			}
		}
	}
}
