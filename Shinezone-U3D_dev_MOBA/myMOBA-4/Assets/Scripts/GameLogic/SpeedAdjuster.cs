using Assets.Scripts.Framework;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class SpeedAdjuster : Singleton<SpeedAdjuster>
	{
		private List<RangeConfig> SpeedUpConfigs = new List<RangeConfig>();

		private bool bActiveWhenSpeedUp;

		private bool bReferenceBaseSpeedWhenSpeedUp;

		private bool bUseStepAdjustWhenSpeedUp = true;

		private List<RangeConfig> SpeedDownConfigs = new List<RangeConfig>();

		private bool bActiveWhenSpeedDown;

		private bool bReferenceBaseSpeedWhenSpeedDown;

		private bool bUseStepAdjustWhenSpeedDown = true;

		private bool bActiveSys;

		public bool isActiveSys
		{
			get
			{
				return this.bActiveSys;
			}
		}

		public bool isActiveWhenSpeedUp
		{
			get
			{
				return this.bActiveSys && this.bActiveWhenSpeedUp && this.SpeedUpConfigs.Count != 0;
			}
		}

		public bool isActiveWhenSpeedDown
		{
			get
			{
				return this.bActiveSys && this.bActiveWhenSpeedDown && this.SpeedDownConfigs.Count != 0;
			}
		}

		public override void Init()
		{
			base.Init();
			ResSpeedAdjustConfig dataByKey = GameDataMgr.speedAdjusterDatabin.GetDataByKey(0u);
			ResSpeedAdjustConfig dataByKey2 = GameDataMgr.speedAdjusterDatabin.GetDataByKey(1u);
			this.bActiveSys = (dataByKey != null && dataByKey2 != null);
			if (!this.bActiveSys)
			{
				return;
			}
			this.SpeedUpConfigs.Clear();
			this.bActiveWhenSpeedUp = (dataByKey.bValid == 1);
			this.bReferenceBaseSpeedWhenSpeedUp = (dataByKey.bReferenceBaseSpeed == 1);
			this.bUseStepAdjustWhenSpeedUp = (dataByKey.bStepAdjust == 1);
			for (int i = 0; i < dataByKey.astRangeConfigs.Length; i++)
			{
				if (dataByKey.astRangeConfigs[i].bValid != 1)
				{
					break;
				}
				RangeConfig item = default(RangeConfig);
				item.MinValue = dataByKey.astRangeConfigs[i].iMinValue;
				item.MaxValue = dataByKey.astRangeConfigs[i].iMaxValue;
				item.Attenuation = (int)dataByKey.astRangeConfigs[i].bAttenuation;
				this.SpeedUpConfigs.Add(item);
			}
			this.SpeedDownConfigs.Clear();
			this.bActiveWhenSpeedDown = (dataByKey2.bValid == 1);
			this.bReferenceBaseSpeedWhenSpeedDown = (dataByKey2.bReferenceBaseSpeed == 1);
			this.bUseStepAdjustWhenSpeedDown = (dataByKey2.bStepAdjust == 1);
			for (int j = 0; j < dataByKey2.astRangeConfigs.Length; j++)
			{
				if (dataByKey2.astRangeConfigs[j].bValid != 1)
				{
					break;
				}
				RangeConfig item2 = default(RangeConfig);
				item2.MinValue = dataByKey2.astRangeConfigs[j].iMinValue;
				item2.MaxValue = dataByKey2.astRangeConfigs[j].iMaxValue;
				item2.Attenuation = (int)dataByKey2.astRangeConfigs[j].bAttenuation;
				this.SpeedDownConfigs.Add(item2);
			}
		}

		public override void UnInit()
		{
			base.UnInit();
			this.SpeedUpConfigs.Clear();
			this.SpeedDownConfigs.Clear();
		}

		public int HandleSpeedAdjust(int BaseValue, int TotalValue)
		{
			if (!this.bActiveSys)
			{
				return TotalValue;
			}
			int num = TotalValue - BaseValue;
			bool flag = num > 0;
			if ((flag && !this.isActiveWhenSpeedUp) || (!flag && !this.isActiveWhenSpeedDown))
			{
				return TotalValue;
			}
			return (!flag) ? this.HandleSpeedDown(BaseValue, TotalValue, BaseValue) : this.HandleSpeedUp(BaseValue, TotalValue, BaseValue);
		}

		private int HandleSpeedUp(int baseValue, int totalValue, int orignalSpeed)
		{
			if (this.bUseStepAdjustWhenSpeedUp)
			{
				int num = 0;
				bool flag = false;
				for (int i = 0; i < this.SpeedUpConfigs.Count; i++)
				{
					RangeConfig rangeConfig = this.SpeedUpConfigs[i];
					int num2 = 0;
					int num3 = 0;
					if (rangeConfig.Intersect((!this.bReferenceBaseSpeedWhenSpeedUp) ? 0 : baseValue, orignalSpeed, totalValue, out num2, out num3))
					{
						flag = true;
						int num4 = num3 - num2;
						num4 *= 100 - rangeConfig.Attenuation;
						num4 /= 100;
						num += num4;
					}
					else if (flag)
					{
						break;
					}
				}
				return orignalSpeed + num;
			}
			for (int j = 0; j < this.SpeedUpConfigs.Count; j++)
			{
				if (this.SpeedUpConfigs[j].Intersect((!this.bReferenceBaseSpeedWhenSpeedUp) ? 0 : baseValue, totalValue))
				{
					return this.SpeedUpConfigs[j].MinValue + (totalValue - this.SpeedUpConfigs[j].MinValue) * (100 - this.SpeedUpConfigs[j].Attenuation) / 100;
				}
			}
			return totalValue;
		}

		private int HandleSpeedDown(int baseValue, int totalValue, int orignalSpeed)
		{
			if (this.bUseStepAdjustWhenSpeedDown)
			{
				int num = 0;
				bool flag = false;
				for (int i = 0; i < this.SpeedDownConfigs.Count; i++)
				{
					RangeConfig rangeConfig = this.SpeedDownConfigs[i];
					int num2 = 0;
					int num3 = 0;
					if (rangeConfig.Intersect((!this.bReferenceBaseSpeedWhenSpeedDown) ? 0 : baseValue, totalValue, orignalSpeed, out num2, out num3))
					{
						flag = true;
						int num4 = num3 - num2;
						num4 *= 100 - rangeConfig.Attenuation;
						num4 /= 100;
						num += num4;
					}
					else if (flag)
					{
						break;
					}
				}
				return orignalSpeed - num;
			}
			for (int j = 0; j < this.SpeedDownConfigs.Count; j++)
			{
				if (this.SpeedDownConfigs[j].Intersect((!this.bReferenceBaseSpeedWhenSpeedUp) ? 0 : baseValue, totalValue))
				{
					return this.SpeedDownConfigs[j].MaxValue - (this.SpeedDownConfigs[j].MaxValue - totalValue) * (100 - this.SpeedDownConfigs[j].Attenuation) / 100;
				}
			}
			return totalValue;
		}
	}
}
